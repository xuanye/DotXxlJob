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
    public class ExecutorRegistry : IExecutorRegistry
    {
        private readonly AdminClient _adminClient;
        private readonly XxlJobExecutorOptions _options;
        private readonly ILogger<ExecutorRegistry> _logger;

        public ExecutorRegistry(AdminClient adminClient, IOptions<XxlJobExecutorOptions> optionsAccessor, ILogger<ExecutorRegistry> logger)
        {
            Preconditions.CheckNotNull(optionsAccessor, "XxlJobExecutorOptions");
            Preconditions.CheckNotNull(optionsAccessor.Value, "XxlJobExecutorOptions");
            _adminClient = adminClient;
            _options = optionsAccessor.Value;
            if (string.IsNullOrEmpty(_options.SpecialBindAddress))
            {
                _options.SpecialBindAddress = IPUtility.GetLocalIntranetIP().MapToIPv4().ToString();
            }
            _logger = logger;
        }

        public async Task RegistryAsync(CancellationToken cancellationToken)
        {
            var registryParam = new RegistryParam {
                RegistryGroup = "EXECUTOR",
                RegistryKey = _options.AppName,
                RegistryValue = $"{_options.SpecialBindAddress}:{_options.Port}"
            };

            _logger.LogInformation(">>>>>>>> start registry to admin <<<<<<<<");

            var errorTimes = 0;

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var ret = await _adminClient.Registry(registryParam);
                    _logger.LogDebug("registry last result:{0}", ret?.Code);
                    errorTimes = 0;
                    await Task.Delay(Constants.RegistryInterval, cancellationToken);
                }
                catch (TaskCanceledException)
                {
                    _logger.LogInformation(">>>>> Application Stopping....<<<<<");
                }
                catch (Exception ex)
                {
                    errorTimes++;
                    await Task.Delay(Constants.RegistryInterval, cancellationToken);
                    _logger.LogError(ex, "registry error:{0},{1} Times", ex.Message, errorTimes);
                }
            }

            _logger.LogInformation(">>>>>>>> end registry to admin <<<<<<<<");

            _logger.LogInformation(">>>>>>>> start remove registry to admin <<<<<<<<");

            var removeRet = await this._adminClient.RegistryRemove(registryParam);
            _logger.LogInformation("remove registry last result:{0}", removeRet?.Code);
            _logger.LogInformation(">>>>>>>> end remove registry to admin <<<<<<<<");
        }
    }
}