using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ProShare.UserApi.Data;
using Microsoft.EntityFrameworkCore;
using ProShare.UserApi.Models;
using ProShare.UserApi.Models.Dtos;
using Consul;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http.Features;
using Infrastructure.Filter;
using System.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace ProShare.UserApi
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
            services.AddDbContext<UserContext>(options =>
            {
                options.UseMySQL(Configuration.GetConnectionString("MysqlUser"));
            });

            //清除默认的JwtToken默认的绑定
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(option =>
                    {
                        option.RequireHttpsMetadata = false;
                        option.Audience = "user_api"; //需要进行验证的 ApiResource
                        option.Authority = "http://localhost:5000"; 
                    });

            //绑定配置文件
            services.Configure<ServiceDiscoveryOptions>(Configuration.GetSection("ServiceDiscovery"));

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

            services.AddMvc(option =>
            {
                option.Filters.Add(typeof(GlobalExceptionFilter));//添加全局异常处理
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

            app.UseAuthentication();

            app.UseMvc();
            InitDataBase(app);
        }

        public void InitDataBase(IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var userDbContext = scope.ServiceProvider.GetService<UserContext>();

                userDbContext.Database.Migrate();
                if (!userDbContext.Users.Any())
                {
                    userDbContext.Users.Add(new AppUser { Name = "Crashsol", Title = "经理" });
                    userDbContext.SaveChanges();
                }

            }

        }
    }
}
