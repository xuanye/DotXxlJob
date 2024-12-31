// Copyright (c) Xuanye Wang. All rights reserved.
// Licensed under MIT license

using DotXxlJob.Core.Models;
using System.Threading.Tasks;

namespace DotXxlJob.Core.CommandExecutors
{
    public class IdleBeatCommandExecutor : ICommandExecutor
    {
        private readonly ISerializer _serializer;

        public IdleBeatCommandExecutor(ISerializer serializer)
        {
            _serializer = serializer;
        }
        public string CommandName => "IdleBeat";

        public Task<ApiResult> ExecuteAsync(byte[] payload)
        {
            var idleBeat = _serializer.Deserialize<IdleBeatCommand>(payload);
            if (idleBeat == null)
            {
                return Task.FromResult(ApiResult.Failure("Command[IdleBrat],parameter is empty"));
            }
            throw new System.NotImplementedException();
        }
    }
}