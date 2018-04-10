using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Consul;
using DnsClient;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ProShare.ContactApi.Filters;
using ProShare.ContactApi.Infrastructure;
using ProShare.ContactApi.Models.Dtos;
using ProShare.ContactApi.Services;
using Resilience;

namespace ProShare.ContactApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.Configure<AppSetting>(Configuration);

            #region Consul 服务注册发现配置
          
            //添加服务发现
            //进行配置绑定       
            services.Configure<ServiceDiscoveryOptions>(Configuration.GetSection("ServiceDiscovery"));
            services.AddSingleton<IDnsQuery>(b =>
            {
                var serviceOption = b.GetRequiredService<IOptions<ServiceDiscoveryOptions>>();
                //添加Consul服务地址
                return new LookupClient(serviceOption.Value.Consul.DnsEndpoint.ToIPEndPoint());
            });

            //注入IConsulClient 用于向Consul进行注册 
            services.AddSingleton<IConsulClient>(b => new ConsulClient(cfg =>
            {
                //从依赖注入中读取 Consul 的配置信息
                var serviceConfiguration = b.GetRequiredService<IOptions<ServiceDiscoveryOptions>>().Value;
                if (!string.IsNullOrEmpty(serviceConfiguration.Consul.HttpEndpoint))
                {
                    // if not configured, the client will use the default value "127.0.0.1:8500"
                    cfg.Address = new Uri(serviceConfiguration.Consul.HttpEndpoint);
                }
            }));

            #endregion

            #region  IResilientHttp 配置

           
            //注入Application Service
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            //注入IResilientHttpClientFactory
            services.AddSingleton<IResilientHttpClientFactory, ResilientHttpClientFactory>(sp =>
            {
                //从ServiceProvider中获得Logger
                var logger = sp.GetRequiredService<ILogger<ResilientHttpClient>>();
                //获得httpContextAccessor
                var httpContextAccessor = sp.GetRequiredService<IHttpContextAccessor>();
                //设置重试次数
                var retryCount = 6;
                //设置在断路器启用前，允许失败次数
                var excetionAllowBeforeBreaking = 5;
                //返回ResilienceFactroy实例
                return new ResilientHttpClientFactory(logger, httpContextAccessor, retryCount, excetionAllowBeforeBreaking);
            });

            //注入IHttpClient 用ResilientHttpClient实现
            services.AddSingleton<IHttpClient, ResilientHttpClient>(sp => sp.GetService<IResilientHttpClientFactory>().CreateResilientHttpClient());

            #endregion

            services.AddScoped<IUserService, UserService>();

            services.AddMvc(option => {

                option.Filters.Add(typeof(GlobalExceptionFilter));
            });
         
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env,
               IApplicationLifetime applicationLifetime,
               IConsulClient consulClient,
               IOptions<ServiceDiscoveryOptions> serviceOptions)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            #region 向Consul进行服务注册     

            //获取服务启动地址绑定信息
            var features = app.Properties["server.Features"] as FeatureCollection;
            var addresses = features.Get<IServerAddressesFeature>()
                .Addresses
                .Select(p => new Uri(p));


            //在服务启动时,向Consul 中心进行注册
            applicationLifetime.ApplicationStarted.Register(() => {

                foreach (var address in addresses)
                {
                    //设定服务Id(全局唯一 unique）
                    var serviceId = $"{serviceOptions.Value.ServiceName}_{address.Host}:{address.Port}";

                    //设置健康检查方法
                    var httpCheck = new AgentServiceCheck()
                    {
                        DeregisterCriticalServiceAfter = TimeSpan.FromMinutes(1),  //错误时间超过1分钟，移除
                        Interval = TimeSpan.FromSeconds(30),                       //30秒检查一下
                        HTTP = new Uri(address, "HealthCheck").OriginalString
                    };
                    //设置Consul中心 配置
                    var registration = new AgentServiceRegistration()
                    {
                        Checks = new[] { httpCheck }, //配置健康检查
                        Address = address.Host,       //Consul 地址
                        Port = address.Port,          //Consul 端口
                        ID = serviceId,               //服务唯一ID
                        Name = serviceOptions.Value.ServiceName,   //对外服务名称

                    };
                    //向Consul 中心进行注册
                    consulClient.Agent.ServiceRegister(registration).GetAwaiter().GetResult();

                }
            });

            //在程序停止时,向Consul 中心进行注销
            applicationLifetime.ApplicationStopped.Register(() =>
            {
                foreach (var address in addresses)
                {
                    //设定服务Id(全局唯一 unique）
                    var serviceId = $"{serviceOptions.Value.ServiceName}_{address.Host}:{address.Port}";
                    consulClient.Agent.ServiceDeregister(serviceId).GetAwaiter().GetResult();
                }
            });

            #endregion

            app.UseMvc();
        }
    }
}
