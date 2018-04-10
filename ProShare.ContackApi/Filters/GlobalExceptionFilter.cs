using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProShare.ContactApi.Filters
{
    public class GlobalExceptionFilter : IExceptionFilter
    {

        private readonly ILogger _logger;

        private readonly IHostingEnvironment _env;

        public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger, IHostingEnvironment env)
        {
            _env = env;
            _logger = logger;
        }
        public void OnException(ExceptionContext context)
        {
            var result = new JsonErrorResponsecs();
            if (context.Exception.GetType() == typeof(UserOperationException))
            {
                result.Message = context.Exception.Message;
                context.Result = new BadRequestObjectResult(result);
            }
            else
            {
                result.Message = "发生了未知的内部错误";
                if (_env.IsDevelopment())
                {
                    //非生产环境就返回堆栈错误信息
                    result.DeveloperMessage = context.Exception.StackTrace;
                }
                context.Result = new InternalServerErrorObjectResult(result);
            }
            //记录错误信息
            _logger.LogError(context.Exception, context.Exception.Message);
            context.ExceptionHandled = true;
            

        }
    }

    /// <summary>
    /// 系统未知异常
    /// </summary>
    public class InternalServerErrorObjectResult : ObjectResult
    {
        public InternalServerErrorObjectResult(object error) : base(error)
        {
            StatusCode = StatusCodes.Status500InternalServerError;
        }

    }

}
