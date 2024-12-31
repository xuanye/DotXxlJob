// Copyright (c) Xuanye Wang. All rights reserved.
// Licensed under MIT license

using DotXxlJob.Core.Models;

namespace DotXxlJob.Core.Internal
{
    internal class JobCompletedArgs
    {
        public Job Job { get; set; } = null!;

        public TaskResult TaskResult { get; set; } = null!;
    }
}
