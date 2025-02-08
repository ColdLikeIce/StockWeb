using CommonCore.Result;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CommonCore.CommonResult
{
    /// <summary>
    /// 表示处理API异常的筛选器。
    /// </summary>
    public class ApiCommonExceptionFilterAttribute : Attribute, IExceptionFilter
    {
        private readonly ILogger<ApiExceptionFilterAttribute> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiExceptionFilterAttribute" /> class.
        /// </summary>
        /// <param name="logger">The logger</param>
        public ApiCommonExceptionFilterAttribute(ILogger<ApiExceptionFilterAttribute> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Called when [exception].
        /// </summary>
        /// <param name="context">The context.</param>
        public void OnException(ExceptionContext context)
        {
            _logger.LogError(0, context.Exception, $"ip={context.HttpContext.Connection.RemoteIpAddress}, path={context.HttpContext.Request.Path}, error={JsonSerializer.Serialize(context.Exception.Message)}");

            if (context.Exception.Data != null && context.Exception.Data["code"] != null)
            {
                var code = (int)context.Exception.Data["code"];
                context.Result = new ObjectResult(CommonApiResult.Failed(context.Exception.Message, code));
            }
            else
            {
                context.Result = new ObjectResult(CommonApiResult.Failed(context.Exception.Message));
            }

            context.ExceptionHandled = true;
        }
    }
}