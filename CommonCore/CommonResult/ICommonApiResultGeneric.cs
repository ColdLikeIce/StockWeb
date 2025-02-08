using CommonCore.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonCore.CommonResult
{
    public interface ICommonApiResult<TData> : ICommonApiResult
    {
        /// <summary>
        /// Gets or sets the result.
        /// </summary>
        /// <value>The result.</value>
        TData Data { get; set; }
    }
}