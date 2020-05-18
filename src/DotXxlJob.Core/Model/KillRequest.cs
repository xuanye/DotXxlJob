using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace DotXxlJob.Core.Model
{
  
    [DataContract]
    public class KillRequest
    {

        [DataMember(Name = "jobId", Order = 1)]
        public int JobId { get; set; }
    }
}
