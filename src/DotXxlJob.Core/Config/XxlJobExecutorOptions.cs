using System;
using System.IO;

namespace DotXxlJob.Core.Config
{
    public class XxlJobExecutorOptions
    {
       
        public string AdminAddresses { get; set; }


        public string AppName { get; set; } = "DotXxlJob";

     
        public string SpecialBindAddress { get; set; }

      
        public int Port { get; set; }

      
        public string AccessToken { get; set; }


        public string LogPath { get; set; } = Path.Combine(AppContext.BaseDirectory, "./logs");


        public int LogRetentionDays { get; set; } = 30;

    }
}