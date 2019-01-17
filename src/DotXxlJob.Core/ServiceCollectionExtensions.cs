using Microsoft.Extensions.DependencyInjection;

namespace DotXxlJob.Core
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddXxlJobExecutor(this IServiceCollection services)
        {
            services.AddSingleton<ITaskExecutor, TaskExecutors.BeanTaskExecutor>();
            services.AddSingleton<IJobHandlerFactory,DefaultJobHandlerFactory >();
            services.AddSingleton<JobDispatcher>();
            services.AddSingleton<TaskExecutorFactory>();
            services.AddSingleton<XxlRpcServiceHandler>();
            services.AddSingleton<CallbackTaskQueue>();
            services.AddSingleton<AdminClient>();
            services.AddSingleton<IExecutorRegistry, ExecutorRegistry>();
            
            return services;
        }
    }
}