using System.Runtime.Serialization;

namespace DotXxlJob.Core.Model
{
    
    [DataContract(Name = "triggerParam")]
    public class TriggerParam
    {
        static readonly long SerialVersionUID = 42L;

        [DataMember(Name = "jobId")]
        public int JobId { get; set; }

        [DataMember(Name = "executorHandler")]
        public string ExecutorHandler { get; set; }
        [DataMember(Name = "executorParams")]
        public string ExecutorParams{ get; set; }
        
        [DataMember(Name = "executorBlockStrategy")]
        public string ExecutorBlockStrategy{ get; set; }
        
        [DataMember(Name = "executorTimeout")]
        public int ExecutorTimeout{ get; set; }
        
        [DataMember(Name = "logId")]
        public int LogId{ get; set; }
        [DataMember(Name = "logDateTim")]
        public long LogDateTime{ get; set; }
        

        [DataMember(Name = "glueType")]
        public string GlueType{ get; set; }
        
        [DataMember(Name = "glueSource")]
        public string GlueSource{ get; set; }
        
        [DataMember(Name = "glueUpdatetime")]
        public long GlueUpdateTime{ get; set; }

        [DataMember(Name = "broadcastIndex")]
        public int BroadcastIndex{ get; set; }
        [DataMember(Name = "broadcastTotal")]
        public int BroadcastTotal{ get; set; }
    }
}