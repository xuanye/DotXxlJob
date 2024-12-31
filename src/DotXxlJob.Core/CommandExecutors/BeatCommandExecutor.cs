// Copyright (c) Xuanye Wang. All rights reserved.
// Licensed under MIT license

using DotXxlJob.Core.Models;
using System.Threading.Tasks;

namespace DotXxlJob.Core.CommandExecutors
{
    public class BeatCommandExecutor : ICommandExecutor
    {
        public string CommandName => "Beat";

        public Task<ApiResult> ExecuteAsync(byte[] payload)
        {
            return Task.FromResult(ApiResult.Success());
        }
    }
}