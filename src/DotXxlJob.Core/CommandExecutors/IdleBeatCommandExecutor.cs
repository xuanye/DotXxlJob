// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using DotXxlJob.Core.Models;
using System.Threading;
using System.Threading.Tasks;

namespace DotXxlJob.Core.CommandExecutors
{
    public class IdleBeatCommandExecutor:ICommandExecutor
    {
        private readonly ISerializer _serializer;

        public IdleBeatCommandExecutor(ISerializer serializer)
        {
            _serializer = serializer;
        }
        public string CommandName => "IdleBeat";
        
        public Task<ExecutorResult> ExecuteAsync(byte[] payload,CancellationToken cancellationToken = default)
        {
            var idleBeat = _serializer.Deserialize<IdleBeatCommand>(payload);
            if (idleBeat == null)
            {
                return Task.FromResult(ExecutorResult.Failure("Command[IdleBrat],parameter is empty"));
            }
            throw new System.NotImplementedException();
        }
    }
}