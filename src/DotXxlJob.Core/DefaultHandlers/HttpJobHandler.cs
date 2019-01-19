using System.Threading.Tasks;
using DotXxlJob.Core.Model;

namespace DotXxlJob.Core.DefaultHandlers
{
    [JobHandler("httpJobHandler")]
    public class HttpJobHandler:AbsJobHandler
    {
        public async override Task<ReturnT> Execute(JobExecuteContext context)
        {
            if (string.IsNullOrEmpty(context.JobParameter))
            {
                return ReturnT.Failed("url is empty");
            }
            //判断是否为单URL
            context.JobLogger.Log("Get Request Data:{0}",context.JobParameter);
            return ReturnT.SUCCESS;
        }
    }
    
}