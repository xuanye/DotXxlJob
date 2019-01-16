using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace DotXxlJob.Core
{
    /// <summary>
    /// 负责响应RPC请求，调度任务执行器的工厂类
    /// </summary>
    public class TaskExecutorFactory
    {
        private readonly IServiceProvider _provider;

        private readonly Dictionary<string, ITaskExecutor> _cache = new Dictionary<string, ITaskExecutor>();
        public TaskExecutorFactory(IServiceProvider provider)
        {
            _provider = provider;
            Initialize();
        }

        private void Initialize()
        {
            var executors =  _provider.GetServices<ITaskExecutor>();

            if (executors != null && executors.Any())
            {
                foreach (var item in executors)
                {
                    _cache.Add(item.GlueType,item);
                }
            }
        }

        public ITaskExecutor GetTaskExecutor(string glueType)
        {
            return _cache.TryGetValue(glueType, out var executor) ? executor : null;
        }
    }
}