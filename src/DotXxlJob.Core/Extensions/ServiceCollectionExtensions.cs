using System;
using System.Linq;
using DotXxlJob.Core.Config;
using DotXxlJob.Core.DefaultHandlers;
using DotXxlJob.Core.Queue;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace DotXxlJob.Core
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddXxlJobExecutor(this IServiceCollection services, IConfiguration configuration) =>
            services.AddXxlJobExecutor(configuration.GetSection("xxlJob").Bind);

        public static IServiceCollection AddXxlJobExecutor(this IServiceCollection services, Action<XxlJobExecutorOptions> configAction)
        {
            services.AddLogging();
            services.AddOptions();
            services.Configure(configAction).AddXxlJobExecutorServiceDependency();
            return services;
        }

        public static IServiceCollection AddDefaultXxlJobHandlers(this IServiceCollection services)
        {
            services.AddSingleton<IJobHandler, SimpleHttpJobHandler>();
            return services;
        }

        public static IServiceCollection AddAutoRegistry(this IServiceCollection services)
        {
            services.AddSingleton<IExecutorRegistry, ExecutorRegistry>()
                .AddSingleton<IHostedService, JobsExecuteHostedService>();
            return services;
        }

        private static IServiceCollection AddXxlJobExecutorServiceDependency(this IServiceCollection services)
        {
            //可在外部提前注册对应实现，并替换默认实现
            services.TryAddSingleton<IJobLogger, JobLogger>();
            services.TryAddSingleton<IJobHandlerFactory, DefaultJobHandlerFactory>();
            services.TryAddSingleton<IExecutorRegistry, ExecutorRegistry>();

            services.AddHttpClient("DotXxlJobClient");
            services.AddSingleton<JobDispatcher>();
            services.AddSingleton<TaskExecutorFactory>();
            services.AddSingleton<XxlRestfulServiceHandler>();
            services.AddSingleton<CallbackTaskQueue>();
            services.AddSingleton<AdminClient>();
            services.AddSingleton<ITaskExecutor, TaskExecutors.BeanTaskExecutor>();
            services.AddSingleton(new JobHandlerCache());

            return services;
        }

        /// <summary>允许创建Scoped实例</summary>
        /// <typeparam name="TJob"></typeparam>
        /// <param name="services"></param>
        /// <param name="constructorParameters">用于创建实例的额外参数，比如字符串</param>
        /// <returns></returns>
        public static IServiceCollection AddJobHandler<TJob>(this IServiceCollection services,
            params object[] constructorParameters) where TJob : IJobHandler
        {
            services.GetJobHandlerCache().AddJobHandler<TJob>(constructorParameters);

            return services;
        }

        /// <summary>允许创建Scoped实例</summary>
        /// <typeparam name="TJob"></typeparam>
        /// <param name="services"></param>
        /// <param name="handlerName"></param>
        /// <param name="constructorParameters">用于创建实例的额外参数，比如字符串</param>
        /// <returns></returns>
        public static IServiceCollection AddJobHandler<TJob>(this IServiceCollection services,
            string handlerName, params object[] constructorParameters) where TJob : IJobHandler
        {
            services.GetJobHandlerCache().AddJobHandler<TJob>(handlerName, constructorParameters);

            return services;
        }

        private static JobHandlerCache GetJobHandlerCache(this IServiceCollection services)
        {
            var sd = services.FirstOrDefault(x => x.ImplementationInstance is JobHandlerCache);
            if (sd != null) return (JobHandlerCache)sd.ImplementationInstance;

            var cache = new JobHandlerCache();

            services.AddSingleton(cache);

            return cache;
        }
    }
}