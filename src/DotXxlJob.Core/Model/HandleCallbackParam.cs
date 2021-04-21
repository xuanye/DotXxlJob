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


        /// <summary>
        /// 2.3.0以前版本
        /// </summary>
        [DataMember(Name = "executeResult",Order = 3)]
        public ReturnT ExecuteResult { get; set; }

        /// <summary>
        /// 2.3.0版本使用的参数
        /// </summary>
        [DataMember(Name = "handleCode", Order = 4)]
        public int HandleCode { 
            get {
                if(this.ExecuteResult != null)
                {
                    return this.ExecuteResult.Code;
                }
                return 500;
            } 
        }

        /// <summary>
        /// 2.3.0版本使用的参数
        /// </summary>
        [DataMember(Name = "handleMsg", Order = 5)]
        public string HandleMsg {
            get {
                return this.ExecuteResult?.Msg;
            }
        }
    }
}