using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

namespace Project.Domain.SeedWork
{
    /// <summary>
    /// 实体基类
    /// </summary>
    public abstract class Entity
    {
        int? _requestedHashCode;
        int _Id;

        public virtual int Id
        {
            get
            {
                return _Id;
            }
            protected set
            {
                _Id = value;
            }
        }

        /// <summary>
        /// 领域事件
        /// </summary>
        private List<INotification> _domainEvents;

        /// <summary>
        /// 领域事件 (只读列表)
        /// </summary>
        public IReadOnlyCollection<INotification> DomainEvents => _domainEvents.AsReadOnly();

        /// <summary>
        /// 添加一个领域事件
        /// </summary>
        /// <param name="eventItem"></param>
        public void AddDomainEvent(INotification eventItem)
        {
            _domainEvents = _domainEvents ?? new List<INotification>();
            _domainEvents.Add(eventItem);
        }

        /// <summary>
        /// 移除一个领域事件
        /// </summary>
        /// <param name="eventItem"></param>
        public void RemoveDomainEvent(INotification eventItem)
        {
            _domainEvents?.Remove(eventItem);
        }

        /// <summary>
        /// 清空领域事件
        /// </summary>
        public void ClearDomainEvent()
        {
            _domainEvents?.Clear();
        }

        /// <summary>
        /// 判断Id是否为int32
        /// </summary>
        /// <returns></returns>
        public bool IsTransient()
        {
            return this.Id == default(Int32);
        }

        /// <summary>
        /// 重写相等判断
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is Entity))
                return false;
            //是否是引用
            if (Object.ReferenceEquals(this, obj))
                return true;

            if (this.GetType() != obj.GetType())
                return false;

            Entity item = (Entity)obj;

            if (item.IsTransient() || this.IsTransient())
                return false;
            else
                return item.Id == this.Id;
        }

        public override int GetHashCode()
        {
            if (!IsTransient())
            {
                if (!_requestedHashCode.HasValue)
                    _requestedHashCode = this.Id.GetHashCode() ^ 31; // XOR for random distribution (http://blogs.msdn.com/b/ericlippert/archive/2011/02/28/guidelines-and-rules-for-gethashcode.aspx)

                return _requestedHashCode.Value;
            }
            else
                return base.GetHashCode();
        }

        public static bool operator ==(Entity left, Entity right)
        {
            if (Object.Equals(left, null))
                return (Object.Equals(right, null)) ? true : false;
            else
                return left.Equals(right);
        }

        public static bool operator !=(Entity left, Entity right)
        {
            return !(left == right);
        }

    }
}
