using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Consul;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

namespace ProShare.GatewayApi
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
           
            //添加Ocelot 依赖注入,
            services.AddOcelot()
                .AddStoreOcelotConfigurationInConsul() //向Consul KV中心缓存配置信息
                .AddAdministration("/Administrator",options => {
                    //配置IdentityServer
                    options.Authority = "http://localhost:5001";            
                    options.SupportedTokens = SupportedTokens.Both;
                    options.ApiName = "gateway_api";
                    options.ApiSecret = "secret";
                    options.RequireHttpsMetadata = false; //是否启用Https     
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //启动Ocelot管道
            app.UseOcelot().Wait();
        }
    }
}
