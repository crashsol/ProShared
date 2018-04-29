using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.API.Application.Services
{
    /// <summary>
    /// 检查项目是否在推荐列表
    /// </summary>
    public interface IRecommendService
    {
        Task<bool> IsProjectInRecommend(int userId,int projectId);
    }
}
