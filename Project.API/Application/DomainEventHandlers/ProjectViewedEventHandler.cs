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
    /// 领域事件 项目被查看
    /// </summary>
    public class ProjectViewedEventHandler : INotificationHandler<ProjectViewedEvent>
    {
        public Task Handle(ProjectViewedEvent notification, CancellationToken cancellationToken)
        {
            return Task.FromResult(0);
        }
    }
}
