using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using DotXxlJob.Core.Config;
using DotXxlJob.Core.Model;
using Flurl;
using Flurl.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace DotXxlJob.Core
{
    public class AdminClient
    {
     
        private readonly XxlJobExecutorOptions _options; 
        private readonly ILogger<AdminClient> _logger;
        private List<AddressEntry> _addresses;
        private int _currentIndex;
        private static readonly string MAPPING = "/api";
        public AdminClient(IOptions<XxlJobExecutorOptions> optionsAccessor          
            , ILogger<AdminClient> logger)
        {
            Preconditions.CheckNotNull(optionsAccessor?.Value, "XxlJobExecutorOptions");

            this._options = optionsAccessor?.Value;        
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
                    var entry = new AddressEntry { RequestUri = item+ MAPPING };
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
            
            return InvokeRpcService("callback", callbackParamList);
        }

        public Task<ReturnT> Registry(RegistryParam registryParam)
        {
            return InvokeRpcService("registry", registryParam);
        }

        public Task<ReturnT> RegistryRemove(RegistryParam registryParam)
        {
            return InvokeRpcService("registryRemove",  registryParam);
        }


        private async Task<ReturnT> InvokeRpcService(string methodName, object jsonObject)
        {
            var triedTimes = 0;
            ReturnT ret = null;

            while (triedTimes++ < this._addresses.Count)
            {

                var address = this._addresses[this._currentIndex];
                this._currentIndex = (this._currentIndex + 1) % this._addresses.Count;
                if (!address.CheckAccessible())
                    continue;
                try
                {

                    var json = await address.RequestUri.AppendPathSegment(methodName)
                    .WithHeader("XXL-JOB-ACCESS-TOKEN", this._options.AccessToken)
                    .PostJsonAsync(jsonObject)
                    .ReceiveString();

                    //.ReceiveJson<ReturnT>();
                    ret = JsonConvert.DeserializeObject<ReturnT>(json);
                    address.Reset();
                }
                catch (Exception ex)
                {
                    this._logger.LogError(ex, "request admin error.{0}", ex.Message);
                    address.SetFail();
                    continue;
                }
            }            
            if(ret == null)
            {
                ret = ReturnT.Failed("call admin fail");
            }
            return ret;
        }

    }
}