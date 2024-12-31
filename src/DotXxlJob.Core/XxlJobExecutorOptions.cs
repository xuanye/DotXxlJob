// Copyright (c) Xuanye Wang. All rights reserved.
// Licensed under MIT license

using System;
using System.IO;

namespace DotXxlJob.Core
{
    public class XxlJobExecutorOptions
    {
        /// <summary>
        /// admin site url, separated by semicolons
        /// </summary>
        public string AdminSiteUrl { get; set; } = null!;


        /// <summary>
        /// App name, must be consistent with the configuration on the admin side when auto-registering
        /// </summary>
        public string AppName { get; set; } = "xxl-job-executor-dotnet";


        /// <summary>
        /// Special URL to bind, if this is configured, SpecialBindAddress and Port will be ignored
        /// </summary>
        public string? SpecialBindUrl { get; set; }

        /// <summary>
        /// Address to submit when auto-registering, if empty, the internal network address will be automatically obtained
        /// </summary>
        public string? SpecialBindAddress { get; set; }


        /// <summary>
        /// Port to bind
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Whether to auto-register
        /// </summary>
        public bool AutoRegistry { get; set; }

        /// <summary>
        /// Access token
        /// </summary>
        public string? AccessToken { get; set; }


        /// <summary>
        /// Log directory, defaults to the logs subdirectory of the execution directory, please configure an absolute path
        /// </summary>
        public string LogPath { get; set; } = Path.Combine(AppContext.BaseDirectory, "./logs");


        /// <summary>
        /// Log retention days
        /// </summary>
        public int LogRetentionDays { get; set; } = 30;


        /// <summary>
        /// Callback interval in milliseconds
        /// </summary>
        public int CallBackInterval { get; set; } = 500;
    }
}
