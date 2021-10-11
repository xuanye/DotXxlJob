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
        public static IServiceCollection AddJobHandler<TJob>(this IServiceCollection services)
            where TJob : class, IJobHandler
        {
            services.GetJobHandlerCache().AddJobHandler<TJob>();

            return services.AddScoped<TJob>();
        }

        /// <summary>允许创建Scoped实例</summary>
        public static IServiceCollection AddJobHandler<TJob>(this IServiceCollection services,
            Func<IServiceProvider, TJob> implementationFactory)
            where TJob : class, IJobHandler
        {
            services.GetJobHandlerCache().AddJobHandler<TJob>();

            return services.AddScoped(implementationFactory);
        }

        /// <summary>允许创建Scoped实例</summary>
        public static IServiceCollection AddJobHandler<TJob>(this IServiceCollection services,
            string handlerName)
            where TJob : class, IJobHandler
        {
            services.GetJobHandlerCache().AddJobHandler<TJob>(handlerName);

            return services.AddScoped<TJob>();
    }

        /// <summary>允许创建Scoped实例</summary>
        public static IServiceCollection AddJobHandler<TJob>(this IServiceCollection services,
            string handlerName, Func<IServiceProvider, TJob> implementationFactory)
            where TJob : class, IJobHandler
        {
            services.GetJobHandlerCache().AddJobHandler<TJob>(handlerName);

            return services.AddScoped(implementationFactory);
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