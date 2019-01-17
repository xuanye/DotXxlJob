using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DotXxlJob.Core.Model
{
    [DataContract(Name = "com.xxl.rpc.remoting.net.params.XxlRpcRequest")]
    public class RpcRequest
    {
        /*
        requestId
            createMillisTime
        accessToken
            className
        methodName
            version
        parameterTypes
            parameters
        */
        [DataMember(Name = "requestId",Order = 1)]
        public string RequestId { get; set; }
        
        //[DataMember(Name = "serverAddress")]
        //public string ServerAddress{ get; set; }
        
        [DataMember(Name = "createMillisTime" ,Order = 2)]
        public long CreateMillisTime{ get; set; }
        
        
        [DataMember(Name = "accessToken" ,Order = 3)]
        public string AccessToken{ get; set; }
        
        [DataMember(Name = "className" ,Order = 4)]
        public string ClassName{ get; set; }
        
        [DataMember(Name = "methodName" ,Order = 5)]
        public string MethodName{ get; set; }
        
        [DataMember(Name = "version" ,Order = 6)]
        public string Version{ get; set; }
        
        [DataMember(Name = "parameterTypes",Order = 7)]
        public IList<object> ParameterTypes{ get; set; }
        
        
        [DataMember(Name = "parameters",Order = 8)]
        public IList<object> Parameters{ get; set; }
        
       
    }
}