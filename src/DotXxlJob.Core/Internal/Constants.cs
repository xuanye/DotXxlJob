using System;

namespace DotXxlJob.Core
{
    internal static class Constants
    {
        public const string RpcRequestJavaFullName = "com.xxl.rpc.remoting.net.params.XxlRpcRequest";
        public const string RpcResponseJavaFullName = "com.xxl.rpc.remoting.net.params.XxlRpcResponse";
        
        public const string XxlLogsDefaultRootDirectory = "xxl-job-logs";
        public const string HandleLogsDirectory = "HandlerLogs";
        public const string LogFileNameCallContextKey = "XxlJob.LogFileName";
        public const int DefaultLogRetentionDays = 30;

        public static TimeSpan RpcRequestExpireTimeSpan = TimeSpan.FromMinutes(3);

        public const int MaxCallbackRetryTimes = 10;
        //每次回调最多发送几条记录
        public const int MaxCallbackRecordsPerRequest = 20;
        public static TimeSpan CallbackRetryInterval = TimeSpan.FromSeconds(600);

        //Admin集群机器请求默认超时时间
        public static TimeSpan AdminServerDefaultTimeout = TimeSpan.FromSeconds(15);
        //Admin集群中的某台机器熔断后间隔多长时间再重试
        public static TimeSpan AdminServerReconnectInterval = TimeSpan.FromMinutes(3);
        //Admin集群中的某台机器请求失败多少次后熔断
        public const int AdminServerCircuitFailedTimes = 3;

 

        public static class GlueType
        {
            public const string BEAN = "BEAN";
        }

        public static class ExecutorBlockStrategy
        {
            public const string SERIAL_EXECUTION = "SERIAL_EXECUTION";

            public const string DISCARD_LATER = "DISCARD_LATER";

            public const string COVER_EARLY = "COVER_EARLY";
        }
    }
}