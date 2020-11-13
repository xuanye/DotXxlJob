using System.Runtime.Serialization;

namespace DotXxlJob.Core.Model
{
    [DataContract(Name = Constants.HandleCallbackParamJavaFullName)]
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
        public long LogId { get; set; }
        [DataMember(Name = "logDateTim",Order = 2)]
        public long LogDateTime { get; set; }
        [DataMember(Name = "executeResult",Order = 3)]
        public ReturnT ExecuteResult { get; set; }
    }
}