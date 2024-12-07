// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using DotXxlJob.Core.Models;
using System.Threading;
using System.Threading.Tasks;

namespace DotXxlJob.Core.CommandExecutors
{
    public class KillCommandExecutor: ICommandExecutor
    {
        private readonly ISerializer _serializer;

        public KillCommandExecutor(ISerializer serializer)
        {
            _serializer = serializer;
        }
        public string CommandName => "kill";
        public Task<ExecutorResult> ExecuteAsync(byte[] payload, CancellationToken cancellationToken)
        {
            var command = _serializer.Deserialize<IdleBeatCommand>(payload);
            if (command == null)
            {
                return Task.FromResult(ExecutorResult.Failure("Command[Kill],parameter is empty"));
            }
            throw new System.NotImplementedException();
        }
    }
}