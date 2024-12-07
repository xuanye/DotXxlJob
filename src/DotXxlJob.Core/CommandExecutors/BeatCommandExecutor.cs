// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using DotXxlJob.Core.Models;
using System.Threading;
using System.Threading.Tasks;

namespace DotXxlJob.Core.CommandExecutors
{
    public class BeatCommandExecutor:ICommandExecutor
    {
        public string CommandName => "Beat";

        public Task<ExecutorResult> ExecuteAsync(byte[] payload,CancellationToken cancellationToken = default)
        {
            return Task.FromResult(ExecutorResult.Success());
        }
    }
}