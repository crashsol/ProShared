using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Project.Domain.Events;

namespace Project.API.Application.DomainEventHandlers
{
    /// <summary>
    /// 领域事件 项目创建处理
    /// </summary>
    public class ProjectCreatedEventHandler : INotificationHandler<ProjectCreatedEvent>
    {
        public Task Handle(ProjectCreatedEvent notification, CancellationToken cancellationToken)
        {
            return  Task.FromResult(0);
        }
    }
}
