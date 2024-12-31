// Copyright (c) Xuanye Wang. All rights reserved.
// Licensed under MIT license

using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using DotXxlJob.Core.Models;
using ErrorOr;

namespace DotXxlJob.Core.Internal
{
    internal class JobWorker : IAsyncDisposable
    {
        private readonly IJobExecutorFactory _executorFactory;

        private readonly ConcurrentDictionary<JobId, JobQueue> _jobsQueue = new ConcurrentDictionary<JobId, JobQueue>();

        public JobWorker(IJobExecutorFactory executorFactory)
        {
            _executorFactory = executorFactory;
        }

        public event EventHandler<JobCompletedArgs>? JobCompleted;


        private void OnJobTaskCompleted(object? sender, JobCompletedArgs args)
        {
            JobCompleted?.Invoke(this, args);
        }

        public async Task<ErrorOr<TaskResult>> EnqueueJobAsync(Job job)
        {
            var executor = _executorFactory.GetJobExecutor(job.GlueType);

            if (executor == null)
            {
                return TaskResult.Failure("Executor not found");
            }
            //new job serial 
            if (!_jobsQueue.TryGetValue(job.JobId, out var queue))
            {
                queue = new JobQueue(executor);
                queue.OnJobTaskCompleted += OnJobTaskCompleted;
                queue.EnqueueJob(job);

                if (_jobsQueue.TryAdd(job.JobId, queue))
                {
                    return TaskResult.Success();
                }

                return TaskResult.Failure("add running queue executor error");
            }

            //change executor
            if (queue.Executor != executor)
            {
                queue.SetExecutor(executor);
                queue.EnqueueJob(job);
                return TaskResult.Success();
            }
            //丢弃后续的
            if (job.ExecutorBlockStrategy == ExecutorBlockStrategy.DISCARD_LATER.ToString())
            {
                //存在还没执行完成的任务
                if (queue.IsRunning())
                {
                    return TaskResult.Failure($"block strategy effect：{job.ExecutorBlockStrategy}");
                }
            }
            else if (job.ExecutorBlockStrategy == ExecutorBlockStrategy.COVER_EARLY.ToString())  //覆盖较早的
            {
                return queue.Replace(job);
            }

            queue.EnqueueJob(job);
            return TaskResult.Success();

        }



        public Task ReplaceJobAsync(Job job)
        {
            throw new NotImplementedException();
        }

        public Task AbortJobAsync(JobId jobId)
        {
            throw new NotImplementedException();
        }

        public ValueTask DisposeAsync()
        {
            throw new NotImplementedException();
        }
    }
}