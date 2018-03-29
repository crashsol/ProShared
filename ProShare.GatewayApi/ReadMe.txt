{
  "ReRoutes": [
****************************************************************************
   {
      "DownstreamPathTemplate": "/api/users",        //配置下游访问 userapi 的路由
      "DownstreamScheme": "http",                     //配置下游Scheme Http 或则 Https
      "DownstreamHostAndPorts": [             
        {
          "Host": "localhost",                        //下游地址 userApi 地址
          "Port": 5003                                //下游端口5003
        }
      ],
      "UpstreamPathTemplate": "/users",               //上游访问地址
      "UpstreamHttpMethod": [ "Get" ],                //访问方法类型
	   "AuthenticationOptions": {
        "AuthenticationProviderKey": "ProShare",     //需要认证的KEY
        "AllowedScopes": []
      }
    }                
	这条路由表示， 用户只能通过访问  Ocelot暴露的 http://localhost/users 地址
	Ocelot 会根据路由配置信息 将请求封装后转发到 真实的 UserApi http://localhost:5003/api/users 这个地址，并将结果原路返回
*************************************************************************************
  ],
  "GlobalConfiguration": {
    "BaseUrl": "http://localhost"                //网关所在地址 一般是使用https 例如本项目启动在80端口，直接填写localhost
  }
}