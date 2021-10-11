using System.Threading.Tasks;
using DotXxlJob.Core;
using DotXxlJob.Core.Model;

namespace ASPNetCoreExecutor
{
    /// <summary>
    /// 示例Job，只是写个日志
    /// </summary>
    [JobHandler("demoJobHandler")]
    public class DemoJobHandler : AbstractJobHandler
    {
        public override async Task<ReturnT> Execute(JobExecuteContext context)
        {
            context.JobLogger.Log("receive demo job handler,parameter:{0}", context.JobParameter);
            context.JobLogger.Log("开始休眠10秒");
            await Task.Delay(10 * 1000);
            context.JobLogger.Log("休眠10秒结束");
            return ReturnT.SUCCESS;
        }
    }
}