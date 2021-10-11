using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using DotXxlJob.Core.DefaultHandlers;

namespace DotXxlJob.Core
{
    public class JobHandlerCache
    {
        internal Dictionary<string, JobHandlerItem> HandlersCache { get; } = new Dictionary<string, JobHandlerItem>();

        public void AddJobHandler<TJob>(params object[] constructorParameters)
            where TJob : IJobHandler =>
            AddJobHandler<TJob>(typeof(TJob).GetCustomAttribute<JobHandlerAttribute>()?.Name ??
                                typeof(TJob).Name, constructorParameters);

        public void AddJobHandler<TJob>(string handlerName, params object[] constructorParameters)
            where TJob : IJobHandler =>
            AddJobHandler(handlerName, new JobHandlerItem {
                JobHandlerType = typeof(TJob),
                JobHandlerConstructorParameters = constructorParameters,
            });

        public void AddJobHandler(IJobHandler jobHandler)
        {
            var jobHandlerType = jobHandler.GetType();

            var handlerName = jobHandlerType.GetCustomAttribute<JobHandlerAttribute>()?.Name ?? jobHandlerType.Name;

            AddJobHandler(handlerName, jobHandler);
        }

        public void AddJobHandler(string handlerName, IJobHandler jobHandler)
        {

            AddJobHandler(handlerName, new JobHandlerItem { JobHandler = jobHandler });
        }

        private void AddJobHandler(string handlerName, JobHandlerItem jobHandler)
        {
            if (HandlersCache.ContainsKey(handlerName))
            {
                throw new ArgumentException($"Same IJobHandler' name: [{handlerName}]", nameof(handlerName));
            }

            HandlersCache.Add(handlerName, jobHandler);
        }

        public JobHandlerItem Get(string handlerName) =>
            HandlersCache.TryGetValue(handlerName, out var item) ? item : null;

        public class JobHandlerItem
        {
            public IJobHandler JobHandler { get; set; }

            public Type JobHandlerType { get; set; }

            public object[] JobHandlerConstructorParameters { get; set; }
        }
    }
}
