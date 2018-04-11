using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.OperationException
{
    /// <summary>
    /// 用户自定义错误信息
    /// </summary>
    public class UserOperationException:Exception
    {

        public UserOperationException(){ }

        public UserOperationException(string message):base(message){ }

        public UserOperationException(string message, Exception exception) : base(message, exception) { }

    }
}
