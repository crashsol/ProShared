using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Project.API.Application.Queries
{
    public interface IProjectQueries
    {
        /// <summary>
        /// 获取项目列表
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        Task<dynamic> GetProjectsByUserIdAsync(int userId);

        /// <summary>
        /// 获取项目明细
        /// </summary>      
        /// <param name="projectId">项目Id</param>
        /// <returns></returns>
        Task<dynamic> GetProjectDetailAsync(int projectId);
    }
}
