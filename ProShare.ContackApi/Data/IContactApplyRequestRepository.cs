﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProShare.ContactApi.Models;

namespace ProShare.ContactApi.Data
{
    public interface IContactApplyRequestRepository
    {

        /// <summary>
        /// 添加好友添加申请请求
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<bool> AddRequest(ContactApplyRequest request);

        /// <summary>
        /// 通过 好友添加申请
        /// </summary>
        /// <param name="applierId"></param>
        /// <returns></returns>
        Task<bool> Approval(int applierId);

        /// <summary>
        /// 获取所有好友添加申请
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        Task<bool> GetRequestList(int userId);
    }
}
