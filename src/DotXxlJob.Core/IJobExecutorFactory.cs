// Copyright (c) Xuanye Wang. All rights reserved.
// Licensed under MIT license

namespace DotXxlJob.Core
{
    public interface IJobExecutorFactory
    {
        IJobExecutor? GetJobExecutor(string glueType);
    }

}
