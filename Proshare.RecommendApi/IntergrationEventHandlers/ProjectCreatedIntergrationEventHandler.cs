using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotNetCore.CAP;
using Proshare.RecommendApi.Data;
using Proshare.RecommendApi.IntergrationEvents;
using Proshare.RecommendApi.Models;
using Proshare.RecommendApi.Models.Dtos;
using Proshare.RecommendApi.Services;
using ProShare.RecommendApi.Services;

namespace Proshare.RecommendApi.IntergrationEventHandlers
{
    public class ProjectCreatedIntergrationEventHandler : ICapSubscribe
    {

        private readonly RecommendDbContext _dbContext;

        private readonly IUserService _userService;

        private readonly IContactService _contactService;

        public ProjectCreatedIntergrationEventHandler(RecommendDbContext recommendDbContext,IContactService contactService, IUserService userService)
        {
            _dbContext = recommendDbContext;
            _contactService = contactService;
            _userService = userService;
        }

        [CapSubscribe("proshare.projectapi.projectcreated")]
        public async Task CreateRecommendFromProjectAsync(ProjectCreatedIntergrationEvent @event)
        {

            //调用UserApi 服务，获取项目创建人信息
            var userinfo = await _userService.GetBaseUserInfoAsync(@event.UserId);

            //调用ContactApi 获取项目创建者的 好友信息
            var friendIds = await _contactService.GetContactsByUserIdAsync(@event.UserId);
           

            foreach (var item in friendIds)
            {
                var recommend = new ProjectRecommend
                {
                    FromUserId = @event.UserId,
                    Company = @event.Company,
                    Tags = @event.Tags,
                    ProjectId = @event.ProjectId,
                    FinStage = @event.FinStage,
                    ProjectAvatar = @event.ProjectAvatar,
                    CreatedTime = @event.CreatedTime,
                    Introducation = @event.Introducation,
                    RecommendType = EnumRecommendType.Friend,
                    RecommendTime = DateTime.Now,
                    FromUserAvatar = userinfo.Avatar,
                    FromUserName = userinfo.Name,
                    UserId = item.UserId
                };

                _dbContext.ProjectRecommends.Add(recommend);
            }
           await  _dbContext.SaveChangesAsync();           
           
        }

    }
}
