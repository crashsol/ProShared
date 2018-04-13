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
using ConsulExtensions;
using ConsulExtensions.Dtos;

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

            //添加Consul服务注册
            services.AddConsulClient();          

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


            //启用Consul 注册和发现
            app.UseConsul(env, applicationLifetime, consulClient, serviceOptions);           

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
