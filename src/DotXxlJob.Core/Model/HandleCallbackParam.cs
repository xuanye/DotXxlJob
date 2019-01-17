using System.Runtime.Serialization;

namespace DotXxlJob.Core.Model
{
    [DataContract(Name = "com.xxl.job.core.biz.model.HandleCallbackParam")]
    public class HandleCallbackParam
    {
        [DataMember(Name = "callbackRetryTimes",Order = 1)]
        public int CallbackRetryTimes;
        [DataMember(Name = "logId",Order = 2)]
        public int LogId;
        [DataMember(Name = "logDateTim",Order = 3)]
        public long LogDateTim;
        [DataMember(Name = "executeResult",Order = 4)]
        public ReturnT ExecuteResult;
    }
}