using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace DotXxlJob.Core.Model
{
    [DataContract]
    public class LogRequest
    {
        [DataMember(Name = "logDateTim", Order =1)]
        public long LogDateTime { get; set; }

        [DataMember(Name = "logId", Order = 2)]
        public int LogId { get; set; }

        [DataMember(Name = "fromLineNum", Order = 3)]
        public int FromLineNum { get; set; }

    }
}
