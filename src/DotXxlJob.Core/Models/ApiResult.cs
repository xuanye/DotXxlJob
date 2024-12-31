// Copyright (c) Xuanye Wang. All rights reserved.
// Licensed under MIT license

using Microsoft.AspNetCore.Http;
using System.Runtime.Serialization;

namespace DotXxlJob.Core.Models
{
    public class ApiResult
    {
        protected const int SUCCESS_CODE = StatusCodes.Status200OK;
        protected const int FAILURE_CODE = StatusCodes.Status500InternalServerError;
        protected const int TIMEOUT_CODE = StatusCodes.Status502BadGateway;

        [DataMember(Name = "code", Order = 1)]
        public int Code { get; set; }
        [DataMember(Name = "msg", Order = 2)]
        public string? Message { get; set; }

        public static ApiResult Success(string message = "")
        {
            return new ApiResult() { Code = SUCCESS_CODE, Message = message };
        }
        public static ApiResult Failure(string message)
        {
            return new ApiResult() { Code = FAILURE_CODE, Message = message };
        }

    }

    public class ApiResult<T> : ApiResult where T : class
    {
        [DataMember(Name = "content", Order = 3)]
        public T Data { get; set; } = default!;

        public static ApiResult Success(string message, T data)
        {
            return new ApiResult<T>() { Code = SUCCESS_CODE, Message = message, Data = data };
        }

    }
}