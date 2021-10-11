using System;
using Microsoft.Extensions.DependencyInjection;

namespace DotXxlJob.Core
{
    public class DefaultJobHandlerFactory : IJobHandlerFactory
    {
        private readonly JobHandlerCache _handlerCache;

        public DefaultJobHandlerFactory(IServiceProvider provider, JobHandlerCache handlerCache = null)
        {
            _handlerCache = handlerCache ?? new JobHandlerCache();

            Initialize(provider);
        }

        private void Initialize(IServiceProvider provider)
        {
            foreach (var handler in provider.GetServices<IJobHandler>())
            {
                _handlerCache.AddJobHandler(handler);
            }

            if (_handlerCache.IsEmpty)
            {
                throw new TypeLoadException("IJobHandlers are not found in IServiceCollection");
            }
        }

        public IJobHandler GetJobHandler(IServiceScopeFactory scopeFactory, string handlerName, out IServiceScope serviceScope)
        {
            serviceScope = null;

            var jobHandler = _handlerCache.Get(handlerName);

            if (jobHandler == null) return null;

            if (jobHandler.JobHandler != null) return jobHandler.JobHandler;

            serviceScope = scopeFactory.CreateScope();

            return (IJobHandler)serviceScope.ServiceProvider.GetRequiredService(jobHandler.JobHandlerType);
        }
    }
}