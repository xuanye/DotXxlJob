using System;
using DotXxlJob.Core.Config;
using DotXxlJob.Core.DefaultHandlers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DotXxlJob.Core
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddXxlJobExecutor(this IServiceCollection services,IConfiguration configuration)
        {
            services.AddLogging();
            services.AddOptions();
            services.Configure<XxlJobExecutorOptions>(configuration.GetSection("xxlJob"))
                .AddXxlJobExecutorServiceDependency();
            
            return services;
        }
        public static IServiceCollection AddXxlJobExecutor(this IServiceCollection services,Action<XxlJobExecutorOptions> configAction)
        {
            services.AddLogging();
            services.AddOptions();
            services.Configure(configAction).AddXxlJobExecutorServiceDependency();
            return services;
        }
        
        public static IServiceCollection AddDefaultXxlJobHandlers(this IServiceCollection services)
        {
            services.AddSingleton<IJobHandler,HttpJobHandler >();
            return services;
        }
        
        private static IServiceCollection AddXxlJobExecutorServiceDependency(this IServiceCollection services)
        {
           
            services.AddHttpClient("DotXxlJobClient");
            services.AddSingleton<IJobLogger, JobLogger>();
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