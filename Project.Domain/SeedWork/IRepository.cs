using System;
using System.Collections.Generic;
using System.Text;

namespace Project.Domain.SeedWork
{
    public interface IProjectRepository<T> where T : IAggregateRoot
    {
        IUnitOfWork UnitOfWork { get; }
    }
}
