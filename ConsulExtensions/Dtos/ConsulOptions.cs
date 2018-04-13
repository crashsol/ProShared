using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConsulExtensions.Dtos
{
    /// <summary>
    /// Consul 配置设置
    /// </summary>
    public class ConsulOptions
    {
        /// <summary>
        /// Consul 服务地址
        /// </summary>
        public string HttpEndpoint { get; set; }

        /// <summary>
        /// Dns 目的地址
        /// </summary>
        public DnsEndpoint DnsEndpoint { get; set; }
    }
}
