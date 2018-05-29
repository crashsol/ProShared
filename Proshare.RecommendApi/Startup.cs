using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ConsulExtensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.PlatformAbstractions;
using Proshare.RecommendApi.Data;
using Proshare.RecommendApi.IntergrationEventHandlers;
using Proshare.RecommendApi.Services;
using ProShare.RecommendApi.Infrastructure;
using ProShare.RecommendApi.Services;
using Resilience;
using Swashbuckle.AspNetCore.Swagger;
using ZipkinExtensions;

namespace Proshare.RecommendApi
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

            services.AddDbContext<RecommendDbContext>(options =>
            {
                options.UseMySQL(Configuration.GetConnectionString("MysqlRecommend"));
            });

            //清除默认的JwtToken默认的绑定
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(option =>
                    {
                        option.RequireHttpsMetadata = false;
                        option.Audience = "recommend_api"; //需要进行验证的 recommend
                        option.Authority = "http://localhost:5000";
                    });


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



            services.AddScoped<IUserService, UserService>()
                    .AddScoped<IContactService, ContactService>()
                    .AddTransient(typeof(ProjectCreatedIntergrationEventHandler));
            #endregion

            #region Consul服务依赖注入          

            //添加Consul服务注册
            services.AddConsulClient(Configuration.GetSection("ServiceDiscovery"))
                    .AddDnsClient();
            #endregion           

            #region CAP 依赖注入配置

            services.AddCap(option =>
            {
                // 如果你的 SqlServer 使用的 EF 进行数据操作，你需要添加如下配置：
                // 注意: 你不需要再次配置 x.UseSqlServer(""")
                option.UseEntityFramework<RecommendDbContext>();

                // 如果你使用的 RabbitMQ 作为MQ，你需要添加如下配置：
                option.UseRabbitMQ(Configuration.GetConnectionString("RabbitMQ"));

                //启用CAP ui
                option.UseDashboard();

                //向Consul 进行注册 register
                option.UseDiscovery(d =>
                {
                    d.DiscoveryServerHostName = "localhost";
                    d.DiscoveryServerPort = 8500;
                    d.CurrentNodeHostName = "localhost";
                    d.CurrentNodePort = 5005;
                    d.NodeId = 5;
                    d.NodeName = "CAP Recommend API Node";

                });

            });


            #endregion

            #region 添加SwaggerUI

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("RecommendApi", new Info { Title = "Recommend Api接口", Version = "v1" });
                var basePath = PlatformServices.Default.Application.ApplicationBasePath;
                var xmlPath = Path.Combine(basePath, "Proshare.RecommendApi.xml");
                options.IncludeXmlComments(xmlPath);
            });


            #endregion


            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env,ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UserZipKin(loggerFactory, "Recommend.Api", "http://www.crashcore.cn:9411", "zipkinlogger", 1);
            app.UseAuthentication();
            app.UseCap();
            app.UseConsul();

            app.UseSwagger(c => {
                c.RouteTemplate = "{documentName}/swagger.json";
            });
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "RecommendApi V1");
            });
            app.UseMvc();
        }
    }
}
