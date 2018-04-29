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
    /// 领域事件 贡献者加入
    /// </summary>
    public class ProjectJoinEventHandler : INotificationHandler<ProjectJoinEvent>
    {
        public Task Handle(ProjectJoinEvent notification, CancellationToken cancellationToken)
        {
            return Task.FromResult(0);
        }
    }
}
