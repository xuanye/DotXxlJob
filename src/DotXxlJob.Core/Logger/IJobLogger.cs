using System;
using DotXxlJob.Core.Model;

namespace DotXxlJob.Core
{
    public interface IJobLogger
    {

        void SetLogFile(long logTime, int logId);
        
        void Log(string pattern, params object[] format);


        void LogError(Exception ex);


        LogResult ReadLog(long logTime, int logId, int fromLine);

        
        void LogSpecialFile(long logTime, int logId, string pattern, params object[] format);

    }
}