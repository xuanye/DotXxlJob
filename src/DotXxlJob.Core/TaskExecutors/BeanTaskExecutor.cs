using System.Threading.Tasks;
using DotXxlJob.Core.Model;
using Microsoft.Extensions.DependencyInjection;

namespace DotXxlJob.Core.TaskExecutors
{
  /// <summary>
  /// 实现 IJobHandler的执行器
  /// </summary>
  public class BeanTaskExecutor : ITaskExecutor
    {
        private readonly IJobHandlerFactory _handlerFactory;
        private readonly IJobLogger _jobLogger;
        private readonly IServiceScopeFactory _scopeFactory;

        public BeanTaskExecutor(IJobHandlerFactory handlerFactory, IJobLogger jobLogger, IServiceScopeFactory scopeFactory)
        {
            this._handlerFactory = handlerFactory;
            this._jobLogger = jobLogger;
            this._scopeFactory = scopeFactory;
        }

        public string GlueType { get; } = Constants.GlueType.BEAN;

        public async Task<ReturnT> Execute(TriggerParam triggerParam)
        {
            var handler = _handlerFactory.GetJobHandler(_scopeFactory, triggerParam.ExecutorHandler, out var scope);

            if (scope == null) return await Execute(handler, triggerParam);

            using (scope)
            using (handler)
            {
                return await Execute(handler, triggerParam);
            }
        }

        private Task<ReturnT> Execute(IJobHandler handler, TriggerParam triggerParam)
        {
            if (handler == null)
            {
                return Task.FromResult(ReturnT.Failed($"job handler [{triggerParam.ExecutorHandler} not found."));
            }
            var context = new JobExecuteContext(this._jobLogger, triggerParam.ExecutorParams);
            return handler.Execute(context);
        }
    }
}