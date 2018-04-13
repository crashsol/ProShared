using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Consul;
using DnsClient;
using Infrastructure.Filter;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ProShare.ContactApi.Infrastructure;
using ProShare.ContactApi.Models.Dtos;
using ProShare.ContactApi.Services;
using Resilience;
using ProShare.ContactApi.Data;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.IdentityModel.Tokens.Jwt;
using ConsulExtensions;
using ConsulExtensions.Dtos;

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

                    
            //清除默认的JwtToken默认的绑定
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(option =>
                {
                    option.RequireHttpsMetadata = false;
                    option.Audience = "contact_api"; //需要进行验证的ApiResourece
                    option.Authority = "http://localhost:5000";            //验证的IdentityServer 地址
                });


            //加载MongoDb配置
            services.Configure<AppSetting>(Configuration);

            ///添加服务发现  进行配置绑定       
            services.Configure<ServiceDiscoveryOptions>(Configuration.GetSection("ServiceDiscovery"));
            services.AddConsulClient();
            services.AddSingletonDnsClient();
          

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

            //服务注入
            services.AddScoped(typeof(ContactContext));
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IContactApplyRequestRepository, MongoContactApplyRequestRepository>();
            services.AddScoped<IContactBookRepository, MongoContactBookRepository>();
        

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
            app.UseConsul(env, applicationLifetime, consulClient, serviceOptions);

            //启用认证框架，以便将header中的Token进行转换到User中
            app.UseAuthentication();

         

            app.UseMvc();
        }
    }
}
