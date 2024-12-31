// Copyright (c) Xuanye Wang. All rights reserved.
// Licensed under MIT license

using DotXxlJob.Core.Models;
using System.Threading.Tasks;

namespace DotXxlJob.Core
{
    public interface ICommandExecutor
    {
        string CommandName { get; }
        Task<ApiResult> ExecuteAsync(byte[] payload);
    }


}