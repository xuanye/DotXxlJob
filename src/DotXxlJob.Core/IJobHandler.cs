using System;
using System.Threading.Tasks;

namespace DotXxlJob.Core
{
    public abstract class AbstractJobHandler:IJobHandler
    {
     
        /// <summary>
        /// 
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public abstract Task<ReturnT<string>> Execute(string param);


        public virtual void Dispose()
        {
        }
    }

    public interface IJobHandler:IDisposable
    {
        Task<ReturnT<string>> Execute(string param);
    }
}