// Copyright (c) Xuanye Wang. All rights reserved.
// Licensed under MIT license

using System.Runtime.Serialization;

namespace DotXxlJob.Core.Models
{
    [DataContract]
    public class IdleBeatCommand
    {
        [DataMember(Name = "jobId", Order = 1)]
        public int JobId { get; set; }
    }
}