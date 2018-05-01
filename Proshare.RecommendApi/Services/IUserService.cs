using ProShare.RecommendApi.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProShare.RecommendApi.Services
{
    public interface IUserService
    {
        /// <summary>
        /// 获取用户基本信息
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        Task<UserIdentity> GetBaseUserInfoAsync(int userId);
      
    }
}
