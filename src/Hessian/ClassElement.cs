using System.Collections.Generic;

namespace Hessian
{
    public class ClassElement
    {
        public string ClassName { get; set; }
        
        public List<PropertyElement> Fields { get; set; }
    }
}