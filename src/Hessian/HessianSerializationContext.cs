using System;
using System.Collections;
using System.Collections.Generic;

namespace Hessian
{
    public class HessianSerializationContext
    {
        public IList<Type> Classes
        {
            get;
            private set;
        }

        public IList Instances
        {
            get;
            private set;
        }

        public HessianSerializationContext()
        {
            Classes = new List<Type>();
            Instances = new List<object>();
        }
    }
}