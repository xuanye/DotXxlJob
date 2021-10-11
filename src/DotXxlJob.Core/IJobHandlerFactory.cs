using System;
using Microsoft.Extensions.DependencyInjection;

namespace DotXxlJob.Core
{
    public interface IJobHandlerFactory
    {
        IJobHandler GetJobHandler(IServiceScopeFactory scopeFactory, string handlerName, out IServiceScope serviceScope);
    }
}