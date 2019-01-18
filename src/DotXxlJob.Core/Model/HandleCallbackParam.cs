using System.Runtime.Serialization;

namespace DotXxlJob.Core.Model
{
    [DataContract(Name = "com.xxl.job.core.biz.model.HandleCallbackParam")]
    public class HandleCallbackParam
    {
        public HandleCallbackParam()
        {
            
        }
        public HandleCallbackParam(TriggerParam triggerParam, ReturnT result)
        {
            this.LogId = triggerParam.LogId;
            this.LogDateTime = triggerParam.LogDateTime;
            this.ExecuteResult = result;
        }
        
       
        public int CallbackRetryTimes { get; set; }
        
        [DataMember(Name = "logId",Order = 1)]
        public int LogId { get; set; }
        [DataMember(Name = "logDateTim",Order = 2)]
        public long LogDateTime { get; set; }
        [DataMember(Name = "executeResult",Order = 3)]
        public ReturnT ExecuteResult { get; set; }
    }
}