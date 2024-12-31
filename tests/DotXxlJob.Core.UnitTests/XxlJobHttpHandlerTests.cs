// Copyright (c) Xuanye Wang. All rights reserved.
// Licensed under MIT license

using DotXxlJob.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Moq;
using System.Text;

namespace DotXxlJob.Core.UnitTests
{

    public class XxlJobHttpHandlerTests
    {
        private readonly Mock<ICommandExecutorFactory> _commandExecutorFactoryMock;
        private readonly Mock<ISerializer> _serializerMock;
        private readonly Mock<IOptions<XxlJobExecutorOptions>> _optionsAccessorMock;
        private readonly XxlJobHttpHandler _handler;

        public XxlJobHttpHandlerTests()
        {
            _commandExecutorFactoryMock = new Mock<ICommandExecutorFactory>();
            _serializerMock = new Mock<ISerializer>();
            _optionsAccessorMock = new Mock<IOptions<XxlJobExecutorOptions>>();
            _optionsAccessorMock.Setup(o => o.Value).Returns(new XxlJobExecutorOptions { AccessToken = "test-token" });

            _handler = new XxlJobHttpHandler(_commandExecutorFactoryMock.Object, _serializerMock.Object, _optionsAccessorMock.Object);
        }

        [Fact]
        public async Task HandleAsync_ShouldReturnUnauthorized_WhenAccessTokenIsInvalid()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Path = "/test-command";
            context.Request.Headers["XXL-JOB-ACCESS-TOKEN"] = "invalid-token";

            // Act
            await _handler.HandleAsync(context);

            // Assert
            Assert.Equal(StatusCodes.Status401Unauthorized, context.Response.StatusCode);
        }

        [Fact]
        public async Task HandleAsync_ShouldReturnBadRequest_WhenCommandExecutorIsNull()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Path = "/test-command";
            context.Request.Headers["XXL-JOB-ACCESS-TOKEN"] = "test-token";

            _commandExecutorFactoryMock.Setup(f => f.GetCommandExecutor(It.IsAny<string>())).Returns((ICommandExecutor)null);

            // Act
            await _handler.HandleAsync(context);

            // Assert
            Assert.Equal(StatusCodes.Status400BadRequest, context.Response.StatusCode);
        }

        [Fact]
        public async Task HandleAsync_ShouldReturnSuccess_WhenCommandExecutorExecutesSuccessfully()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Path = "/test-command";
            context.Request.Headers["XXL-JOB-ACCESS-TOKEN"] = "test-token";
            context.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes("test-payload"));

            var executorMock = new Mock<ICommandExecutor>();
            executorMock.Setup(e => e.ExecuteAsync(It.IsAny<byte[]>())).ReturnsAsync(new ApiResult { Code = 200 });

            _commandExecutorFactoryMock.Setup(f => f.GetCommandExecutor(It.IsAny<string>())).Returns(executorMock.Object);

            // Act
            await _handler.HandleAsync(context);

            // Assert
            Assert.Equal(StatusCodes.Status200OK, context.Response.StatusCode);
        }
    }

}
