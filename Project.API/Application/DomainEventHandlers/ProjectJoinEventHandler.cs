using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DotNetCore.CAP;
using MediatR;
using Project.API.Application.IngergrationEvents;
using Project.Domain.Events;
namespace Project.API.Application.DomainEventHandlers
{
    /// <summary>
    /// 领域-事件 贡献者加入处理
    /// </summary>
    public class ProjectJoinEventHandler : INotificationHandler<ProjectJoinEvent>
    {
        private readonly ICapPublisher _capPublisher;
        public ProjectJoinEventHandler(ICapPublisher capPublisher)
        {
            _capPublisher = capPublisher;
        }
        /// <summary>
        /// 将领域事件通过CAP 发出集成事件，由消息服务接受并处理
        /// </summary>
        /// <param name="notification"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task Handle(ProjectJoinEvent notification, CancellationToken cancellationToken)
        {
            var @event = new ProjectJoinedIntergrationEvent
            {
                Company = notification.Company,
                Introduction = notification.Introduction,
                ProjectContributor = notification.ProjectContributor
            };
            _capPublisher.Publish("proshare.projectapi.projectjoined", @event);
            return Task.CompletedTask;
        }
    }
}
