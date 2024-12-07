// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using DotXxlJob.Core.Models;
using System.Threading;
using System.Threading.Tasks;

namespace DotXxlJob.Core
{
    public interface ICommandExecutor
    {
        string CommandName { get; }
        Task<ExecutorResult> ExecuteAsync(byte[] payload,CancellationToken cancellationToken);
    }

    
}