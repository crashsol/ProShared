using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ProShare.IdentityApi.Services
{
    public class UserService : IUserService
    {
        private readonly HttpClient _httpClient;

        private readonly string BaseUrl = "http://localhost:5003";

        public UserService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<int> GetOrCreateAsync(string phone)
        {

            var query = new Dictionary<string, string> { { "phone", phone } };
            var queryContent = new FormUrlEncodedContent(query);
            var response = await _httpClient.PostAsync(BaseUrl + "/api/users/get-or-create", queryContent);
            if (response.IsSuccessStatusCode)
            {
                var userId = await response.Content.ReadAsStringAsync();
                int.TryParse(userId, out int id);
                return id;

            }
            return 0;


        }
    }
}
