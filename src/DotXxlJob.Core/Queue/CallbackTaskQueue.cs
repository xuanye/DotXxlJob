using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotXxlJob.Core.Config;
using DotXxlJob.Core.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DotXxlJob.Core.Queue
{
    public class CallbackTaskQueue:IDisposable
    {
        private readonly AdminClient _adminClient;
        private readonly IJobLogger _jobLogger;
        private readonly RetryCallbackTaskQueue _retryQueue;
        private readonly ILogger<CallbackTaskQueue> _logger;
        private readonly ConcurrentQueue<HandleCallbackParam> taskQueue = new ConcurrentQueue<HandleCallbackParam>();

        private bool _stop;

        private bool _isRunning;

        private Task _runTask;
        public CallbackTaskQueue(AdminClient adminClient,IJobLogger jobLogger,IOptions<XxlJobExecutorOptions> optionsAccessor
            , ILoggerFactory loggerFactory)
        {
            this._adminClient = adminClient;
            this._jobLogger = jobLogger;

            this._retryQueue = new RetryCallbackTaskQueue(optionsAccessor.Value.LogPath,
                Push,
                loggerFactory.CreateLogger<RetryCallbackTaskQueue>());
            
            this._logger = loggerFactory.CreateLogger<CallbackTaskQueue>();
        }
        
        public void Push(HandleCallbackParam callbackParam)
        {
            this.taskQueue.Enqueue(callbackParam);
            StartCallBack();
        }
        
        
        public void Dispose()
        {
            this._stop = true;
            this._retryQueue.Dispose();
            this._runTask?.GetAwaiter().GetResult();
        }


        private void StartCallBack()
        {
            if ( this._isRunning)
            {
                return;
            }
            
            this._runTask = Task.Run(async () =>
            {
                this._logger.LogDebug("start to callback");
                this._isRunning = true;
                while (!this._stop)
                {
                   await DoCallBack();
                }
                this._logger.LogDebug("end to callback");
                this._isRunning = false;
            });
           
        }

        private async Task DoCallBack()
        {
            List<HandleCallbackParam> list = new List<HandleCallbackParam>();
            
            while (list.Count < Constants.MaxCallbackRecordsPerRequest && this.taskQueue.TryDequeue(out var item))
            {
                list.Add(item);
            }

            if (!list.Any())
            {
                return;
            }

            ReturnT result; 
            try
            {
                result = await _adminClient.Callback(list);
            }
            catch (Exception ex){
                this._logger.LogError(ex,"trigger callback error:{error}",ex.Message);
                result = ReturnT.Failed(ex.Message);
                this._retryQueue.Push(list);
            }

            LogCallBackResult(result, list);
        }

        private void LogCallBackResult(ReturnT result,List<HandleCallbackParam> list)
        {
            foreach (var param in list)
            {
                this._jobLogger.LogSpecialFile(param.LogDateTime, param.LogId, result.Msg??"Success");
            }
        }

      
    }
}