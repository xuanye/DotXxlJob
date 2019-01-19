using System.Runtime.Serialization;

namespace DotXxlJob.Core.Model
{
    [DataContract(Name = Constants.LogResultJavaFullName)]
    public class LogResult
    {

        public LogResult(int fromLine ,int toLine,string content,bool isEnd)
        {
            this.FromLineNum = fromLine;
            this.ToLineNum = toLine;
            this.LogContent = content;
            this.IsEnd = isEnd;
        }
        
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