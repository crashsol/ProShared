using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Validation;
using IdentityServer4.Models;
using ProShare.IdentityApi.Services;

namespace ProShare.IdentityApi.Authentication
{
    /// <summary>
    /// 自定义扩展IdentityService4 授权模式
    /// </summary>
    public class SmsAuthCodeValidator : IExtensionGrantValidator
    {
        /// <summary>
        /// 定义授权验证名称
        /// </summary>
        public string GrantType =>"sms_auth_code";

        private readonly IAuthCodeService _authCodeService;

        private readonly IUserService _userService;

        public SmsAuthCodeValidator(IAuthCodeService authCodeService, IUserService userService)
        {
            _authCodeService = authCodeService;
            _userService = userService;

        }

        public async Task ValidateAsync(ExtensionGrantValidationContext context)
        {
            //从请求中获得 手机号和验证码
            var phone = context.Request.Raw["phone"];
            var code = context.Request.Raw["auth_code"];

            //授权失败
            var errorValidationResult = new GrantValidationResult(TokenRequestErrors.InvalidGrant);
            //检查手机号和验证码参数是否符合预期
            if(string.IsNullOrWhiteSpace(phone) || string.IsNullOrWhiteSpace(code))
            {
                context.Result = errorValidationResult;
                return;
            }
            //检查手机号和验证码是否匹配
            if(!await _authCodeService.Validate(phone,code))
            {
                context.Result = errorValidationResult;
                return;
            }
            var userId = await _userService.GetOrCreate(phone);          
            if(userId <=0)
            {  
                //如果用户ID小于等于0 ，验证失败
                context.Result = errorValidationResult;
                return;
            }
            context.Result = new GrantValidationResult(userId.ToString(), GrantType);
        }
    }
}
