using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using System;
using Microsoft.Extensions.DependencyInjection;
using zipkin4net;
using Microsoft.Extensions.Logging;
using zipkin4net.Transport.Http;
using zipkin4net.Tracers.Zipkin;
using zipkin4net.dotnetcore;
using zipkin4net.Utils;
using zipkin4net.Annotation;
using zipkin4net.Propagation;
using zipkin4net.Middleware;

namespace ZipkinExtensions
{
    /// <summary>
    /// ZipKin帮助类
    /// </summary>
    public static class ZipKinHelper
    {
        /// <summary>
        /// 启用zipkin中间件 
        /// </summary>
        /// <param name="app"></param>
        /// <param name="servierName">收集日志（服务名称）</param>
        /// <param name="zipkinUri">Zpikin 地址</param>
        /// <param name="loggerName">日志名称</param>
        /// <returns></returns>
        public static IApplicationBuilder UserZipKin(this IApplicationBuilder app,ILoggerFactory loggerFactory,
                    string servierName,string zipkinUri, string loggerName,float logpercent=1)
        {
            var applicationLife = app.ApplicationServices.GetService<IApplicationLifetime>();
            if (applicationLife == null) throw new ArgumentNullException(nameof(applicationLife));

            if (string.IsNullOrWhiteSpace(servierName)) throw new ArgumentNullException(nameof(servierName));
            if (string.IsNullOrWhiteSpace(zipkinUri)) throw new ArgumentNullException(nameof(zipkinUri));
            if (string.IsNullOrWhiteSpace(loggerName)) throw new ArgumentNullException(nameof(loggerName));

            //服务启动时候注入zipkin
            applicationLife.ApplicationStarted.Register(() =>
            {
                //收集日志比例
                TraceManager.SamplingRate = logpercent;

                var logger = new TracingLogger(loggerFactory, loggerName);

                //配置zipkin 服务器地址
                var httpSender = new HttpZipkinSender(zipkinUri, "application/json");

                //追踪器
                var tracer = new ZipkinTracer(httpSender, new JSONSpanSerializer(), new Statistics());

                //控制台追踪器
                var consoleTracer = new zipkin4net.Tracers.ConsoleTracer();
                TraceManager.RegisterTracer(tracer);
                TraceManager.RegisterTracer(consoleTracer);
                TraceManager.Start(logger);

            });

            applicationLife.ApplicationStopped.Register(() =>
            {
                TraceManager.Stop();
            });
            app.UseTracing(servierName);        
            return app;
        }
    }
}
