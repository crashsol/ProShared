using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.OperationException
{
    /// <summary>
    /// 异常信息返回
    /// </summary>
    public class JsonErrorResponsecs
    {

        public string Message { get; set; }

        public object DeveloperMessage { get; set; }
    }
}
