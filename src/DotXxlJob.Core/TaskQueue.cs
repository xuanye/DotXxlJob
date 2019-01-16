using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using DotXxlJob.Core.Model;
using Microsoft.Extensions.Logging;

namespace DotXxlJob.Core
{
    public class JobQueue
    {
        private readonly ITaskExecutor _executor;
        private readonly CallbackTaskQueue _callbackTaskQueue;
        private readonly ILogger<JobQueue> _logger;
        private readonly ConcurrentQueue<TriggerParam> TASK_QUEUE = new ConcurrentQueue<TriggerParam>();
        public JobQueue(ITaskExecutor executor,CallbackTaskQueue callbackTaskQueue,ILogger<JobQueue> logger)
        {
            _executor = executor;
            _callbackTaskQueue = callbackTaskQueue;
            _logger = logger;
        }

        public ITaskExecutor Executor => this._executor;


        private CancellationTokenSource _cancellationTokenSource;

       /// <summary>
       /// 覆盖之前的队列
       /// </summary>
       /// <param name="triggerParam"></param>
       /// <returns></returns>
       public ReturnT Replace(TriggerParam triggerParam)
       {
            Stop();
            while (!TASK_QUEUE.IsEmpty)
            {
                TASK_QUEUE.TryDequeue(out _);
            }

            return Push(triggerParam);
       }

       public ReturnT Push(TriggerParam triggerParam)
       {
           this.TASK_QUEUE.Enqueue(triggerParam);
           StartTask();
           return ReturnT.SUCCESS;
       }

       public void Stop()
       {
           this._cancellationTokenSource?.Cancel();
           this._cancellationTokenSource?.Dispose();
           this._cancellationTokenSource = null;
           
       }


       private void StartTask()
       {
           if (this._cancellationTokenSource != null )
           {
               return; //running
           }
           this._cancellationTokenSource =new CancellationTokenSource();
           CancellationToken ct = _cancellationTokenSource.Token;
           
           Task.Factory.StartNew(async () =>
           {
               
               //ct.ThrowIfCancellationRequested();
               
               while (!ct.IsCancellationRequested)
               {
                   if (TASK_QUEUE.IsEmpty)
                   {
                       break;
                   }

                   ReturnT result = null;
                   TriggerParam triggerParam = null;
                   try
                   {
                      
                       if (TASK_QUEUE.TryDequeue(out triggerParam))
                       {
                          result = await this._executor.Execute(triggerParam);
                       }
                       else
                       {
                           this._logger.LogWarning("Dequeue Task Failed");
                       }
                   }
                   catch (Exception ex)
                   {
                       result = ReturnT.Failed("Dequeue Task Failed:"+ex.Message);
                   }
                 
                   if(triggerParam !=null)
                   {
                       this._callbackTaskQueue.Push(new CallbackParam(triggerParam, result));
                   }
                  
               }

              
               this._cancellationTokenSource.Dispose();
               this._cancellationTokenSource = null;

           }, this._cancellationTokenSource.Token);
           
          
       }
    }
}