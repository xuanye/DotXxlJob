using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Hessian;
using DotXxlJob.Core.Config;
using DotXxlJob.Core.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DotXxlJob.Core
{
    
    /// <summary>
    /// 负责执行Http请求，序列化和反序列化并发送响应
    /// </summary>
    public class XxlRpcServiceHandler
    {
       
        private readonly JobDispatcher _jobDispatcher;
        private readonly IJobLogger _jobLogger;
        private readonly ILogger<XxlRpcServiceHandler> _logger;
        private readonly XxlJobExecutorOptions _options;

        private readonly ConcurrentDictionary<string, MethodInfo> METHOD_CACHE =
            new ConcurrentDictionary<string, MethodInfo>(); 
            
        public XxlRpcServiceHandler(IOptions<XxlJobExecutorOptions> optionsAccessor,
            JobDispatcher jobDispatcher, 
            IJobLogger jobLogger,
            ILogger<XxlRpcServiceHandler> logger)
        {
           
            this._jobDispatcher = jobDispatcher;
            this._jobLogger = jobLogger;
            this._logger = logger;
        
            this._options = optionsAccessor.Value;
            if (this._options == null)
            {
                throw  new ArgumentNullException(nameof(XxlJobExecutorOptions));
            }
            
        }
        
        /// <summary>
        /// 处理XxlRpc请求流
        /// </summary>
        /// <param name="reqStream"></param>
        /// <returns></returns>
        public async Task<byte[]> HandlerAsync(Stream reqStream)
        {
            var req = HessianSerializer.DeserializeRequest(reqStream);
            
            var res = new RpcResponse { RequestId = req.RequestId};
            
            if (!ValidRequest(req, out var error))
            {
                this._logger.LogWarning("job task request is not valid:{error}",error);
                res.ErrorMsg = error;
            }
            else
            {
                this._logger.LogDebug("receive job task ,req.RequestId={requestId},method={methodName}"
                    ,req.RequestId,req.MethodName);
                await Invoke(req, res);
                this._logger.LogDebug("completed receive job task ,req.RequestId={requestId},method={methodName},IsError={IsError}"
                    ,req.RequestId,req.MethodName,res.IsError);
            }
          
            using (var outputStream = new MemoryStream())
            {
                HessianSerializer.SerializeResponse(outputStream,res);
                return outputStream.GetBuffer();
            }
            
        }

        /// <summary>
        /// 校验请求信息
        /// </summary>
        /// <param name="req"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        private bool ValidRequest(RpcRequest req,out string error)
        {
            error = string.Empty;
            if (req == null)
            {
                error = "unknown request stream data,codec fail";
                return false;
            }
            
            if (!"com.xxl.job.core.biz.ExecutorBiz".Equals(req.ClassName)) //
            {
                error =  "not supported request!";
                return false;
            }
             
            if (DateTime.UtcNow.Subtract(req.CreateMillisTime.FromMilliseconds()) > Constants.RpcRequestExpireTimeSpan)
            {
                error =  "request is timeout!";
                return false;
            }

            if (!string.IsNullOrEmpty(this._options.AccessToken) && this._options.AccessToken !=  req.AccessToken)
            {
                error = "need authorize";
                return false;
            }
            
            return true;
        }

        /// <summary>
        /// 执行请求，获取执行函数
        /// </summary>
        /// <param name="req"></param>
        /// <param name="res"></param>
        /// <returns></returns>
        private Task Invoke(RpcRequest req, RpcResponse res)
        {
            try
            {
                var method = GetMethodInfo(req.MethodName);
                if (method == null)
                {
                    res.ErrorMsg = $"The method{req.MethodName} is not defined.";
                    this._logger.LogWarning( $"The method{req.MethodName} is not defined.");
                }
                else
                {
                    var result = method.Invoke(this, req.Parameters.ToArray());
                    
                    res.Result = result;
                }
               
            }
            catch (Exception ex)
            {
                res.ErrorMsg = ex.Message +"\n--------------\n"+ ex.StackTrace;
                this._logger.LogError(ex,"invoke method error:{0}",ex.Message);
            }

            return Task.CompletedTask;

        }

        private MethodInfo GetMethodInfo(string methodName)
        {
            if (METHOD_CACHE.TryGetValue(methodName, out var method))
            {
                return method;
            }
            
            var type = GetType();
            method = type.GetMethod( methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.IgnoreCase);
            if (method != null)
            {
                METHOD_CACHE.TryAdd(methodName, method);
            }

            return method;
        }
        
        
        #region rpc service
        
        private ReturnT Beat()
        {
            return ReturnT.SUCCESS;
        }

        private ReturnT IdleBeat(int jobId)
        {
            return this._jobDispatcher.IdleBeat(jobId);
        }

        private ReturnT Kill(int jobId)
        {
            return this._jobDispatcher.TryRemoveJobTask(jobId) ? 
                ReturnT.SUCCESS 
                : 
                ReturnT.Success("job thread already killed.");
        }

        /// <summary>
        ///  read Log
        /// </summary>
        /// <param name="logDateTime"></param>
        /// <param name="logId"></param>
        /// <param name="fromLineNum"></param>
        /// <returns></returns>
        private ReturnT Log(long logDateTime, int logId, int fromLineNum)
        {
            var ret =  ReturnT.Success(null);
            ret.Content = this._jobLogger.ReadLog(logDateTime, logId, fromLineNum);
            return ret;
        }

        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="triggerParam"></param>
        /// <returns></returns>
        private ReturnT Run(TriggerParam triggerParam)
        {
             return this._jobDispatcher.Execute(triggerParam);
        }
        #endregion
        
        
        
    }
}