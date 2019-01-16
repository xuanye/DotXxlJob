using System.Threading.Tasks;
using DotXxlJob.Core.Model;

namespace DotXxlJob.Core
{
    public class HttpJobHandler:IJobHandler
    {
        public void Dispose()
        {
           
        }

        public Task<ReturnT> Execute(JobExecuteContext context)
        {
             return Task.FromResult(ReturnT.SUCCESS);
        }
    }
}