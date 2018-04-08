using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProShare.ContactApi.Data;
using ProShare.ContactApi.Services;
using ProShare.ContactApi.Models;

namespace ProShare.ContactApi.Controllers
{
    [Route("api/[controller]")]
    public class ContactController : BaseController
    {

        private readonly IContactApplyRequestRepository _contactApplyRequestRepository;

        private readonly IUserService _userService;

        public ContactController(IUserService userService, IContactApplyRequestRepository contactApplyRequestRepository, ILogger<BaseController> logger) : base(logger)
        {
            _contactApplyRequestRepository = contactApplyRequestRepository;
            _userService = userService;
        }

        /// <summary>
        /// 获取好友申请列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("apply-requests")]
        public async Task<IActionResult> GetApplyRequest()
        {
            var result = await _contactApplyRequestRepository.GetRequestList(UserIdentity.UserId);
            return Ok(result);
        }

        /// <summary>
        /// 添加好友请求
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("apply-requests")]
        public async Task<IActionResult> AddApplyRequesetAsync(int userId)
        {

            var baseUserInfo = _userService.GetBaseUserInfo(userId);
            if (baseUserInfo == null)
            {
                throw new Exception("用户参数错误");
            }
            var result = await _contactApplyRequestRepository.AddRequest(new ContactApplyRequest
            {
                ApplierId = UserIdentity.UserId,
                UserId = userId,
                Name = baseUserInfo.Name,
                Title = baseUserInfo.Title,
                Company = baseUserInfo.Company,
                Avatar = baseUserInfo.Avatar,
                CreateTime = DateTime.Now

            });
            if (!result)
            {
                return BadRequest();
            }
            return Ok();


        }
        /// <summary>
        /// 通过好友请求
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [Route("apply-requests")]
        public async Task<IActionResult> ApproveRequestAsync(int applierId)
        {
            var result =await _contactApplyRequestRepository.Approval(applierId);
            if (!result)
            {
                return BadRequest();
            }
            return Ok();
        }

    }
}
