using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProShare.ContactApi
{
    public class UserOperationException:Exception
    {

        public UserOperationException(){ }

        public UserOperationException(string message):base(message){ }

        public UserOperationException(string message, Exception exception) : base(message, exception) { }

    }
}
