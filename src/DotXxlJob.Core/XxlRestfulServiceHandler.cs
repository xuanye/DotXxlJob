using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Cache;
using System.Text;
using System.Threading.Tasks;
using DotXxlJob.Core.Config;
using DotXxlJob.Core.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace DotXxlJob.Core
{
    public class XxlRestfulServiceHandler
    {
        private readonly JobDispatcher _jobDispatcher;
        private readonly IJobLogger _jobLogger;
        private readonly ILogger<XxlRestfulServiceHandler> _logger;
        private readonly XxlJobExecutorOptions _options;

        public XxlRestfulServiceHandler(IOptions<XxlJobExecutorOptions> optionsAccessor,
           JobDispatcher jobDispatcher,
           IJobLogger jobLogger,
           ILogger<XxlRestfulServiceHandler> logger)
        {

            this._jobDispatcher = jobDispatcher;
            this._jobLogger = jobLogger;
            this._logger = logger;

            this._options = optionsAccessor.Value;
            if (this._options == null)
            {
                throw new ArgumentNullException(nameof(XxlJobExecutorOptions));
            }

        }

        public async Task HandlerAsync(HttpRequest request,HttpResponse response)
        {
           

            var path = request.Path.Value ;

            ReturnT ret = null;
            var arrParts = path.Split('/');
            var method = arrParts[arrParts.Length - 1].ToLower();
                     
            if (!string.IsNullOrEmpty(this._options.AccessToken))
            {
                var reqToken = "";
                if (request.Headers.TryGetValue("XXL-JOB-ACCESS-TOKEN", out var tokenValues))
                {
                    reqToken = tokenValues[0].ToString();
                }
                if(this._options.AccessToken != reqToken)
                {
                    ret = ReturnT.Failed("ACCESS-TOKEN Auth Fail");
                    response.ContentType = "application/json;charset=utf-8";
                    await response.WriteAsync(JsonConvert.SerializeObject(ret));
                    return;
                }
            }
            try
            {
                string json = await CollectBody(request.Body);
                switch (method)
                {
                    case "beat":
                        ret = Beat();
                        break;
                    case "idlebeat":
                        ret = IdleBeat(JsonConvert.DeserializeObject<IdleBeatRequest>(json));
                        break;
                    case "run":
                        ret = Run(JsonConvert.DeserializeObject<TriggerParam>(json));
                        break;
                    case "kill":
                        ret = Kill(JsonConvert.DeserializeObject<KillRequest>(json));
                        break;
                    case "log":
                        ret = Log(JsonConvert.DeserializeObject<LogRequest>(json));
                        break;
                }
            }
            catch(Exception ex)
            {
                this._logger.LogError(ex,"响应出错"+ ex.Message);
                ret = ReturnT.Failed("执行器内部错误");
            }
           
          
            if(ret == null)
            {
                ret = ReturnT.Failed($"method {method}  is not impl");
            }
            response.ContentType = "application/json;charset=utf-8";
            await response.WriteAsync(JsonConvert.SerializeObject(ret, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii }));           
        }
        private async Task<string> CollectBody(Stream body)
        {
            string bodyText;
            using (var reader = new StreamReader(body))
            {
                bodyText = await reader.ReadToEndAsync();
            }
            return bodyText;
        }

        #region rpc service

        private ReturnT Beat()
        {
            return ReturnT.SUCCESS;
        }

        private ReturnT IdleBeat(IdleBeatRequest req)
        {
            if(req == null)
            {
                return ReturnT.Failed("IdleBeat Error");
            }
            return this._jobDispatcher.IdleBeat(req.JobId);
        }

        private ReturnT Kill(KillRequest req)
        {
            if (req == null)
            {
                return ReturnT.Failed("Kill Error");
            }
            return this._jobDispatcher.TryRemoveJobTask(req.JobId) ?
                ReturnT.SUCCESS
                :
                ReturnT.Success("job thread already killed.");
        }

        /// <summary>
        ///  read Log
        /// </summary>     
        /// <returns></returns>
        private ReturnT Log(LogRequest req)
        {
            if (req == null)
            {
                return ReturnT.Failed("Log Error");
            }
            var ret = ReturnT.Success(null);
            ret.Content = this._jobLogger.ReadLog(req.LogDateTime,req.LogId, req.FromLineNum);
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
