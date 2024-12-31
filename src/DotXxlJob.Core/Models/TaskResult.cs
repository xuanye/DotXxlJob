// Copyright (c) Xuanye Wang. All rights reserved.
// Licensed under MIT license

namespace DotXxlJob.Core.Models
{
    public enum ResultCodes
    {
        Success = 200,
        Failure = 500,
        Timeout = 502
    }

    public class TaskResult
    {
        public ResultCodes Code { get; set; }

        public string? Message { get; set; }

        private static TaskResult _successResult = new() { Code = ResultCodes.Success };
        private static TaskResult _timeoutResult = new() { Code = ResultCodes.Timeout };

        public static TaskResult Failure(string message)
        {
            return new TaskResult() { Code = ResultCodes.Failure, Message = message };
        }
        public static TaskResult Timeout()
        {
            return _timeoutResult;
        }
        public static TaskResult Success()
        {
            return _successResult;
        }

    }

    internal class TaskResult<T> : TaskResult where T : class
    {
        public T Data { get; set; } = default!;
    }
}
