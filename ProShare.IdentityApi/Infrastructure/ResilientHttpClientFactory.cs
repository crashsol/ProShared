using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Polly;
using Resilience;

namespace ProShare.IdentityApi.Infrastructure
{
    public class ResilientHttpClientFactory : IResilientHttpClientFactory
    {

        private readonly ILogger<ResilientHttpClient> _logger;
        //设置重试的次数
        private readonly int _retryCount;
        //失败几次后断路器打开
        private readonly int _exceptionsAllowedBeforeBreaking;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ResilientHttpClientFactory(ILogger<ResilientHttpClient> logger, IHttpContextAccessor httpContextAccessor, int exceptionsAllowedBeforeBreaking = 5, int retryCount = 6)
        {
            _logger = logger;
            _exceptionsAllowedBeforeBreaking = exceptionsAllowedBeforeBreaking;
            _retryCount = retryCount;
            _httpContextAccessor = httpContextAccessor;
        }

        public ResilientHttpClient CreateResilientHttpClient()
                    => new ResilientHttpClient((origin) => CreatePolicies(origin), _logger, _httpContextAccessor);


        //origin  创建 Policy 策略，这个可以根据origin 设定不同的Policy组
        private Policy[] CreatePolicies(string origin)
          => new Policy[]
          {
                //处理HttpRequestException
                Policy.Handle<HttpRequestException>()
                        .WaitAndRetry(
                             _retryCount, //重试次数
                              retryAttempt => TimeSpan.FromSeconds(Math.Pow(2,retryAttempt)),//每次重试时间是2次方
                       (exception,timespan,retryCount,context) =>{
                           //重试期间进行日志记录
                           //exception 异常信息
                           //timespan 时间间隔
                           //retryCount 当前重试次数
                           //content 类容
                        var msg = $"第 {retryCount} 几次进行错误重试 " +
                                      $"of {context.PolicyKey} " +
                                      $"at {context.ExecutionKey}, " +
                                      $"due to: {exception}.";
                                     _logger.LogWarning(msg);
                                     _logger.LogDebug(msg);

                       }),
                Policy.Handle<HttpRequestException>()
                        .CircuitBreakerAsync(
                            _exceptionsAllowedBeforeBreaking, //失败几次后断路器打开
                            TimeSpan.FromMinutes(1),          //断路器打开时长
                            //断路器打开时执行
                            (exception,duration) =>
                            {
                                //断路器打开进行日志记录
                                _logger.LogTrace("断路器打开");
                            },
                            //断路器关闭
                            ()=>{

                                _logger.LogTrace("断路器关闭");
                            })

          };

    }
}
