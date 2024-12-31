// Copyright (c) Xuanye Wang. All rights reserved.
// Licensed under MIT license

namespace DotXxlJob.Core.Models
{

    public readonly record struct JobId(int Value);

    public class Job
    {
        public JobId JobId { get; set; }

        public string GlueType { get; set; } = null!;
        public string HandlerName { get; set; } = null!;
        public string ExecutorParams { get; set; } = null!;
        public IJobExecutor? Executor { get; set; }

        public string ExecutorBlockStrategy { get; set; } = null!;

        public int ExecutorTimeout { get; set; }
    }
}
