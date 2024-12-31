// Copyright (c) Xuanye Wang. All rights reserved.
// Licensed under MIT license

using DotXxlJob.Core.Models;
using System.Threading.Tasks;

namespace DotXxlJob.Core.CommandExecutors
{
    public class KillCommandExecutor : ICommandExecutor
    {
        private readonly ISerializer _serializer;

        public KillCommandExecutor(ISerializer serializer)
        {
            _serializer = serializer;
        }
        public string CommandName => "kill";
        public Task<ApiResult> ExecuteAsync(byte[] payload)
        {
            var command = _serializer.Deserialize<IdleBeatCommand>(payload);
            if (command == null)
            {
                return Task.FromResult(ApiResult.Failure("Command[Kill],parameter is empty"));
            }
            throw new System.NotImplementedException();
        }
    }
}