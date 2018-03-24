using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProShare.IdentityApi.Services
{
    public  interface IUserService
    {
        /// <summary>
        /// 检查手机号是否已经注册，如果没有注册则创建用户
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        Task<int> GetOrCreate(string phone);
    }
}
