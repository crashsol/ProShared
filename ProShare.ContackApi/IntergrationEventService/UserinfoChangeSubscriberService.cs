using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DotNetCore.CAP;
using Microsoft.Extensions.Logging;
using ProShare.ContactApi.Data;
using ProShare.ContactApi.IntergrationEventService.Dtos;
using ProShare.ContactApi.Models.Dtos;

namespace ProShare.ContactApi.IntergrationEventService
{

    /// <summary>
    /// 用户修改消息  消费者处理类
    /// </summary>
    public class UserinfoChangeSubscriberService : IUserinfoChangeSubscriberService, ICapSubscribe
    {
        private readonly IContactBookRepository _contactBookRepository;

        private readonly ILogger<UserinfoChangeSubscriberService> _logger;

        public UserinfoChangeSubscriberService(IContactBookRepository contactBookRepository, ILogger<UserinfoChangeSubscriberService> logger)
        {
            _contactBookRepository = contactBookRepository;
            _logger = logger;
        }
        //指定消费的类型
        [CapSubscribe("proshare.userapi.userprofilechanged")]
        public async Task ChangeUserinfoAsync(UserInfoChangeEventModel eventModel)
        {

            _logger.LogInformation($"时间:{DateTime.Now }  收到用户 {eventModel.Name} 修改信息事件");
            Console.WriteLine($"时间:{DateTime.Now }  收到用户 {eventModel.Name} 修改信息事件");
            var result = await _contactBookRepository.UpdateUserInfoAsync(new BaseUserInfo
            {
                UserId = eventModel.UserId,
                Name = eventModel.Name,
                Title = eventModel.Title,
                Company = eventModel.Company,
                Avatar = eventModel.Avatar

            }, new CancellationToken());
            if (result)
            {
                _logger.LogInformation($"更新用户信息成功");
            }
            else
            {
                _logger.LogInformation($"更新用户信息失败");
            }
            Console.WriteLine("更新用户信息成功");         
        }
    }
}
