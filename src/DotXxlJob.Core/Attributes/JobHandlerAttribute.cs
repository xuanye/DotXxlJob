using System;

namespace DotXxlJob.Core
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class JobHandlerAttribute:Attribute
    {
        public JobHandlerAttribute(string name)
        {
            Name = name;
        }
        
        public string Name { get; }
        
        /// <summary>
        /// set Ignore 
        /// </summary>
        public bool Ignore { get; set; }
    }
}