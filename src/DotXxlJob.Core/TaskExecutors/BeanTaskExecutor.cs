using System.Threading.Tasks;
using DotXxlJob.Core.Model;

namespace DotXxlJob.Core.TaskExecutors
{
    /// <summary>
    /// 实现 IJobHandler的执行器
    /// </summary>
    public class BeanTaskExecutor:ITaskExecutor
    {
        private readonly IJobHandlerFactory _handlerFactory;

        public BeanTaskExecutor(IJobHandlerFactory handlerFactory)
        {
            _handlerFactory = handlerFactory;
        }
        
        public string GlueType { get; } = Constants.GlueType.BEAN;
        
        public Task<ReturnT> Execute(TriggerParam triggerParam)
        {
            var handler = _handlerFactory.GetJobHandler(triggerParam.ExecutorHandler);

            if (handler == null)
            {
                
               return Task.FromResult(ReturnT.Failed($"job handler [{triggerParam.ExecutorHandler} not found."));
            }

            return Task.FromResult(ReturnT.Success("OK"));
            //return handler.Execute(new JobExecuteContext());
        }
    }
}