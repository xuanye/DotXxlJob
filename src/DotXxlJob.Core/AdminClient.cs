using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using DotXxlJob.Core.Config;
using DotXxlJob.Core.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DotXxlJob.Core
{
    public class AdminClient
    {
        static readonly string MAPPING = "/api";
        private readonly XxlJobExecutorOptions _options;
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<AdminClient> _logger;
        
        private List<AddressEntry> _addresses;

        private int _currentIndex;
        
        public AdminClient(IOptions<XxlJobExecutorOptions> optionsAccessor
            ,IHttpClientFactory clientFactory
            ,ILogger<AdminClient> logger)
        {
            this._options = optionsAccessor.Value;
            _clientFactory = clientFactory;
            this._logger = logger;
            InitAddress();
        }

        private void InitAddress()
        {
            this._addresses = new List<AddressEntry>();
            foreach (var item in this._options.AdminAddresses)
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
            return InvokeRpcService("callback", new List<string> {"java.util.List"}, callbackParamList);
        }


        public  Task<ReturnT> Registry(RegistryParam registryParam)
        { 
            return InvokeRpcService("callback", new List<string> {"java.lang.Class"}, registryParam);
        }

        public  Task<ReturnT> RegistryRemove(RegistryParam registryParam)
        {
            return InvokeRpcService("callback", new List<string> {"java.lang.Class"}, registryParam);
        }

        private async Task<ReturnT> InvokeRpcService(string methodName, List<string> parameterTypes,
            object parameters)
        {
            var request = new RpcRequest {
                CreateMillisTime = DateTime.Now.ToUnixTimeSeconds(),
                AccessToken = this._options.AccessToken,
                ClassName = "com.xxl.job.core.biz.AdminBiz",
                MethodName = methodName,
                ParameterTypes = parameterTypes.ToList<object>(),
                Parameters = new List<object> {parameters}
            };
            byte[] postBuf;
            using (var stream = new MemoryStream())
            {
               HessianSerializer.SerializeRequest(stream,request);

               postBuf =stream.ToArray();
            }

            int triedTimes = 0;
            
            using (var client = this._clientFactory.CreateClient())
            {
           
                while (triedTimes++ < _addresses.Count)
                {
                    var address = _addresses[_currentIndex];
                    _currentIndex = (_currentIndex + 1) % _addresses.Count;
                    if (!address.CheckAccessable())
                        continue;
    
                    Stream resStream;
                    try
                    {
                        resStream =await DoPost(client, address, postBuf);
                        address.Reset();
                       
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "request admin error.");
                        address.SetFail();
                        continue;
                    }

                    RpcResponse res;
                    try
                    {
                       res =  HessianSerializer.DeserializeResponse(resStream);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError("des");
                    }
                    
                   
                    if (res.IsError)
                    {
                        throw new Exception(res.error);
                    }
                    else
                    {
                        return rpcResponse.result as ReturnT;
                    }
                }
            }
            throw new Exception("xxl-rpc server address not accessable.");
            
            
        }

        private async Task<Stream> DoPost(HttpClient client,AddressEntry address, byte[] postBuf)
        {
            var content = new ByteArrayContent(postBuf);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            var responseMessage = await client.PostAsync(address.RequestUri, content);
            responseMessage.EnsureSuccessStatusCode();
            return await responseMessage.Content.ReadAsStreamAsync();
        }
    
   
    }
}