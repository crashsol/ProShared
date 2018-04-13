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
using Infrastructure.OperationException;
using ProShare.ContactApi.Models.Dtos;

namespace ProShare.ContactApi.Controllers
{
    [Route("api/contacts")]
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
        /// 获取好友列表
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        public async Task<IActionResult> GetContactsAsync(CancellationToken cancellationToken)
        {
            return Ok(await _contactBookRepository.GetContactsAsync(UserIdentity.UserId, cancellationToken));

        }


        /// <summary>
        /// 给好友打标签
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("tag")]
        public async Task<IActionResult> TagContactAsync([FromBody]ContactTagVModel model, CancellationToken cancellationToken)
        {
            var result = await _contactBookRepository.TagContactAsync(UserIdentity.UserId, model.UserId, model.Tags, cancellationToken);
            if (!result)
            {
                _logger.LogInformation($"用户{UserIdentity}给好友{model.UserId}添加标签失败");
                throw new UserOperationException("添加好友标签失败");
            }
            return Ok();
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
        /// <param name="applierId">申请添加的好友ID</param>
        /// <returns></returns>
        [HttpPost]
        [Route("apply-requests/{applierId}")]
        public async Task<IActionResult> AddApplyRequesetAsync(int applierId, CancellationToken cancellationToken)
        {
            var result = await _contactApplyRequestRepository.AddRequestAsync(new ContactApplyRequest
            {
                //申请待添加的好友ID
                UserId = applierId,

                //本人/声请人的 个人信息
                ApplierId = UserIdentity.UserId,
                Name = UserIdentity.Name,
                Title = UserIdentity.Title,
                Company = UserIdentity.Company,
                Avatar = UserIdentity.Avatar,
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
        [Route("apply-requests/{applierId}")]
        public async Task<IActionResult> ApproveRequestAsync(int applierId, CancellationToken cancellationToken)
        {
            var result = await _contactApplyRequestRepository.ApprovalAsync(UserIdentity.UserId, applierId, cancellationToken);
            if (!result)
            {
                _logger.LogInformation($"用户{UserIdentity.UserId},审核 {applierId} 好友申请失败!");
                throw new UserOperationException("通过好友请求失败！");
            }
            //通过好友添加申请后， 更新双方的通讯里
            //本人信息 UserIdentity中获得
            var userinfo = new BaseUserInfo
            {
                UserId = UserIdentity.UserId,
                Name = UserIdentity.Name,
                Avatar = UserIdentity.Avatar,
                Company = UserIdentity.Company,
                Title = UserIdentity.Company
            };
            var applyUserinfo = await _userService.GetBaseUserInfoAsync(applierId);
            await _contactBookRepository.AddContactAsync(UserIdentity.UserId, applyUserinfo, cancellationToken);
            await _contactBookRepository.AddContactAsync(applierId, userinfo, cancellationToken);
            return Ok();
        }



    }
}
