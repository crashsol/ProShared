using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Project.Domain.Events;
using DotNetCore.CAP;
using Project.API.Application.IngergrationEvents;

namespace Project.API.Application.DomainEventHandlers
{
    /// <summary>
    /// 领域-事件 项目创建处理
    /// </summary>
    public class ProjectCreatedEventHandler : INotificationHandler<ProjectCreatedEvent>
    {
        private readonly ICapPublisher _capPublisher;

        public ProjectCreatedEventHandler(ICapPublisher capPublisher)
        {
            _capPublisher = capPublisher;
        }

        /// <summary>
        /// 将领域事件通过CAP 发出集成事件，由消息服务接受并处理
        /// </summary>
        /// <param name="notification"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task Handle(ProjectCreatedEvent notification, CancellationToken cancellationToken)
        {
            var @event = new ProjectCreatedIntergrationEvent
            {
                CreatedTime = DateTime.Now,
                UserId = notification.Project.UserId,
                ProjectId = notification.Project.Id,
                Company = notification.Project.Company,
                FinStage = notification.Project.FinStage,
                Introducation = notification.Project.Introduction,
                ProjectAvatar = notification.Project.Avatar,
                Tags = notification.Project.Tags

            };
            _capPublisher.Publish("proshare.projectapi.projectcreated", @event);
            return Task.CompletedTask;
        }
    }
}
