using Proshare.RecommendApi.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Proshare.RecommendApi.Services
{
    public interface IContactService
    {
        /// <summary>
        /// 获取好友列表
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<List<Contact>> GetContactsByUserIdAsync(int userId);
    }
}
