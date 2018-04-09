using ProShare.ContactApi.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

namespace ProShare.ContactApi.Data
{
    public interface IContactBookRepository
    {
        /// <summary>
        /// 更新用户信息
        /// </summary>
        /// <param name="baseUserInfo">要更新的用户信息</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<bool> UpdateUserInfoAsync(BaseUserInfo baseUserInfo, CancellationToken cancellationToken);
    }
}
