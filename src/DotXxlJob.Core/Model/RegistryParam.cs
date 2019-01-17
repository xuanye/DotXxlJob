using System.Runtime.Serialization;

namespace DotXxlJob.Core.Model
{
    [DataContract(Name = "")]
    public class RegistryParam
    {
        [DataMember(Name = "registGroup",Order = 1)]
        public string RegistryGroup { get; set; }
        [DataMember(Name = "registryKey",Order = 2)]
        public string RegistryKey { get; set; }
        [DataMember(Name = "registryValue",Order = 3)]
        public string RegistryValue { get; set; }

    }
}