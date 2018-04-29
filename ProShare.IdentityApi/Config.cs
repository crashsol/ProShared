using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ProShare.IdentityApi
{

    public class Config
    {
        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>{
                new ApiResource("gateway_api","gateway service"),
                new ApiResource("user_api", "user service"),
                new ApiResource("contact_api","contact service"),
                new ApiResource("project_api","project service")
                
            };
        }

        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>
            {              
                //测试账号
                new Client{
                    ClientId ="android",
                    ClientSecrets  = new List<Secret> { new Secret("secret".Sha256()) },
                    RefreshTokenExpiration = TokenExpiration.Sliding,  //滑动过期
                   AllowOfflineAccess = true,
                   RequireClientSecret =false,
                   AllowedGrantTypes = new List<string>{ "sms_auth_code" },//授权模式，设定为我们自定义的
                   AlwaysIncludeUserClaimsInIdToken  =true ,//是否发送user 的Claims 信息

                   //允许访问的 Scope
                   AllowedScopes = new List<string>
                   {
                       "gateway_api",
                       "user_api",
                       "contact_api",
                       "project_api",
                       IdentityServerConstants.StandardScopes.OpenId,
                       IdentityServerConstants.StandardScopes.Profile,
                       IdentityServerConstants.StandardScopes.OfflineAccess
                   }



                }
            };
        }
        /// <summary>
        /// 添加测试 用户
        /// </summary>
        /// <returns></returns>
        public static List<TestUser> GetTestUsers()
        {
            return new List<TestUser>
            {
                ///password 登录用户测试
                new TestUser
                {
                    SubjectId="pwdClient",
                    Password = "123",
                    Username ="123"
                },


                // MVC 登录测试用户
                new TestUser
                {
                    SubjectId="10000",
                    Password = "admin",
                    Username ="admin",
                    Claims = new Claim[]{
                        new Claim("permission","home.read"),                //无法返回
                        new Claim("permission","home.write"),                //无法返回
                        new Claim("permission","home.delete"),                //无法返回
                        new Claim("role","Admin"),                  //无法返回
                        new Claim("website", "https://bob.com")
                    }
                }


            };
        }

        /// <summary>
        /// 添加 Identity Resource
        /// </summary>
        /// <returns></returns>
        public static List<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {               
                 new IdentityResources.OpenId(),
                 new IdentityResources.Profile()
               
            };

        }
    }

}
