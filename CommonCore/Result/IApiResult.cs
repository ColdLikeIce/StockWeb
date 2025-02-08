using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CommonCore.Result
{
    public interface IApiResult
    {
        //
        // 摘要:
        //     请求是否成功
        public bool IsSuccess { get; set; }

        //
        // 摘要:
        //     HTTP 响应状态码
        public int StatusCode { get; set; }

        //
        // 摘要:
        //     原始内容
        public string OrginContent { get; set; }
    }

    public interface IApiResult<TData> : IApiResult
    {
        //
        // 摘要:
        //     结果
        public TData Result { get; set; }
    }
}