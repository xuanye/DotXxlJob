// Copyright (c) Xuanye Wang. All rights reserved.
// Licensed under MIT license

using System.Threading;
using System.Threading.Tasks;
using DotXxlJob.Core.Models;

namespace DotXxlJob.Core
{
    public interface IJobExecutor
    {
        string GlueType { get; }

        Task<TaskResult> ExecuteAsync(JobExecuteContext context, Job job, CancellationToken cancellationToken);
    }
}
