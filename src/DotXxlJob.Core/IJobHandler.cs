using System;
using System.Threading.Tasks;
using DotXxlJob.Core.Model;

namespace DotXxlJob.Core
{
    public abstract class AbstractJobHandler:IJobHandler
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public abstract Task<ReturnT> Execute(JobExecuteContext context);


        public virtual void Dispose()
        {
        }
    }

    public interface IJobHandler:IDisposable
    {
        Task<ReturnT> Execute(JobExecuteContext context);
    }
}