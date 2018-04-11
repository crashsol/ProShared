using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProShare.ContactApi.Models;
using System.Threading;

namespace ProShare.ContactApi.Data
{
    /// <summary>
    /// 好友申请仓储
    /// </summary>
    public interface IContactApplyRequestRepository
    {

        /// <summary>
        /// 添加好友申请请求
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<bool> AddRequestAsync(ContactApplyRequest request, CancellationToken cancellationToken);

        /// <summary>
        /// 通过好友申请
        /// </summary>
        /// <param name="userId">操作者ID</param>
        /// <param name="applierId">申请人ID</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<bool> ApprovalAsync(int userId, int applierId, CancellationToken cancellationToken);

        /// <summary>
        /// 获取好友添加申请
        /// </summary>
        /// <param name="userId">当前登录用户ID</param>
        /// <returns></returns>
        Task<List<ContactApplyRequest>> GetRequestList(int userId, CancellationToken cancellationToken);
    }
}
