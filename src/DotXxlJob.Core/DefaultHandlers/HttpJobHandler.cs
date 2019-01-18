using System.Threading.Tasks;
using DotXxlJob.Core.Model;

namespace DotXxlJob.Core.DefaultHandlers
{
    [JobHandler("httpJobHandler")]
    public class HttpJobHandler:AbsJobHandler
    {
        public override Task<ReturnT> Execute(JobExecuteContext context)
        {
             context.JobLogger.Log("Get Request Data:{0}",context.JobParameter);
             return Task.FromResult(ReturnT.SUCCESS);
        }
    }
}