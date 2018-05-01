using Resilience;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProShare.RecommendApi.Infrastructure
{
    /// <summary>
    /// ResilientHttp创建工厂
    /// </summary>
    public interface IResilientHttpClientFactory
    {
        /// <summary>
        /// 创建 ResilientHttpClient 实例
        /// </summary>
        /// <returns></returns>
        ResilientHttpClient CreateResilientHttpClient();

    }
}
