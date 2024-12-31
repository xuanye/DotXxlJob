// Copyright (c) Xuanye Wang. All rights reserved.
// Licensed under MIT license

using System.Collections.Generic;
using System.Linq;

namespace DotXxlJob.Core.CommandExecutors
{
    public class CommandExecutorFactory : ICommandExecutorFactory
    {
        private readonly Dictionary<string, ICommandExecutor> _commandExecutors;

        public CommandExecutorFactory(IEnumerable<ICommandExecutor> commandExecutors)
        {
            _commandExecutors = commandExecutors.ToDictionary(x => x.CommandName, y => y);
        }

        public ICommandExecutor? GetCommandExecutor(string commandName)
        {

            if (_commandExecutors.TryGetValue(commandName, out var commandExecutor))
            {
                return commandExecutor;
            }
            return null;
        }
    }
}
