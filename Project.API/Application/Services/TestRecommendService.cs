using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.API.Application.Services
{
    public class TestRecommendService : IRecommendService
    {
        public Task<bool> IsProjectInRecommend(int userId, int projectId)
        {
            return Task.FromResult(true);
        }
    }
}
