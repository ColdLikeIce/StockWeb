using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CommonCore.Result
{
    public class ApiResult<TData> : ApiResult, IApiResult<TData>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApiResult{TResult}"/> class.
        /// </summary>
        public ApiResult()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiResult{TResult}" /> class.
        /// </summary>
        /// <param name="data">The result.</param>
        /// <param name="code">The status code.</param>
        public ApiResult(TData result, bool? isSuccess)
        {
            IsSuccess = isSuccess.Value;
            Result = result;
        }

        /// <summary>
        /// Gets or sets the result.
        /// </summary>
        /// <value>The result.</value>
        public TData Result { get; set; }
    }

    public class ApiResult : IApiResult
    {
        /// <summary>
        /// Represents an empty <see cref="IApiResult"/>.
        /// </summary>
        public static readonly IApiResult Empty = new ApiResult
        {
            IsSuccess = true,
            StatusCode = 200
        };

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

        /// <summary>
        /// Creates a new instance of <see cref="IApiResult{TData}"/> by the specified result.
        /// </summary>
        /// <typeparam name="TData">The type of the result.</typeparam>
        /// <param name="data">The result.</param>
        /// <returns>An instance inherited from <see cref="IApiResult{TResult}"/> interface.</returns>
        public static IApiResult<TData> Succeed<TData>(TData result) => new ApiResult<TData>
        {
            StatusCode = 200,
            IsSuccess = true,
            Result = result
        };

        /// <summary>
        /// Creates a new instance of <see cref="IApiResult"/> by the specified status code and message.
        /// </summary>
        /// <param name="code">The status code.</param>
        /// <param name="message">The message.</param>
        /// <returns>An instance inherited from <see cref="IApiResult"/> interface.</returns>
        public static IApiResult From(int code, string message = null) => new ApiResult
        {
            StatusCode = 200,
            IsSuccess = true,
        };

        /// <summary>
        ///  Creates a new instance of <see cref="IApiResult"/> by the specified error message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="code">The status code</param>
        /// <returns>An instance inherited from <see cref="IApiResult"/> interface.</returns>
        public static IApiResult<TData> Failed<TData>(TData data) => new ApiResult<TData>
        {
            StatusCode = 200,
            IsSuccess = false,
            Result = data
        };

        /// <summary>
        ///  Creates a new instance of <see cref="IApiResult{TResult}"/> by the specified error message.
        /// </summary>
        /// <typeparam name="TData">The type of the result.</typeparam>
        /// <param name="data">The error result.</param>
        /// <param name="message">The message.</param>
        /// <param name="code">The status code.</param>
        /// <returns>An instance inherited from <see cref="IApiResult"/> interface.</returns>
        public static IApiResult<TData> Failed<TData>(TData data, string message, int? code = null) => new ApiResult<TData>
        {
            StatusCode = 200,
            IsSuccess = false,
            Result = data
        };

        /// <summary>
        /// Creates a new instance of <see cref="IApiResult{TResult}"/> by the specified result.
        /// </summary>
        /// <typeparam name="TData">The type of the result.</typeparam>
        /// <param name="data">The result.</param>
        /// <param name="code">The status code.</param>
        /// <param name="message">The message.</param>
        /// <returns>An instance inherited from <see cref="IApiResult{TResult}"/> interface.</returns>
        public static IApiResult<TData> From<TData>(TData data, bool isSuccess) => new ApiResult<TData>
        {
            IsSuccess = isSuccess,
            StatusCode = 200,
            Result = data
        };
    }
}