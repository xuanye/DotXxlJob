using System.Runtime.Serialization;

namespace DotXxlJob.Core.Model
{
    
    [DataContract]
    public class TriggerParam
    {
        //static readonly long SerialVersionUID = 42L;

        [DataMember(Name = "jobId", Order = 1)]
        public int JobId { get; set; }

        [DataMember(Name = "executorHandler", Order = 2)]
        public string ExecutorHandler { get; set; }
        [DataMember(Name = "executorParams", Order = 3)]
        public string ExecutorParams{ get; set; }
        
        [DataMember(Name = "executorBlockStrategy", Order = 4)]
        public string ExecutorBlockStrategy{ get; set; }
        
        [DataMember(Name = "executorTimeout", Order = 5)]
        public int ExecutorTimeout{ get; set; }
        
        [DataMember(Name = "logId",Order = 5)]
        public long LogId { get; set; }
        [DataMember(Name = "logDateTime", Order = 6)]
        public long LogDateTime{ get; set; }
        

        [DataMember(Name = "glueType",Order = 7)]
        public string GlueType{ get; set; }
        
        [DataMember(Name = "glueSource",Order = 8)]
        public string GlueSource{ get; set; }
        
        [DataMember(Name = "glueUpdatetime", Order = 9)]
        public long GlueUpdateTime{ get; set; }

        [DataMember(Name = "broadcastIndex",Order = 10)]
        public int BroadcastIndex{ get; set; }
        [DataMember(Name = "broadcastTotal",Order = 11)]
        public int BroadcastTotal{ get; set; }
    }
}