// Copyright (c) Xuanye Wang. All rights reserved.
// Licensed under MIT license

using System.Threading.Tasks;
using DotXxlJob.Core.Models;

namespace DotXxlJob.Core.CommandExecutors
{
    public class TriggerCommandExecutor : ICommandExecutor
    {
        private readonly ISerializer _serializer;

        public TriggerCommandExecutor(ISerializer serializer)
        {
            _serializer = serializer;
        }
        public string CommandName => "Run";
        public Task<ApiResult> ExecuteAsync(byte[] payload)
        {
            var command = _serializer.Deserialize<TriggerCommand>(payload);
            if (command == null)
            {
                return Task.FromResult(ApiResult.Failure("command[run],parameter is empty"));
            }
            throw new System.NotImplementedException();
        }
    }
}