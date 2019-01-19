using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DotXxlJob.Core.Json;
using DotXxlJob.Core.Model;
using Microsoft.Extensions.Logging;

namespace DotXxlJob.Core.Queue
{
    public class RetryCallbackTaskQueue:IDisposable
    {
        
        private readonly Action<HandleCallbackParam> _actionDoCallback;
        private readonly ILogger<RetryCallbackTaskQueue> _logger;

        private CancellationTokenSource _cancellation;
        private Task _runTask;
        private readonly string _backupFile;
        public RetryCallbackTaskQueue(string backupPath,Action<HandleCallbackParam> actionDoCallback,ILogger<RetryCallbackTaskQueue> logger)
        {
            
            this._actionDoCallback = actionDoCallback;
            this._logger = logger;
            this._backupFile = Path.Combine(backupPath, Constants.XxlJobRetryLogsFile);
            var dir = Path.GetDirectoryName(backupPath);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir ?? throw new Exception("logs path is empty"));
            }
            
            StartQueue();
        }

        private void StartQueue()
        {
            this._cancellation = new CancellationTokenSource();
            var stopToken = this._cancellation.Token;
            this._runTask = Task.Factory.StartNew(async () =>
            {
                while (!stopToken.IsCancellationRequested)
                {
                    await LoadFromFile();
                    await Task.Delay(Constants.CallbackRetryInterval,stopToken);
                }
              
            }, TaskCreationOptions.LongRunning);
        }

        private async Task LoadFromFile()
        {
            var list = new List<HandleCallbackParam>();

            if (!File.Exists(this._backupFile))
            {
                return;
            }

            using (StreamReader reader = new StreamReader(this._backupFile))
            {
                string nextLine;
                while ((nextLine = await reader.ReadLineAsync()) != null)
                {
                    try
                    {
                        list.Add(Utf8Json.JsonSerializer.Deserialize<HandleCallbackParam>(nextLine, ProjectDefaultResolver.Instance));
                    }
                    catch(Exception ex)
                    {
                        this._logger.LogError(ex,"read backup file  error:{error}",ex.Message);
                    }
                   
                }
            }

            if (list.Any())
            {
                foreach (var item in list)
                {
                    this._actionDoCallback(item);
                }
            }
            
        }

        public void Push(List<HandleCallbackParam> list)
        {
            if (!list.Any())
            {
                return;
            }

            try
            {
              
                using (var writer = new StreamWriter(this._backupFile, true, Encoding.UTF8))
                {
                    foreach (var item in list)
                    {
                        if (item.CallbackRetryTimes >= Constants.MaxCallbackRetryTimes)
                        {
                            this._logger.LogInformation("callback too many times and will be abandon,logId {logId}", item.LogId);
                        }
                        else
                        {
                            item.CallbackRetryTimes++;
                            byte[] buffer = Utf8Json.JsonSerializer.Serialize(item,ProjectDefaultResolver.Instance);
                            writer.WriteLine(Encoding.UTF8.GetString(buffer));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "SaveCallbackParams error.");
            }
        }

        public void Dispose()
        {
            this._cancellation.Cancel();
            this._runTask?.GetAwaiter().GetResult();
        }
    }
}