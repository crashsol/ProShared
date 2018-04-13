using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProShare.IdentityApi.Services
{
    public class UserProfileService : IProfileService
    {

        private readonly ILogger<UserProfileService> _logger;

        public UserProfileService(ILogger<UserProfileService> logger)
        {
            _logger = logger;
        }

        public Task GetProfileDataAsync(ProfileDataRequestContext context)
        {

            //根据用户唯一标识查找用户信息
            var userId = context.Subject.GetSubjectId();
            if (userId != null)
            {
                //将我们给我用赋予的Claims 附加到IssuedClaims, 便能在token中正常获取到所需Claim
                context.IssuedClaims.AddRange(context.Subject.Claims);
            }
            //打印Claims
            context.LogIssuedClaims(_logger);
            return Task.CompletedTask;
        }

        public Task IsActiveAsync(IsActiveContext context)
        {
            _logger.LogDebug("IsActive called from: {caller}", context.Caller);
 
            //检查subId是否为空
            var userId = context.Subject.GetSubjectId();
            if (userId != null)
            {
                context.IsActive = true;
            }

            return Task.CompletedTask;
        }
    }
}
