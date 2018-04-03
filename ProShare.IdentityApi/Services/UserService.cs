using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using DnsClient;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ProShare.IdentityApi.Infrastructure;
using ProShare.IdentityApi.Models.Dtos;
using Resilience;

namespace ProShare.IdentityApi.Services
{
    public class UserService : IUserService
    {
        private readonly IHttpClient _httpClient;    

        private readonly IDnsQuery _dnsQuery;

        private readonly ServiceDiscoveryOptions _serviceOption;

        private readonly ILogger<UserService> _ilogger;

        //服务请求地址
        private readonly string QueryAction = "/api/users/get-or-create";

        public UserService(IHttpClient httpClient,IDnsQuery dnsQuery,IOptions<ServiceDiscoveryOptions> option, ILogger<UserService> logger )
        {
            _httpClient = httpClient;         
            _dnsQuery = dnsQuery ?? throw new ArgumentNullException(nameof(dnsQuery));
            _serviceOption = option.Value ?? throw new ArgumentNullException(nameof(option));
            _ilogger = logger;

        }

      

        public async Task<int> GetOrCreateAsync(string phone)
        {
            var form = new Dictionary<string, string> { { "phone", phone } };

            try
            {
                var queryUrl = await GetApplicateUrlFromConsulAsync();
                var response = await _httpClient.PostAsync(queryUrl, form);
                if (response.IsSuccessStatusCode)
                {
                    var userId = await response.Content.ReadAsStringAsync();
                    int.TryParse(userId, out int id);
                    return id;

                }
            }
            catch (Exception ex)
            {
                _ilogger.LogError("GetOrCreateAsync 在重试后调用失败", ex.Message + ex.StackTrace);
                throw;
            }

          
            return 0;
        }

        /// <summary>
        /// 从Consul发现用户服务地址
        /// </summary>
        /// <returns></returns>
        private async Task<string> GetApplicateUrlFromConsulAsync()
        {
            var appUrl = "";
            var result = await _dnsQuery.ResolveServiceAsync("service.consul", _serviceOption.ServiceName);
            var addressList = result.First().AddressList;
            var address = addressList.Any() ? addressList.First().ToString() : result.First().HostName;
            var port = result.First().Port;        
            appUrl = $"http://{address}:{port}{QueryAction}";
            return appUrl;
        }
    }
}
