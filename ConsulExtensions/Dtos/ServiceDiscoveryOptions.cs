using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConsulExtensions.Dtos
{
    /// <summary>
    /// 服务发现配置类
    /// </summary>
    public class ServiceDiscoveryOptions
    {
        public ServiceDiscoveryOptions()
        {
            DisConveServiceNames = new List<string>();
        }

        /// <summary>
        /// 注册的服务名称
        /// </summary>
        public string ServiceName { get; set; }

        public List<string> DisConveServiceNames { get; set; }

        /// <summary>
        /// Consul 节点信息
        /// </summary>
        public ConsulOptions Consul { get; set; }
    }
}
