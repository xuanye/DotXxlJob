// Copyright (c) Xuanye Wang. All rights reserved.
// Licensed under MIT license

using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using DotXxlJob.Core.Models;

namespace DotXxlJob.Core.Internal
{
    internal class JobQueue
    {
        private readonly ConcurrentQueue<Job> _jobs = new ConcurrentQueue<Job>();
        private readonly IJobLogger _jobLogger;
        private CancellationTokenSource? _cancellationTokenSource;
        private Task? _runningTask;

        public JobQueue(IJobExecutor executor, IJobLogger jobLogger)
        {
            Executor = executor;
            _jobLogger = jobLogger;
        }

        public IJobExecutor Executor { get; private set; } = null!;

        public event EventHandler<JobCompletedArgs>? OnJobTaskCompleted;

        public async Task SetExecutor(IJobExecutor executor)
        {
            Executor = executor;
            //TODO: cancel all pending jobs
            await Stop();

            _jobs.Clear();
        }

        public void EnqueueJob(Job job)
        {
            _jobs.Enqueue(job);
            _runningTask = Start();
        }

        public bool IsRunning()
        {
            return _cancellationTokenSource != null && !_cancellationTokenSource.IsCancellationRequested;
        }

        public async Task Replace(Job job)
        {
            _jobs.Clear();
            await Stop();
            EnqueueJob(job);
        }

        private Task Start()
        {
            if (_cancellationTokenSource != null)
            {
                return _runningTask!;
            }
            _cancellationTokenSource = new CancellationTokenSource();
            var ct = _cancellationTokenSource.Token;
            return Task.Factory.StartNew(async () =>
            {

                while (!ct.IsCancellationRequested)
                {
                    if (_jobs.IsEmpty)
                    {
                        //_logger.LogInformation("task queue is empty!");
                        break;
                    }

                    TaskResult? result = null;
                    Job? jobTask = null;
                    try
                    {

                        if (_jobs.TryDequeue(out jobTask))
                        {

                            //TODO: set Logger;
                            //_jobLogger.SetLogFile(jobTask.LogDateTime, jobTask.LogId);
                            //_jobLogger.Log("<br>----------- xxl-job job execute start -----------<br>----------- Param:{0}", jobTask.ExecutorParams);

                            var exectorToken = ct;
                            CancellationTokenSource? timeoutCts = null;
                            if (jobTask.ExecutorTimeout > 0)
                            {
                                timeoutCts = new CancellationTokenSource(jobTask.ExecutorTimeout * 1000);
                                exectorToken = CancellationTokenSource.CreateLinkedTokenSource(exectorToken, timeoutCts.Token).Token;
                            }

                            result = await Executor.ExecuteAsync(new JobExecuteContext(_jobLogger) { }, jobTask!, exectorToken);

                            if (timeoutCts != null && timeoutCts.IsCancellationRequested)
                            {
                                result = TaskResult.Timeout();
                                timeoutCts.Dispose();
                                timeoutCts = null;
                            }

                            //_jobLogger.Log("<br>----------- xxl-job job execute end(finish) -----------<br>----------- ReturnT:" + result.Code);
                        }
                    }
                    catch (Exception ex)
                    {
                        result = TaskResult.Failure("Dequeue Task Failed:" + ex.Message);
                        //_jobLogger.Log("<br>----------- JobThread Exception:" + ex.Message + "<br>----------- xxl-job job execute end(error) -----------");
                    }

                    if (jobTask != null)
                    {
                        OnJobTaskCompleted?.Invoke(this, new JobCompletedArgs() { Job = jobTask, TaskResult = result! });
                    }

                }

                _cancellationTokenSource?.Dispose();
                _cancellationTokenSource = null;
            }, _cancellationTokenSource.Token);

        }
        private Task Stop()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = null;

            return _runningTask!;
        }
    }
}
