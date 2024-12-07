// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using DotXxlJob.Core.Models;
using System.Threading;
using System.Threading.Tasks;

namespace DotXxlJob.Core.CommandExecutors
{
    public class TriggerCommandExecutor: ICommandExecutor
    {
        private readonly ISerializer _serializer;

        public TriggerCommandExecutor(ISerializer serializer)
        {
            _serializer = serializer;
        }
        public string CommandName => "Run";
        public Task<ExecutorResult> ExecuteAsync(byte[] payload, CancellationToken cancellationToken)
        {
            var command = _serializer.Deserialize<TriggerCommand>(payload);
            if (command== null)
            {
                return Task.FromResult(ExecutorResult.Failure("command[run],parameter is empty"));
            }
            throw new System.NotImplementedException();
        }
    }
}