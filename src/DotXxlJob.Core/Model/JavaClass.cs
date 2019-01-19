using System.Runtime.Serialization;

namespace DotXxlJob.Core.Model
{
    [DataContract(Name = Constants.JavaClassFulName)]
    public class JavaClass
    {
        [DataMember(Name = "name",Order = 1)]
        public string Name { get; set; }
    }
}