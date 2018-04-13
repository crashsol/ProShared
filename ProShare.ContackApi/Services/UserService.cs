using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DnsClient;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using ProShare.ContactApi.Models.Dtos;
using Resilience;
using Newtonsoft;
using Newtonsoft.Json;

namespace ProShare.ContactApi.Services
{
    public class UserService : IUserService
    {

        private readonly IHttpClient _httpClient;

        private readonly IDnsQuery _dnsQuery;

        private readonly ServiceDiscoveryOptions _serviceOption;

        private readonly ILogger<UserService> _ilogger;

        //服务请求地址
        private readonly string QueryAction = "/api/users/get-userinfo/";

        public UserService(IHttpClient httpClient, IDnsQuery dnsQuery, IOptions<ServiceDiscoveryOptions> option, ILogger<UserService> logger)
        {
            _httpClient = httpClient;
            _dnsQuery = dnsQuery ?? throw new ArgumentNullException(nameof(dnsQuery));
            _serviceOption = option.Value ?? throw new ArgumentNullException(nameof(option));
            _ilogger = logger;

        }

        /// <summary>
        /// 从Consul发现用户服务地址
        /// </summary>
        /// <returns></returns>
        private async Task<string> GetApplicateUrlFromConsulAsync()
        {
            try
            {
                var policyRetry = Policy.Handle<InvalidOperationException>()
                      .WaitAndRetryAsync(
                      3,
                      retryTimespan => TimeSpan.FromSeconds(Math.Pow(2, retryTimespan)),
                      (exception, timespan, retryCount, context) =>
                      {
                          //重试期间进行日志记录
                          //exception 异常信息
                          //timespan 时间间隔
                          //retryCount 当前重试次数
                          //content 类容
                          var msg = $"第 {retryCount} 次进行错误重试 " +
                                          $"of {context.PolicyKey} " +
                                          $"at {context.ExecutionKey}, " +
                                          $"due to: {exception}.";
                          _ilogger.LogWarning(msg);
                          _ilogger.LogDebug(msg);

                      });
                var policyBreak = Policy.Handle<InvalidOperationException>()
                                    .CircuitBreakerAsync(2, TimeSpan.FromMinutes(1),
                                    (exctption, timespan) =>
                                    {

                                        _ilogger.LogTrace("断路器打开");
                                    },
                                    () =>
                                    {
                                        _ilogger.LogTrace("断路器关闭");
                                    });

                var policyWary = Policy.WrapAsync(policyRetry, policyBreak);


                var appUrl1 = await policyWary.ExecuteAsync(async () =>
                {
                    var result = await _dnsQuery.ResolveServiceAsync("service.consul", _serviceOption.DisConverServiceName);
                    var addressList = result.First().AddressList;
                    var address = addressList.Any() ? addressList.First().ToString() : result.First().HostName;
                    var port = result.First().Port;
                    var appUrl = $"http://{address}:{port}{QueryAction}";
                    return appUrl;
                });

                return appUrl1;

            }
            catch (Exception ex)
            {
                _ilogger.LogError($"从Consul发现UserApi地址,在重试3次后失败" + ex.Message + ex.StackTrace);
                return "";
            }


        }
        public async Task<BaseUserInfo> GetBaseUserInfoAsync(int userId)
        {
            var userpaiAddress = await GetApplicateUrlFromConsulAsync();

            var result =await _httpClient.GetStringAsync(userpaiAddress +  userId);

            return JsonConvert.DeserializeObject<BaseUserInfo>(result);

        }
    }
}
