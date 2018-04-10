using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProShare.ContactApi.Data;
using ProShare.ContactApi.Services;
using ProShare.ContactApi.Models;
using System.Threading;
using ProShare.ContactApi.Models.VModels;

namespace ProShare.ContactApi.Controllers
{
    [Route("api/[controller]")]
    public class ContactController : BaseController
    {

        private readonly IContactApplyRequestRepository _contactApplyRequestRepository;

        private readonly IContactBookRepository _contactBookRepository;

        private readonly IUserService _userService;

        public ContactController(IUserService userService, IContactApplyRequestRepository contactApplyRequestRepository,
            ILogger<BaseController> logger,
            IContactBookRepository contactBookRepository) : base(logger)
        {
            _contactApplyRequestRepository = contactApplyRequestRepository;
            _userService = userService;
            _contactBookRepository = contactBookRepository;
        }

        /// <summary>
        /// 获取好友申请列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("apply-requests")]
        public async Task<IActionResult> GetApplyRequest()
        {
            var result = await _contactApplyRequestRepository.GetRequestList(UserIdentity.UserId, new CancellationToken());
            return Ok(result);
        }

        /// <summary>
        /// 添加好友 请求
        /// </summary>
        /// <param name="userId">申请添加的好友ID</param>
        /// <returns></returns>
        [HttpPost]
        [Route("apply-requests")]
        public async Task<IActionResult> AddApplyRequesetAsync(int userId, CancellationToken cancellationToken)
        {
            //获取本人的个人信息
            var baseUserInfo = _userService.GetBaseUserInfo(UserIdentity.UserId);
            if (baseUserInfo == null)
            {
                throw new UserOperationException("用户参数错误");
            }
            var result = await _contactApplyRequestRepository.AddRequestAsync(new ContactApplyRequest
            {
                //申请待添加的好友ID
                UserId = userId,

                //本人/声请人的 个人信息
                ApplierId = UserIdentity.UserId,
                Name = baseUserInfo.Name,
                Title = baseUserInfo.Title,
                Company = baseUserInfo.Company,
                Avatar = baseUserInfo.Avatar,
                ApplyTime = DateTime.Now

            }, cancellationToken);
            if (!result)
            {
                throw new UserOperationException("添加好友请求失败");
            }
            return Ok();


        }


        /// <summary>
        /// 通过好友请求
        /// </summary>
        /// <param name="applierId">申请人ID</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("apply-requests")]
        public async Task<IActionResult> ApproveRequestAsync(int applierId, CancellationToken cancellationToken)
        {
            var result = await _contactApplyRequestRepository.ApprovalAsync(UserIdentity.UserId,applierId, cancellationToken);
            if (!result)
            {
                _logger.LogInformation($"用户{UserIdentity.UserId},审核 {applierId} 好友申请失败!");
                throw new UserOperationException("通过好友请求失败！");
            }
            //通过好友添加申请后， 更新双方的通讯里
            //本人信息
            var userinfo = _userService.GetBaseUserInfo(UserIdentity.UserId);
            var applyUserinfo = _userService.GetBaseUserInfo(applierId);
            await _contactBookRepository.AddContactAsync(UserIdentity.UserId, applyUserinfo, cancellationToken);
            await _contactBookRepository.AddContactAsync(applierId, userinfo, cancellationToken);          
            return Ok();
        }

        [HttpPut]
        [Route("tag")]
        public async Task<IActionResult> TagContactAsync([FromBody]ContactTagVModel model,CancellationToken cancellationToken)
        {
           var result = await _contactBookRepository.TagContactAsync(UserIdentity.UserId, model.UserId, model.Tags, cancellationToken);
            if(!result)
            {
                _logger.LogInformation($"用户{UserIdentity}给好友{model.UserId}添加标签失败");
                throw new UserOperationException("添加好友标签失败");
            }
            return Ok();
        }

    }
}
