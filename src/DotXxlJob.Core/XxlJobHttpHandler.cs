// Copyright (c) Xuanye Wang. All rights reserved.
// Licensed under MIT license

using DotXxlJob.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Threading.Tasks;


namespace DotXxlJob.Core
{
    public class XxlJobHttpHandler
    {
        private readonly ICommandExecutorFactory _commandExecutorFactory;
        private readonly ISerializer _serializer;
        private readonly XxlJobExecutorOptions _options;
        public XxlJobHttpHandler(ICommandExecutorFactory commandExecutorFactory, ISerializer serializer, IOptions<XxlJobExecutorOptions> optionsAccessor)
        {
            if (optionsAccessor?.Value == null)
            {
                throw new ArgumentNullException(nameof(optionsAccessor));
            }
            _options = optionsAccessor.Value;
            _commandExecutorFactory = commandExecutorFactory;
            _serializer = serializer;
        }
        public async Task HandleAsync(HttpContext context)
        {
            var request = context.Request;
            var response = context.Response;
            var command = GetMethodName(request.Path.Value);

            if (string.IsNullOrEmpty(command))
            {
                //no need to do anything
                return;
            }
            if (!ValidateAccessToken(request))
            {
                await SendErrorResponse(response, StatusCodes.Status401Unauthorized, "Unauthorized");
                return;
            }
            var executor = _commandExecutorFactory.GetCommandExecutor(command);

            if (executor == null)
            {
                await SendErrorResponse(response, StatusCodes.Status400BadRequest, "The method have not been implemented");
                return;
            }

            byte[] payload;
            using (var memoryStream = new MemoryStream())
            {
                await request.Body.CopyToAsync(memoryStream);
                payload = memoryStream.ToArray();
            }
            var result = await executor.ExecuteAsync(payload);
            await SendResponse(response, result.Code, result);
        }

        private bool ValidateAccessToken(HttpRequest request)
        {
            if (string.IsNullOrEmpty(_options.AccessToken))
            {
                return true;
            }

            if (request.Headers.TryGetValue("XXL-JOB-ACCESS-TOKEN", out var accessToken) && _options.AccessToken.Equals(accessToken))
            {
                return true;
            }

            return false;
        }

        private static string GetMethodName(string? path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return string.Empty;
            }

            var arrParts = path.Split('/');
            if (arrParts.Length < 1)
            {
                return string.Empty;
            }
            return arrParts[arrParts.Length - 1].ToLower();
        }

        private Task SendResponse(HttpResponse response, int statusCode, object? data = null)
        {
            response.StatusCode = statusCode;
            response.ContentType = "application/json";
            if (data != null)
            {
                var bytes = _serializer.Serialize(data, data.GetType());
                return response.Body.WriteAsync(bytes, 0, bytes.Length);
            }
            return Task.CompletedTask;
        }

        private Task SendErrorResponse(HttpResponse response, int statusCode, string message)
        {
            return SendResponse(response, statusCode, ApiResult.Failure(message));
        }
    }
}

