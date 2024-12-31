// Copyright (c) Xuanye Wang. All rights reserved.
// Licensed under MIT license

using System.Threading.Tasks;

namespace DotXxlJob.Core
{
    public interface IJobLogger
    {
        Task LogAsync(string message);
    }
}
