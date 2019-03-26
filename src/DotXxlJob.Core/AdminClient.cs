using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using DotXxlJob.Core.Config;
using DotXxlJob.Core.Model;
using Hessian;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DotXxlJob.Core
{
    public class AdminClient
    {
        private static readonly string MAPPING = "/api";
        private readonly XxlJobExecutorOptions _options;
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<AdminClient> _logger;

        private List<AddressEntry> _addresses;

        private int _currentIndex;

        public AdminClient(IOptions<XxlJobExecutorOptions> optionsAccessor
            , IHttpClientFactory clientFactory
            , ILogger<AdminClient> logger)
        {
            Preconditions.CheckNotNull(optionsAccessor?.Value, "XxlJobExecutorOptions");

            this._options = optionsAccessor?.Value;
            this._clientFactory = clientFactory;
            this._logger = logger;
            InitAddress();
        }

        private void InitAddress()
        {
            this._addresses = new List<AddressEntry>();
            foreach (var item in this._options.AdminAddresses.Split(';'))
            {
                try
                {
                    var uri = new Uri(item + MAPPING);
                    var entry = new AddressEntry { RequestUri = uri };
                    this._addresses.Add(entry);
                }
                catch (Exception ex)
                {
                    this._logger.LogError(ex, "init admin address error.");
                }
            }
        }

        public Task<ReturnT> Callback(List<HandleCallbackParam> callbackParamList)
        {
            return InvokeRpcService("callback", new List<object> { new JavaClass { Name = Constants.JavaListFulName } }, callbackParamList);
        }

        public Task<ReturnT> Registry(RegistryParam registryParam)
        {
            return InvokeRpcService("registry", new List<object> { new JavaClass { Name = "com.xxl.job.core.biz.model.RegistryParam" } }, registryParam, true);
        }

        public Task<ReturnT> RegistryRemove(RegistryParam registryParam)
        {
            return InvokeRpcService("registryRemove", new List<object> { new JavaClass { Name = "com.xxl.job.core.biz.model.RegistryParam" } }, registryParam, true);
        }

        private async Task<ReturnT> InvokeRpcService(string methodName, List<object> parameterTypes,
            object parameters, bool polling = false)
        {
            var request = new RpcRequest {
                RequestId = Guid.NewGuid().ToString("N"),
                CreateMillisTime = DateTime.Now.GetTotalMilliseconds(),
                AccessToken = _options.AccessToken,
                ClassName = "com.xxl.job.core.biz.AdminBiz",
                MethodName = methodName,
                ParameterTypes = parameterTypes,
                Parameters = new List<object> { parameters }
            };
            byte[] postBuf;
            using (var stream = new MemoryStream())
            {
                HessianSerializer.SerializeRequest(stream, request);

                postBuf = stream.ToArray();
            }

            var triedTimes = 0;
            var retList = new List<ReturnT>();

            using (var client = this._clientFactory.CreateClient(Constants.DefaultHttpClientName))
            {
                while (triedTimes++ < this._addresses.Count)
                {
                    var address = this._addresses[this._currentIndex];
                    this._currentIndex = (this._currentIndex + 1) % this._addresses.Count;
                    if (!address.CheckAccessible())
                        continue;

                    Stream resStream;
                    try
                    {
                        resStream = await DoPost(client, address, postBuf);
                        address.Reset();
                    }
                    catch (Exception ex)
                    {
                        this._logger.LogError(ex, "request admin error.{0}", ex.Message);
                        address.SetFail();
                        continue;
                    }

                    RpcResponse res = null;
                    try
                    {
                        /*
                       using (StreamReader reader = new StreamReader(resStream))
                       {
                           string content  = await reader.ReadToEndAsync();

                           this._logger.LogWarning(content);
                       }
                       */
                        res = HessianSerializer.DeserializeResponse(resStream);
                    }
                    catch (Exception ex)
                    {
                        this._logger.LogError(ex, "DeserializeResponse error:{errorMessage}", ex.Message);
                    }

                    if (res == null)
                    {
                        retList.Add(ReturnT.Failed("response is null"));
                    }
                    else if (res.IsError)
                    {
                        retList.Add(ReturnT.Failed(res.ErrorMsg));
                    }
                    else if (res.Result is ReturnT ret)
                    {
                        retList.Add(ret);
                    }
                    else
                    {
                        retList.Add(ReturnT.Failed("response is null"));
                    }

                    if (!polling)
                    {
                        return retList[0];
                    }
                }

                if (retList.Count > 0)
                {
                    return retList.Last();
                }
            }
            throw new Exception("xxl-rpc server address not accessible.");
        }

        private async Task<Stream> DoPost(HttpClient client, AddressEntry address, byte[] postBuf)
        {
            var content = new ByteArrayContent(postBuf);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            var responseMessage = await client.PostAsync(address.RequestUri, content);

            responseMessage.EnsureSuccessStatusCode();
            return await responseMessage.Content.ReadAsStreamAsync();
        }
    }
}