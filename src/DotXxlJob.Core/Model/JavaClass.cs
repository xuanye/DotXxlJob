using System.Runtime.Serialization;

namespace DotXxlJob.Core.Model
{
    [DataContract(Name = "java.lang.Class")]
    public class JavaClass
    {
        [DataMember(Name = "name",Order = 1)]
        public string Name { get; set; }
    }
}