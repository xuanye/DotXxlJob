using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DotXxlJob.Core.Config;
using DotXxlJob.Core.Model;
using Hessian;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DotXxlJob.Core
{
    public class JobLogger:IJobLogger
    {
        private readonly ILogger<JobLogger> _logger;
        
        private readonly AsyncLocal<string> LogFileName = new AsyncLocal<string>();

        private readonly XxlJobExecutorOptions _options;
        public JobLogger(IOptions<XxlJobExecutorOptions> optionsAccessor,ILogger<JobLogger> logger)
        {
            this._logger = logger;
            this._options = optionsAccessor.Value;
        }

        public void SetLogFile(long logTime, int logId)
        {
            try
            {
                var filePath = MakeLogFileName(logTime, logId);
                var dir = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                    CleanOldLogs();
                }
                LogFileName.Value = filePath;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SetLogFileName error.");
            }
        }
        

        public void Log(string pattern, params object[] format)
        {
            var appendLog = string.Format(pattern, format);
            var callInfo = new StackTrace(true).GetFrame(1);
            LogDetail(GetLogFileName(), callInfo, appendLog);
        }
        
        public void LogError(Exception ex)
        {
            var callInfo = new StackTrace(true).GetFrame(1);
            LogDetail(GetLogFileName(), callInfo, ex.Message + ex.StackTrace);
        }

        public LogResult ReadLog(long logTime, int logId, int fromLine)
        {
            var filePath = MakeLogFileName(logTime, logId);
            if (string.IsNullOrEmpty(filePath))
            {
                return new LogResult(fromLine, 0, "readLog fail, logFile not found", true);
            }
            if (!File.Exists(filePath))
            {
                return new LogResult(fromLine, 0, "readLog fail, logFile not exists", true);
            }

            // read file
            var logContentBuffer = new StringBuilder();
            int toLineNum = 0;
            try
            {
                using (var reader = new StreamReader(filePath, Encoding.UTF8))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        toLineNum++;
                        if (toLineNum >= fromLine)
                        {
                            logContentBuffer.AppendLine(line);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ReadLog error.");
            }

            // result
            var logResult = new LogResult(fromLine, toLineNum, logContentBuffer.ToString(), false);
            return logResult;
        }

        public void LogSpecialFile(long logTime, int logId, string pattern, params object[] format)
        {
            var filePath = MakeLogFileName(logTime, logId);
            var callInfo = new StackTrace(true).GetFrame(1);
            var content = string.Format(pattern, format);
            LogDetail(filePath, callInfo, content);
        }
        
        
        private string GetLogFileName()
        {
            return LogFileName.Value;
        }
        private string MakeLogFileName(long logDateTime, int logId)
        {
            //log fileName like: logPath/HandlerLogs/yyyy-MM-dd/9999.log
            return Path.Combine(_options.LogPath, Constants.HandleLogsDirectory,
                logDateTime.FromMilliseconds().ToString("yyyy-MM-dd"), $"{logId}.log");
        }
        private void LogDetail(string logFileName, StackFrame callInfo, string appendLog)
        {
            if (string.IsNullOrEmpty(logFileName))
            {
                return;
            }

            var stringBuffer = new StringBuilder();
            stringBuffer
                .Append(DateTime.Now.ToString("s")).Append(" ")
                .Append("[" + callInfo.GetMethod().DeclaringType.FullName + "#" + callInfo.GetMethod().Name + "]").Append("-")
                .Append("[line " + callInfo.GetFileLineNumber() + "]").Append("-")
                .Append("[thread " + Thread.CurrentThread.ManagedThreadId + "]").Append(" ")
                .Append(appendLog ?? string.Empty)
                .AppendLine();
            
            var formatAppendLog = stringBuffer.ToString();

            try
            {
                File.AppendAllText(logFileName, formatAppendLog, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "LogDetail error");
            }
        }
        
        private void CleanOldLogs()
        {
            if (_options.LogRetentionDays <= 0)
            {
                return;
            }

            Task.Run(() =>
            {
                try
                {
                    var handlerLogsDir = new DirectoryInfo(Path.Combine(_options.LogPath, Constants.HandleLogsDirectory));
                    if (!handlerLogsDir.Exists)
                    {
                        return;
                    }

                    var today = DateTime.UtcNow.Date;
                    foreach (var dir in handlerLogsDir.GetDirectories())
                    {
                        if (DateTime.TryParse(dir.Name, out var dirDate))
                        {
                            if (today.Subtract(dirDate.Date).Days > _options.LogRetentionDays)
                            {
                                dir.Delete(true);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "CleanOldLogs error.");
                }
            });
        }

    }
}