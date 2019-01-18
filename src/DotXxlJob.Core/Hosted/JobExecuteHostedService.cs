using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace DotXxlJob.Core
{
    /// <summary>
    ///  NOTE: 负责启动Executor服务，和进行服务注册的宿主服务
    /// </summary>
    public class JobsExecuteHostedService:IHostedService
    {
        public Task StartAsync(CancellationToken cancellationToken)
        {
            //1 初始化服务注册
            //2 初始化adminClient
            
            //3 初始化执行线程
            
            //4 初始化XXL_RPC服务端口，HTTP服务
            
            throw new System.NotImplementedException();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            //清理Start中启动的资源
            throw new System.NotImplementedException();
        }
    }
}