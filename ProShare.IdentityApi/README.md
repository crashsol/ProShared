1、在处理微服务内部各服务之间的调用，熔断、异常处理可以是用polly实现。
 【强烈推荐】美团点评技术文章：服务容错模式： 
  https://tech.meituan.com/service-fault-tolerant-pattern.html

  #微服务之间的服务发现    
  IdentityApi  ==》请求 Consul，发现UseApi服务地址  ==》进行用户创建或则验证，实现微服务之间的服务发现
  
  1、UserApi须要向Consul进行服务注册
  2、IdentityApi通过配置文件,使用DnsClient向Consul进行服务查询，获得UserApi服务地址
	 并结合本地设置，组成完整的请求地址
  3、IdentityApi进行服务调用，完成服务之间的调用