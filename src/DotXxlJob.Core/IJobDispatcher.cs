// Copyright (c) Xuanye Wang. All rights reserved.
// Licensed under MIT license

using System.Threading;
using System.Threading.Tasks;
using DotXxlJob.Core.Models;

namespace DotXxlJob.Core
{
    public interface IJobDispatcher
    {
        Task StartAsync(CancellationToken cancellationToken);
        Task StopAsync(CancellationToken cancellationToken);

        Task AddJobAsync(Job job);
        Task ReplaceJobAsync(Job job);
        Task AbortJobAsync(JobId jobId);
    }
}
