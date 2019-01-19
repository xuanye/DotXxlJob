using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using DotXxlJob.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace ASPNetCoreExecutor
{
    public class XxlJobExecutorMiddleware
    {
        private readonly IServiceProvider _provider;
        private readonly RequestDelegate _next;

        private readonly XxlRpcServiceHandler _rpcService;
        public XxlJobExecutorMiddleware(IServiceProvider provider, RequestDelegate next)
        {
            this._provider = provider;
            this._next = next;
            this._rpcService = _provider.GetRequiredService<XxlRpcServiceHandler>();
        }


        public async Task Invoke(HttpContext context)
        {

            if ("POST".Equals(context.Request.Method, StringComparison.OrdinalIgnoreCase) && 
                "application/octet-stream".Equals(context.Request.ContentType, StringComparison.OrdinalIgnoreCase))
            {
                /*
                using (Stream file = File.Create("./"+DateTime.Now.ToUnixTimeSeconds()+".data"))
                {
                    context.Request.Body.CopyTo(file);
                }
                
                return;
                */
                
                var rsp =  await _rpcService.HandlerAsync(context.Request.Body);

                context.Response.StatusCode = (int) HttpStatusCode.OK;
                context.Response.ContentType = "text/plain;utf-8";
                await context.Response.Body.WriteAsync(rsp,0,rsp.Length);
                return;
            }
            
            await _next.Invoke(context);
        }
    }
}