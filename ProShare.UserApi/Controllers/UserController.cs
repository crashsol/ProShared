using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProShare.UserApi.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.JsonPatch;
using ProShare.UserApi.Models;

namespace ProShare.UserApi.Controllers
{
    [Route("api/users")]
    public class UserController : BaseController
    {

        private readonly UserContext _dbContext;
        public UserController(ILogger<BaseController> logger, UserContext userContext) : base(logger)
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
                    .Include(b => b.Properties)
                    .SingleOrDefaultAsync(b => b.Id == UserIdentity.UserId);
            if (user == null)
                throw new UserOperationException($"错误的用户编号：{UserIdentity.UserId}");
            return Json(user);
        }

        /// <summary>
        /// 用户更新个人信息
        /// </summary>
        /// <returns></returns>
        [Route("")]
        [HttpPatch]
        public async Task<IActionResult> Patch([FromBody]JsonPatchDocument<AppUser> patch)
        {
            var entity = await _dbContext.Users
                                .SingleOrDefaultAsync(b => b.Id == UserIdentity.UserId);

            //将需要跟新的数据复制给对象
            patch.ApplyTo(entity);
         
            //如果有修改Properties, 不追踪 AppUser 实体的 Properties 属性 单独通过以下的方法进行处理
            if (entity.Properties != null)
            {
           
                foreach (var item in entity.Properties)
                {
                    _dbContext.Entry(item).State = EntityState.Detached;
                  
                }
              
                //Properties 属性 单独通过以下的方法进行处理
                //获取原来用户所有的Properties, 必须使用 AsNoTracking()，否则会自动附加到用户属性上
                var originProperties = await _dbContext.UserProperties.AsNoTracking().Where(b => b.AppUserId == UserIdentity.UserId).ToListAsync();
           

                foreach (var item in originProperties)
                {
                    if (!entity.Properties.Exists(b => b.Key == item.Key && b.Value == item.Value))
                    {
                        //如果不存在做删除操作
                        _dbContext.Remove(item);
                    }
                }
                foreach (var item in entity.Properties)
                {
                    if (!originProperties.Exists(b => b.Key == item.Key && b.Value == item.Value))
                    {
                        //如果不存在做新增操作
                        _dbContext.Add(item);
                    }
                }               
            }
            
            //更新用户信息
            _dbContext.Users.Update(entity);
            _dbContext.SaveChanges();

            return Json(entity);
        }

    }
}