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
using MediatR;
using ConsulExtensions;
using Infrastructure.Filter;
using Microsoft.EntityFrameworkCore;
using Project.Infrastructure;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Reflection;
using Project.API.Application.Queries;
using Project.API.Application.Services;
using Project.Domain.AggregatesModel;
using Project.Infrastructure.Repositorys;

namespace Project.API
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
            //添加DbContext依赖注入          
            services.AddDbContext<ProjectContext>(option =>
            {
                option.UseMySQL(Configuration.GetConnectionString("MysqlProject"),sql => {
                    sql.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name);
                });
            });

            //添加Authentication配置
            //清除默认的JwtToken默认的绑定
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(option =>
                    {
                        option.RequireHttpsMetadata = false;
                        option.Audience = "project_api"; //需要进行验证的 ApiResource
                        option.Authority = "http://localhost:5000";
                    });

            //添加Consul服务注入
            services.AddConsulClient(Configuration.GetSection("ServiceDiscovery"))
                    .AddDnsClient();

            //添加服务依赖注入
            services.AddScoped<IProjectQueries, ProjectQueries>()
                    .AddScoped<IProjectRepository,ProjectRepository>()
                    .AddScoped<IRecommendService, TestRecommendService>();

            //添加MeditatR注入
            services.AddMediatR(typeof(Startup).GetTypeInfo().Assembly);
            services.AddMvc(option =>
            {
                option.Filters.Add(typeof(GlobalExceptionFilter));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            //启用服务注册
            app.UseConsul();

            //启用Authentication
            app.UseAuthentication();

            app.UseMvc();
        }
    }
}
