using System;
using System.Collections.Concurrent;
using DotXxlJob.Core.Model;
using DotXxlJob.Core.Queue;
using Microsoft.Extensions.Logging;

namespace DotXxlJob.Core
{
    /// <summary>
    /// 负责实际的JOB轮询
    /// </summary>
    public class JobDispatcher
    {
        private readonly TaskExecutorFactory _executorFactory;
        private readonly CallbackTaskQueue _callbackTaskQueue;
        private readonly IJobLogger _jobLogger;

        private readonly ConcurrentDictionary<int,JobTaskQueue> RUNNING_QUEUE = new ConcurrentDictionary<int, JobTaskQueue>();


        private readonly ILogger<JobTaskQueue> _jobQueueLogger;
        private readonly ILogger<JobDispatcher> _logger;
        public JobDispatcher(
            TaskExecutorFactory executorFactory,
            CallbackTaskQueue callbackTaskQueue,
            IJobLogger jobLogger,
            ILoggerFactory loggerFactory
            )
        {
            this. _executorFactory = executorFactory;
            this. _callbackTaskQueue = callbackTaskQueue;
            this._jobLogger = jobLogger;


            this._jobQueueLogger =  loggerFactory.CreateLogger<JobTaskQueue>();
            this._logger =  loggerFactory.CreateLogger<JobDispatcher>();
        }
    
     
        /// <summary>
        /// 尝试移除JobTask
        /// </summary>
        /// <param name="jobId"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        public bool TryRemoveJobTask(int jobId)
        {
            if (RUNNING_QUEUE.TryGetValue(jobId, out var jobQueue))
            {
                jobQueue.Stop();
                return true;
            }
            return false;
        }

        /// <summary>
        /// 执行队列，并快速返回结果
        /// </summary>
        /// <param name="triggerParam"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public ReturnT Execute(TriggerParam triggerParam)
        {

            var executor = this._executorFactory.GetTaskExecutor(triggerParam.GlueType);
            if (executor == null)
            {
                return ReturnT.Failed($"glueType[{triggerParam.GlueType}] is not supported ");
            }
            
            // 1. 根据JobId 获取 TaskQueue; 用于判断是否有正在执行的任务
            if (RUNNING_QUEUE.TryGetValue(triggerParam.JobId, out var taskQueue))
            {
                if (taskQueue.Executor != executor) //任务执行器变更
                {
                    return ChangeJobQueue(triggerParam, executor);
                }
            }

            if (taskQueue != null) //旧任务还在执行，判断执行策略
            {
                //丢弃后续的
                if (Constants.ExecutorBlockStrategy.DISCARD_LATER == triggerParam.ExecutorBlockStrategy)
                {
                    if (taskQueue.IsRunning())
                    {
                        return ReturnT.Failed($"block strategy effect：{triggerParam.ExecutorBlockStrategy}");
                    }
                    
                }
                //覆盖较早的
                if (Constants.ExecutorBlockStrategy.COVER_EARLY == triggerParam.ExecutorBlockStrategy)
                {
                    return taskQueue.Replace(triggerParam);
                }
            }
            
            return PushJobQueue(triggerParam, executor);
           
        }


        /// <summary>
        /// IdleBeat
        /// </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        public ReturnT IdleBeat(int jobId)
        {
            return RUNNING_QUEUE.ContainsKey(jobId) ? 
                new ReturnT(ReturnT.FAIL_CODE, "job thread is running or has trigger queue.") 
                : ReturnT.SUCCESS;
        }

        private void TriggerCallback(object sender, HandleCallbackParam callbackParam)
        {
            this._callbackTaskQueue.Push(callbackParam);
        }
      
        private ReturnT PushJobQueue(TriggerParam triggerParam, ITaskExecutor executor)
        { 
            
            if (RUNNING_QUEUE.TryGetValue(triggerParam.JobId,out var jobQueue))
            {
                return jobQueue.Push(triggerParam);
            }
            
            //NewJobId
            jobQueue = new JobTaskQueue ( executor,this._jobLogger, this._jobQueueLogger);
            jobQueue.CallBack += TriggerCallback;
            if (RUNNING_QUEUE.TryAdd(triggerParam.JobId, jobQueue))
            {
                return jobQueue.Push(triggerParam);
            }
            return ReturnT.Failed("add running queue executor error");
        }
        
        private ReturnT ChangeJobQueue(TriggerParam triggerParam, ITaskExecutor executor)
        {
           
            if (RUNNING_QUEUE.TryRemove(triggerParam.JobId, out var oldJobTask))
            { 
                oldJobTask.CallBack -= TriggerCallback;
                oldJobTask.Dispose(); //释放原来的资源
            }
            
            JobTaskQueue jobQueue = new JobTaskQueue ( executor,this._jobLogger, this._jobQueueLogger);
            jobQueue.CallBack += TriggerCallback;
            if (RUNNING_QUEUE.TryAdd(triggerParam.JobId, jobQueue))
            {
                return jobQueue.Push(triggerParam);
            }
            return ReturnT.Failed(" replace running queue executor error");
        }

    }
}