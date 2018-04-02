using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Resilience
{
    /// <summary>
    /// 自定义HttpClient代理类
    /// </summary>
    public interface IHttpClient
    {
      
        Task<HttpResponseMessage> PostAsync<T>(string uri, T item, string authorizationToken = null, string requestId = null, string authorizationMethod = "Bearer");


        //Task<string> GetStringAsync(string uri, string authorizationToken = null, string authorizationMethod = "Bearer");


        //Task<HttpResponseMessage> DeleteAsync(string uri, string authorizationToken = null, string requestId = null, string authorizationMethod = "Bearer");

        //Task<HttpResponseMessage> PutAsync<T>(string uri, T item, string authorizationToken = null, string requestId = null, string authorizationMethod = "Bearer");
    }
}
