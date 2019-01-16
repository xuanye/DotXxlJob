using System;
using System.Collections.Concurrent;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using DotXxlJob.Core.Config;
using DotXxlJob.Core.Model;
using Hessian.Net;
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
        private readonly ILogger<XxlRpcServiceHandler> _logger;
        private readonly DataContractHessianSerializer _reqSerializer;
        private readonly DataContractHessianSerializer _resSerializer;
        private readonly XxlJobExecutorOptions _options;

        private readonly ConcurrentDictionary<string, MethodInfo> METHOD_CACHE =
            new ConcurrentDictionary<string, MethodInfo>(); 
            
        public XxlRpcServiceHandler(IOptions<XxlJobExecutorOptions> optionsAccessor,
            JobDispatcher jobDispatcher, 
            ILogger<XxlRpcServiceHandler> logger)
        {
           
            _jobDispatcher = jobDispatcher;
            this._logger = logger;
            this._reqSerializer = new DataContractHessianSerializer(typeof (RpcRequest));
            this._resSerializer = new DataContractHessianSerializer(typeof (RpcResponse));
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
        /// <exception cref="NotImplementedException"></exception>
        public async Task<byte[]> HandlerAsync(Stream reqStream)
        {
            using (Stream output = File.OpenWrite(DateTime.Now.ToUnixTimeSeconds()+".dat"))
            {
                reqStream.CopyTo(output);
            }
            
            var req = _reqSerializer.ReadObject(reqStream) as RpcRequest;
            var res = new RpcResponse();
            if (!ValidRequest(req, out var error))
            {
                res.ErrorMsg = error;
            }
            else
            {
                await Invoke(req, res);
            }
          
            using (var outputStream = new MemoryStream())
            {
                _resSerializer.WriteObject(outputStream,res);
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
             
            if (DateTime.UtcNow.Subtract(req.CreateMillisTime.FromUnixTimeMilliseconds()) > Constants.RpcRequestExpireTimeSpan)
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
                }
                else
                {
                    var result = method.Invoke(this, req.Parameters.ToArray());
                    
                    res.Result = result;
                }
               
            }
            catch (Exception ex)
            {
                res.ErrorMsg = ex.ToString();
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
        /// TODO:获取执行日志
        /// </summary>
        /// <param name="logDateTime"></param>
        /// <param name="logId"></param>
        /// <param name="fromLineNum"></param>
        /// <returns></returns>
        private ReturnT Log(long logDateTime, int logId, int fromLineNum)
        {
            //var logResult = JobLogger.ReadLog(logDateTime, logId, fromLineNum);
            return ReturnT.Success(null);
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