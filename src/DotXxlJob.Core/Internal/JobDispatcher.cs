// Copyright (c) Xuanye Wang. All rights reserved.
// Licensed under MIT license

using System;
using System.Threading;
using System.Threading.Tasks;
using DotXxlJob.Core.Models;
using Microsoft.Extensions.DependencyInjection;

namespace DotXxlJob.Core.Internal
{
    public class JobDispatcher : IJobDispatcher
    {
        private readonly JobWorker _jobWorker;
        private readonly CallbackWorker _callbackWorker;
        public JobDispatcher(IServiceProvider provider)
        {
            _jobWorker = ActivatorUtilities.CreateInstance<JobWorker>(provider);
            _jobWorker.JobCompleted += _jobWorker_JobCompleted;

            _callbackWorker = ActivatorUtilities.CreateInstance<CallbackWorker>(provider);
        }

        private void _jobWorker_JobCompleted(object? sender, JobCompletedArgs e)
        {
            //TODO:add job callback task
            throw new NotImplementedException();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            var t1 = _jobWorker.DisposeAsync();
            var t2 = _callbackWorker.DisposeAsync();
            await Task.WhenAll(t1.AsTask(), t2.AsTask());

            _jobWorker.JobCompleted -= _jobWorker_JobCompleted;
        }

        public Task AddJobAsync(Job job)
        {
            return _jobWorker.EnqueueJobAsync(job);
        }
        public Task ReplaceJobAsync(Job job)
        {
            return _jobWorker.ReplaceJobAsync(job);
        }
        public Task AbortJobAsync(JobId jobId)
        {
            return _jobWorker.AbortJobAsync(jobId);
        }

    }
}
