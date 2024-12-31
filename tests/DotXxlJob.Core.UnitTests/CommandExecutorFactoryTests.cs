// Copyright (c) Xuanye Wang. All rights reserved.
// Licensed under MIT license

using DotXxlJob.Core.CommandExecutors;
using Moq;

namespace DotXxlJob.Core.UnitTests
{
    public class CommandExecutorFactoryTests
    {
        [Fact]
        public void GetCommandExecutor_ReturnsExecutor_WhenExecutorExists()
        {
            // Arrange
            var mockExecutor = new Mock<ICommandExecutor>();
            mockExecutor.Setup(e => e.CommandName).Returns("TestCommand");
            var executors = new List<ICommandExecutor> { mockExecutor.Object };
            var factory = new CommandExecutorFactory(executors);

            // Act
            var result = factory.GetCommandExecutor("TestCommand");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("TestCommand", result?.CommandName);
        }

        [Fact]
        public void GetCommandExecutor_ReturnsNull_WhenExecutorDoesNotExist()
        {
            // Arrange
            var mockExecutor = new Mock<ICommandExecutor>();
            mockExecutor.Setup(e => e.CommandName).Returns("TestCommand");
            var executors = new List<ICommandExecutor> { mockExecutor.Object };
            var factory = new CommandExecutorFactory(executors);

            // Act
            var result = factory.GetCommandExecutor("NonExistentCommand");

            // Assert
            Assert.Null(result);
        }
    }
}
