using ProShare.IdentityApi.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProShare.IdentityApi.Models.Dto
{
    /// <summary>
    /// 服务发现配置类
    /// </summary>
    public class ServiceDiscoveryOptions
    {
        /// <summary>
        /// 注册的服务名称
        /// </summary>
        public string ServiceName { get; set; }

        /// <summary>
        /// 需要发现的服务名称
        /// </summary>
        public string DisConverServiceName { get; set; }

        /// <summary>
        /// Consul 节点信息
        /// </summary>
        public ConsulOptions Consul { get; set; }
    }
}
