using System;
using System.Threading;
using System.Threading.Tasks;
using DotXxlJob.Core.Config;
using DotXxlJob.Core.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DotXxlJob.Core
{
    /// <summary>
    ///  执行器注册注册
    /// </summary>
    public class ExecutorRegistry:IExecutorRegistry
    {
        private readonly AdminClient _adminClient;
        private readonly XxlJobExecutorOptions _options;
        private readonly ILogger<ExecutorRegistry> _logger;

        public ExecutorRegistry(AdminClient adminClient,IOptions<XxlJobExecutorOptions> optionsAccessor,ILogger<ExecutorRegistry> logger)
        {
            Preconditions.CheckNotNull(optionsAccessor, "XxlJobExecutorOptions");
            Preconditions.CheckNotNull(optionsAccessor.Value, "XxlJobExecutorOptions");
            this._adminClient = adminClient;
            this._options = optionsAccessor.Value;
            if (string.IsNullOrEmpty(this._options.SpecialBindAddress))
            {
                this._options.SpecialBindAddress = IPUtility.GetLocalIntranetIP().MapToIPv4().ToString();
            }
            this._logger = logger;
        }
        
        public async Task RegistryAsync(CancellationToken cancellationToken)
        {
            var registryParam = new RegistryParam {
                RegistryGroup = "EXECUTOR",
                RegistryKey = this._options.AppName,
                RegistryValue = $"{this._options.SpecialBindAddress}:{this._options.Port}"
            };

            this._logger.LogInformation(">>>>>>>> start registry to admin <<<<<<<<");

            var errorTimes = 0;
            
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var ret = await this._adminClient.Registry(registryParam);
                    this._logger.LogDebug("registry last result:{0}", ret?.Code);
                    errorTimes = 0;
                    await Task.Delay(Constants.RegistryInterval, cancellationToken);
                }
                catch (TaskCanceledException)
                {
                    this._logger.LogInformation(">>>>> Application Stopping....<<<<<");
                }
                catch (Exception ex)
                {
                    errorTimes++;
                    this._logger.LogError(ex,"registry error:{0},{1} Times",ex.Message,errorTimes);
                }
            } 
            
            this._logger.LogInformation(">>>>>>>> end registry to admin <<<<<<<<");
            
            this._logger.LogInformation(">>>>>>>> start remove registry to admin <<<<<<<<");
            
            var removeRet = await this._adminClient.RegistryRemove(registryParam);
            this._logger.LogInformation("remove registry last result:{0}",removeRet?.Code);
            this._logger.LogInformation(">>>>>>>> end remove registry to admin <<<<<<<<");
        }
       
        
    }
}