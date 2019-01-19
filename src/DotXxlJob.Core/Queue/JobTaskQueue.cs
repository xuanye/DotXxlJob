using System;
using System.Collections.Concurrent;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;
using DotXxlJob.Core.Model;
using Microsoft.Extensions.Logging;

namespace DotXxlJob.Core
{
    public class JobTaskQueue:IDisposable
    {
        private readonly ITaskExecutor _executor;
        private readonly IJobLogger _jobLogger;
        private readonly ILogger<JobTaskQueue> _logger;
        private readonly ConcurrentQueue<TriggerParam> TASK_QUEUE = new ConcurrentQueue<TriggerParam>();
        private readonly ConcurrentDictionary<int, byte> ID_IN_QUEUE = new ConcurrentDictionary<int, byte>();
        private CancellationTokenSource _cancellationTokenSource;
        private Task _runTask;
        public JobTaskQueue(ITaskExecutor executor,IJobLogger jobLogger,ILogger<JobTaskQueue> logger)
        {
            this._executor = executor;
            this._jobLogger = jobLogger;
            this._logger = logger;
        }

        public ITaskExecutor Executor => this._executor;


        public event EventHandler<HandleCallbackParam> CallBack;
        
       

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
            ID_IN_QUEUE.Clear();

            return Push(triggerParam);
       }

       public ReturnT Push(TriggerParam triggerParam)
       {
           if(!ID_IN_QUEUE.TryAdd(triggerParam.LogId,0))
           {
               this._logger.LogWarning("repeat job task,logId={logId},jobId={jobId}",triggerParam.LogId,triggerParam.JobId);
               return ReturnT.Failed("repeat job task!");
           }

           //this._logger.LogWarning("add job with logId={logId},jobId={jobId}",triggerParam.LogId,triggerParam.JobId);

           this.TASK_QUEUE.Enqueue(triggerParam);
           StartTask();
           return ReturnT.SUCCESS;
       }

       public void Stop()
       {
           this._cancellationTokenSource?.Cancel();
           this._cancellationTokenSource?.Dispose();
           this._cancellationTokenSource = null;
           
           //wait for task completed
           this._runTask?.GetAwaiter().GetResult();
       }

       public void Dispose()
       {
           Stop();
           while (!TASK_QUEUE.IsEmpty)
           {
               TASK_QUEUE.TryDequeue(out _);
           }
           ID_IN_QUEUE.Clear();
       }

       private void StartTask()
       {
           if (this._cancellationTokenSource != null )
           {
               return; //running
           }
           this._cancellationTokenSource = new CancellationTokenSource();
           var ct = this._cancellationTokenSource.Token;
           
           this._runTask = Task.Factory.StartNew(async () =>
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
                           if (!ID_IN_QUEUE.TryRemove(triggerParam.LogId,out _))
                           {
                               this._logger.LogWarning("remove queue failed,logId={logId},jobId={jobId},exists={exists}"
                                   ,triggerParam.LogId,triggerParam.JobId,ID_IN_QUEUE.ContainsKey(triggerParam.LogId));
                           }
                           //set log file;
                           this._jobLogger.SetLogFile(triggerParam.LogDateTime,triggerParam.LogId);
                           
                           this._jobLogger.Log("<br>----------- xxl-job job execute start -----------<br>----------- Param:{0}" ,triggerParam.ExecutorParams);
                           
                           result = await this._executor.Execute(triggerParam);
                           
                           this._jobLogger.Log("<br>----------- xxl-job job execute end(finish) -----------<br>----------- ReturnT:" + result.Code);
                       }
                       else
                       {
                           this._logger.LogWarning("Dequeue Task Failed");
                       }
                   }
                   catch (Exception ex)
                   {
                       result = ReturnT.Failed("Dequeue Task Failed:"+ex.Message);
                       this._jobLogger.Log("<br>----------- JobThread Exception:" + ex.Message + "<br>----------- xxl-job job execute end(error) -----------");
                   }
                 
                   if(triggerParam !=null)
                   {
                       CallBack?.Invoke(this,new HandleCallbackParam(triggerParam, result??ReturnT.FAIL));
                   }
                  
               }

              
               this._cancellationTokenSource.Dispose();
               this._cancellationTokenSource = null;

           }, this._cancellationTokenSource.Token);
           
          
       }
    }
}