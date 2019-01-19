using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace DotXxlJob.Core
{
    public class DefaultJobHandlerFactory:IJobHandlerFactory
    {
        private readonly IServiceProvider _provider;
        private readonly Dictionary<string, IJobHandler> handlersCache = new Dictionary<string, IJobHandler>();
        public DefaultJobHandlerFactory(IServiceProvider provider)
        {
            this._provider = provider;
            Initialize();
        }

        private void Initialize()
        {
            var list = this._provider.GetServices<IJobHandler>();
            if (list == null || !list.Any())
            {
                throw new TypeLoadException("IJobHandlers are not found in IServiceCollection");
            }

            foreach (var handler in list)
            {
                var jobHandlerAttr = handler.GetType().GetCustomAttribute<JobHandlerAttribute>();
                var handlerName = jobHandlerAttr == null ? handler.GetType().Name : jobHandlerAttr.Name;
                if (handlersCache.ContainsKey(handlerName))
                {
                    throw  new Exception($"same IJobHandler' name: [{handlerName}]");
                }
                handlersCache.Add(handlerName,handler);
            }
           
        }

        public IJobHandler GetJobHandler(string handlerName)
        {
            if (handlersCache.ContainsKey(handlerName))
            {
               return handlersCache[handlerName];
            }
            return null;
        }
    }
}