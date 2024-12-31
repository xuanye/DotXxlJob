// Copyright (c) Xuanye Wang. All rights reserved.
// Licensed under MIT license

namespace DotXxlJob.Core
{
    public class JobExecuteContext
    {
        public JobExecuteContext(IJobLogger jobLogger)
        {
            JobLogger = jobLogger;
        }

        public IJobLogger JobLogger { get; } = null!;
    }
}
