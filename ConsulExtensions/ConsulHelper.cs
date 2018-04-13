using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using ConsulExtensions.Dtos;
using Microsoft.Extensions.Options;
using Consul;
using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Hosting.Server.Features;
using System.Linq;
using DnsClient;

namespace ConsulExtensions
{
    public static class ConsulHelper
    {
        /// <summary>
        /// 向服务中注入ConsulClient
        /// </summary>
        /// <param name="services"></param>
        /// <param name="Configuration"></param>
        public static void AddConsulClient(this IServiceCollection services)
        {
            //services.Configure<ServiceDiscoveryOptions>(Configuration.GetSection("ServiceDiscovery"));
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
        }


        /// <summary>
        /// 添加DnsClient 服务依赖
        /// </summary>
        /// <param name="services"></param>
        public static void AddSingletonDnsClient(this IServiceCollection services)
        {
            services.AddSingleton<IDnsQuery>(b =>
            {
                var serviceOption = b.GetRequiredService<IOptions<ServiceDiscoveryOptions>>();
                //添加Consul服务地址
                return new LookupClient(serviceOption.Value.Consul.DnsEndpoint.ToIPEndPoint());
            });
        }

        /// <summary>
        /// 启用Consul 服务注册与发现
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        /// <param name="applicationLifetime"></param>
        /// <param name="consulClient"></param>
        /// <param name="serviceOptions"></param>
        public static void UseConsul(this IApplicationBuilder app, IHostingEnvironment env,
                IApplicationLifetime applicationLifetime,
                IConsulClient consulClient,
                IOptions<ServiceDiscoveryOptions> serviceOptions)
        {

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
        }
    }
}
