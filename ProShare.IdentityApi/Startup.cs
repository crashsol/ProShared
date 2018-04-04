using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ProShare.IdentityApi.Services;
using IdentityServer4;
using ProShare.IdentityApi.Authentication;
using ProShare.IdentityApi.Models.Dtos;
using DnsClient;
using ProShare.IdentityApi.Infrastructure;
using Microsoft.AspNetCore.Http;
using Resilience;

namespace ProShare.IdentityApi
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
            services.AddIdentityServer()
                .AddExtensionGrantValidator<SmsAuthCodeValidator>() //添加自己实行的认证方式
                .AddDeveloperSigningCredential()                               
                .AddInMemoryApiResources(Config.GetApiResources())
                .AddInMemoryClients(Config.GetClients())
                .AddInMemoryIdentityResources(Config.GetIdentityResources());


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



            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAuthCodeService, TestAuthCodeService>();
            
            //添加服务发现
            //进行配置绑定       
            services.Configure<ServiceDiscoveryOptions>(Configuration.GetSection("ServiceDiscovery"));          
            services.AddSingleton<IDnsQuery>(b =>            
            {
                var serviceOption = b.GetRequiredService<IOptions<ServiceDiscoveryOptions>>();
                //添加Consul服务地址
                return new  LookupClient(serviceOption.Value.Consul.DnsEndpoint.ToIPEndPoint());
            });

            services.AddMvc();
          
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseIdentityServer();

            app.UseMvc();
        }
    }
}
