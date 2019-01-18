using System.Threading.Tasks;
using DotXxlJob.Core.Model;

namespace DotXxlJob.Core.DefaultHandlers
{
    public abstract class AbsJobHandler:IJobHandler
    {
        public virtual void Dispose()
        {
           
        }

        public abstract Task<ReturnT> Execute(JobExecuteContext context);

    }
}