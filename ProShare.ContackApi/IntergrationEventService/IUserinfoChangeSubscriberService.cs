using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProShare.ContactApi.IntergrationEventService
{
    public interface IUserinfoChangeSubscriberService
    {

         Task ChangeUserinfoAsync(UserInfoChangeEventModel eventModel);
    }
}
