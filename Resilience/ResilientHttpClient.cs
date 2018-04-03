using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Polly;
using Polly.Wrap;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Resilience
{
    /// <summary>
    /// 使用Polly进行封装,是代理类具有错误重试、断路器、仓壁隔离等特性
    /// </summary>
    public class ResilientHttpClient : IHttpClient
    {
        /// <summary>
        /// Http代理类
        /// </summary>
        private readonly HttpClient _client;

        private readonly ILogger<ResilientHttpClient> _logger;

        /// <summary>
        /// Polly 条件创建
        /// </summary>
        private readonly Func<string, IEnumerable<Policy>> _policyCreator;

        /// <summary>
        /// Polly包装（ConcurrentDictionary线程并发安全）
        /// </summary>
        private ConcurrentDictionary<string, PolicyWrap> _policyWrappers;

        private readonly IHttpContextAccessor _httpContextAccessor;

        public ResilientHttpClient(Func<string,IEnumerable<Policy>> policyCreator,ILogger<ResilientHttpClient> logger, IHttpContextAccessor httpContextAccessor)
        {
            //直接创建一个
            _client = new HttpClient(); 

            //创建一个Policy包装器
            _policyWrappers = new ConcurrentDictionary<string, PolicyWrap>();

            _logger = logger;
            _policyCreator = policyCreator;
            _httpContextAccessor = httpContextAccessor;

        }
        public Task<HttpResponseMessage> PostAsync<T>(string uri, T item, string authorizationToken = null, string requestId = null, string authorizationMethod = "Bearer")
        {
            Func<HttpRequestMessage> func = () => CreateHttpRequestMessage(HttpMethod.Post, uri, item);
            return DoPostPutAsync(HttpMethod.Post, uri, func, authorizationToken, requestId, authorizationMethod);
        }

        public Task<HttpResponseMessage> PostAsync(string uri, Dictionary<string,string> form, string authorizationToken = null, string requestId = null, string authorizationMethod = "Bearer")
        {
            Func<HttpRequestMessage> func = ()=>  CreateHttpRequestMessage(HttpMethod.Post, uri, form);
            
            return DoPostPutAsync(HttpMethod.Post, uri, func, authorizationToken, requestId, authorizationMethod);
        }


        /// <summary>
        /// 创建HttpRequestMessage  application/json 请求
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="method">方法类型</param>
        /// <param name="url">链接地址</param>
        /// <param name="item">要传递的内容</param>
        /// <returns></returns>
        private HttpRequestMessage CreateHttpRequestMessage<T>(HttpMethod method,string url,T item)
        {
            var httpRequestMessage = new HttpRequestMessage(method, url)
            {
                Content = new StringContent(JsonConvert.SerializeObject(item), System.Text.Encoding.UTF8, "application/json")
            };
            return httpRequestMessage;
        }

        /// <summary>
        /// 创建HttpRequestMessage FormUrlEncodedContent 请求
        /// </summary>   
        /// <param name="method">方法类型</param>
        /// <param name="url">链接地址</param>
        /// <param name="form">要传递的内容</param>
        private HttpRequestMessage CreateHttpRequestMessage(HttpMethod method, string url, Dictionary<string,string> form)
        {
            var httpRequestMessage = new HttpRequestMessage(method, url)
            {
                Content = new FormUrlEncodedContent(form)
            };
            return httpRequestMessage;
        }


        private Task<HttpResponseMessage> DoPostPutAsync(HttpMethod method, string uri,Func<HttpRequestMessage> requestFunc, string authorizationToken = null, string requestId = null, string authorizationMethod = "Bearer")
        {
            if (method != HttpMethod.Post && method != HttpMethod.Put)
            {
                throw new ArgumentException("调用方法类型不正确.", nameof(method));            }

            var origin = GetOriginFromUri(uri);

            //根据名称origin名称，获得PoliyWary，并返回
            return HttpInvoker(origin, async () =>            {

                //构建请求
                var requestMessage = requestFunc();

                //设置请求Header
                SetAuthorizationHeader(requestMessage);

                //设置请求body,默认使用 application/json
               

                if (authorizationToken != null)
                {
                    requestMessage.Headers.Authorization = new AuthenticationHeaderValue(authorizationMethod, authorizationToken);
                }

                if (requestId != null)
                {
                    requestMessage.Headers.Add("x-requestid", requestId);
                }
                //发送请求
                var response = await _client.SendAsync(requestMessage);

                //如果请求返回时500，抛出HttpRequestException 
                if (response.StatusCode == HttpStatusCode.InternalServerError)
                {
                    throw new HttpRequestException();
                }

                return response;
            });
        }

        #region 私有方法


        /// <summary>
        /// 根据origin名称，获得指定的policywrap
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="origin"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        private async Task<T> HttpInvoker<T>(string origin, Func<Task<T>> action)
        {
            var normalizedOrigin = NormalizeOrigin(origin);

            if (!_policyWrappers.TryGetValue(normalizedOrigin, out PolicyWrap policyWrap))
            {
                policyWrap = Policy.WrapAsync(_policyCreator(normalizedOrigin).ToArray());
                _policyWrappers.TryAdd(normalizedOrigin, policyWrap);
            }

            // Executes the action applying all 
            // the policies defined in the wrapper
            return await policyWrap.ExecuteAsync(action, new Context(normalizedOrigin));
        }

        /// <summary>
        /// 装换origin 名称
        /// </summary>
        /// <param name="origin"></param>
        /// <returns></returns>
        private static string NormalizeOrigin(string origin)
        {
            return origin?.Trim()?.ToLower();
        }

        /// <summary>
        /// 根据Url来设定origin名称，以便确定使用对应的policy的策略
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        private static string GetOriginFromUri(string uri)
        {
            var url = new Uri(uri);

            var origin = $"{url.Scheme}://{url.DnsSafeHost}:{url.Port}";

            return origin;
        }

        /// <summary>
        /// 设置请求头
        /// </summary>
        /// <param name="requestMessage"></param>
        private void SetAuthorizationHeader(HttpRequestMessage requestMessage)
        {
            var authorizationHeader = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];
            if (!string.IsNullOrEmpty(authorizationHeader))
            {
                requestMessage.Headers.Add("Authorization", new List<string>() { authorizationHeader });
            }
        }
        #endregion
    }
}
