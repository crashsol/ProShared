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
using Infrastructure.OperationException;

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


        /// <summary>
        /// 检查或则创建用户 并返回用户基本信息
        /// </summary>
        /// <param name="phone">手机号码</param>
        /// <returns>用户ID</returns>
        [Route("check-or-create")]
        [HttpPost]
        public async Task<IActionResult> CheckOrCreateUser([FromBody]string phone)
        {
            var user = await _dbContext.Users.SingleOrDefaultAsync(b => b.Phone == phone);
            if (user == null)
            {
                //用户不存在，直接创建用户
                user = new AppUser { Phone = phone};
                await _dbContext.Users.AddAsync(user);

                await _dbContext.SaveChangesAsync();
            }
            return Ok(new
            {
                UserId = user.Id,
                user.Name,
                user.Title,
                user.Company,
                user.Avatar
            });
        }

        /// <summary>
        /// 更新用户标签数据
        /// </summary>
        /// <param name="tags">用户标签数据</param>
        /// <returns></returns>       
        [HttpPut]
        [Route("tags")]
        public async Task<IActionResult> UpdateTags([FromBody]List<string> tags)
        {
            var originTags = await _dbContext.UserTags.Where(b => b.AppUserId == UserIdentity.UserId).ToListAsync();
            var newTags = tags.Except(originTags.Select(b => b.Tag));
            await _dbContext.UserTags.AddRangeAsync(newTags.Select(b => new UserTag
            {
                CreateTime = DateTime.Now,
                AppUserId = UserIdentity.UserId,
                Tag = b
            }));
            await _dbContext.SaveChangesAsync();
            return Ok();
        }

        /// <summary>
        /// 获取用户标签数据
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("tags")]
        public async Task<IActionResult> GetUserTagsAsync()
        {
            return Ok(await _dbContext.UserTags.Where(b => b.AppUserId == UserIdentity.UserId).ToListAsync());
        }

        /// <summary>
        /// 通过手机号查询信息
        /// </summary>
        /// <param name="phone">手机号</param>
        /// <returns>人员信息</returns>
        /// 
        [HttpPost]
        [Route("search/{phone}")]
        public async Task<IActionResult> Search(string phone)
        {
            return Ok(await _dbContext.Users.Include(b => b.Properties).SingleOrDefaultAsync(b => b.Phone == phone));
        }

        [HttpGet]
        [Route("get-userinfo/{id}")]
        public async Task<IActionResult> GetUserBaseInfoAsync(int id)
        {
            var entity = await _dbContext.Users.SingleOrDefaultAsync(b => b.Id == id);

            if (entity == null)
            {
                _logger.LogInformation($"查询用户编号 {id},信息不存在");
                throw new UserOperationException("用户不存在");
            }
            return Ok(new
            {
                UserId = entity.Id,
                entity.Name,
                entity.Title,
                entity.Company,
                entity.Avatar
            });


        }



    }
}