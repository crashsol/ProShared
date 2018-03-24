using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProShare.IdentityApi.Services
{
    public class TestAuthCodeService : IAuthCodeService
    {
        public async Task<bool> Validate(string phone, string authCode)
        {
            //TBD 手机号的验证
            //测试环境默认返回 true
            return await Task.FromResult(true);
        }
    }
}
