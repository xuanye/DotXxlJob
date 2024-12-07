// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using System.Runtime.Serialization;

namespace DotXxlJob.Core.Models
{
    public class ExecutorResult
    {
        protected const int SUCCESS_CODE = 200;
        protected const int FAILURE_CODE = 500;
        protected const int TIMEOUT_CODE = 502;
        [DataMember(Name = "code",Order = 1)]
        public  int Code { get; set; }
        [DataMember(Name = "msg",Order = 2)]
        public string? Message { get; set; }

        public static ExecutorResult Success(string message="")
        {
            return new ExecutorResult(){Code = SUCCESS_CODE, Message = message};
        }
        public static ExecutorResult Failure(string message)
        {
            return new ExecutorResult(){Code = FAILURE_CODE, Message = message};
        }
        
    }

    public class ExecutorResult<T> : ExecutorResult where T : class
    {
        [DataMember(Name = "content",Order = 3)]
        public T Data { get; set; } = default!;
        
        public static ExecutorResult Success(string message, T data)
        {
            return new ExecutorResult<T>(){Code = SUCCESS_CODE, Message = message,Data = data};
        }
       
    }
}