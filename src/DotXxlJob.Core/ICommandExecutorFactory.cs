// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

namespace DotXxlJob.Core
{
    public interface ICommandExecutorFactory
    {
        ICommandExecutor GetCommandExecutor(string commandName);
    }
}