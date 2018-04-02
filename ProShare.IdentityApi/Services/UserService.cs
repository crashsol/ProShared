using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using DnsClient;
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

        //服务请求地址
        private readonly string QueryAction = "/api/users/get-or-create";

        public UserService(IHttpClient httpClient,IDnsQuery dnsQuery,IOptions<ServiceDiscoveryOptions> option)
        {
            _httpClient = httpClient;         
            _dnsQuery = dnsQuery ?? throw new ArgumentNullException(nameof(dnsQuery));
            _serviceOption = option.Value ?? throw new ArgumentNullException(nameof(option));

        }

      

        public async Task<int> GetOrCreateAsync(string phone)
        {
            var query = new Dictionary<string, string> { { "phone", phone } };
            var queryContent = new FormUrlEncodedContent(query);
            var queryUrl = await GetApplicateUrlFromConsulAsync();
            var response = await _httpClient.PostAsync(queryUrl, queryContent);
            if (response.IsSuccessStatusCode)
            {
                var userId = await response.Content.ReadAsStringAsync();
                int.TryParse(userId, out int id);
                return id;

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
