using System;
using System.Collections.Generic;
using System.Reflection;

namespace DotXxlJob.Core
{
    public class JobHandlerCache
    {
        private readonly Dictionary<string, JobHandlerItem> _handlersCache  = new Dictionary<string, JobHandlerItem>();

        public bool IsEmpty => _handlersCache.Count < 1;

        public void AddJobHandler<TJob>() where TJob : IJobHandler =>
            AddJobHandler<TJob>(typeof(TJob).GetCustomAttribute<JobHandlerAttribute>()?.Name ??
                                typeof(TJob).Name);

        public void AddJobHandler<TJob>(string handlerName) where TJob : IJobHandler =>
            AddJobHandler(handlerName, new JobHandlerItem { JobHandlerType = typeof(TJob) });

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
            if (_handlersCache.ContainsKey(handlerName))
            {
                throw new ArgumentException($"Same IJobHandler' name: [{handlerName}]", nameof(handlerName));
            }

            _handlersCache.Add(handlerName, jobHandler);
        }

        public JobHandlerItem Get(string handlerName) =>
            _handlersCache.TryGetValue(handlerName, out var item) ? item : null;

        public class JobHandlerItem
        {
            public IJobHandler JobHandler { get; set; }

            public Type JobHandlerType { get; set; }
        }
    }
}
