using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProShare.UserApi.Data;
using Microsoft.EntityFrameworkCore;

namespace ProShare.UserApi.Controllers
{
    [Route("api/users")]
    public class UserController : BaseController
    {

        private readonly UserContext _dbContext;
        public UserController(ILogger<BaseController> logger,UserContext userContext) : base(logger)
        {
            _dbContext = userContext;
        }

        /// <summary>
        /// 登录用户获取个人信息
        /// </summary>
        /// <returns></returns>
        [Route("")]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var user = await _dbContext.Users
                    .AsNoTracking()
                    .Include(b=>b.Properties)
                    .SingleOrDefaultAsync(b => b.Id == UserIdentity.UserId);
            if (user == null)
                throw new UserOperationException($"错误的用户编号：{UserIdentity.UserId}");
            return Json( user);
        }

        /// <summary>
        /// 用户更新个人信息
        /// </summary>
        /// <returns></returns>
        [Route("")]
        [HttpPatch]
        public IActionResult Patch()
        {
            return Ok();
        }
       
    }
}