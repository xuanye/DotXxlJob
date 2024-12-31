// Copyright (c) Xuanye Wang. All rights reserved.
// Licensed under MIT license

using System.Collections.Generic;
using System.Linq;

namespace DotXxlJob.Core.Internal
{
    internal class JobExecutorFactory : IJobExecutorFactory
    {
        private readonly Dictionary<string, IJobExecutor> _jobExecutors;
        public JobExecutorFactory(IEnumerable<IJobExecutor> jobExecutors)
        {
            _jobExecutors = jobExecutors.ToDictionary(x => x.GlueType);
        }
        public IJobExecutor? GetJobExecutor(string glueType)
        {
            if (_jobExecutors.TryGetValue(glueType, out var executor))
            {
                return executor;
            }
            return null;
        }
    }

}
