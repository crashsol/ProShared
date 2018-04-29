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
    /// 领域-项目被查看事件处理
    /// </summary>
    public class ProjectViewedEventHandler : INotificationHandler<ProjectViewedEvent>
    {

        private readonly ICapPublisher _capPublisher;
        public ProjectViewedEventHandler(ICapPublisher capPublisher)
        {
            _capPublisher = capPublisher;
        }
        /// <summary>
        /// 将领域事件通过CAP 发出集成事件，由消息服务接受并处理
        /// </summary>
        /// <param name="notification"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task Handle(ProjectViewedEvent notification, CancellationToken cancellationToken)
        {
            var @event = new ProjectViewedIntergrationEvent
            {
                Company = notification.Company,
                Introduction = notification.Introduction,
                Viewer = notification.ProjectViewer
            };
            _capPublisher.Publish("proshare.projectapi.projectviewed", @event);
            return Task.CompletedTask;
        }
    }
}
