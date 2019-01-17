using System.Runtime.Serialization;

namespace DotXxlJob.Core.Model
{
    [DataContract(Name = "com.xxl.job.core.biz.model.LogResult")]
    public class LogResult
    {
        [DataMember(Name = "fromLineNum",Order = 1)]
        public int FromLineNum { get; set; }
        [DataMember(Name = "toLineNum",Order = 2)]
        public int ToLineNum { get; set; }
        [DataMember(Name = "logContent",Order = 3)]
        public string LogContent { get; set; }
        [DataMember(Name = "isEnd",Order = 4)]
        public bool IsEnd { get; set; }
    }
}