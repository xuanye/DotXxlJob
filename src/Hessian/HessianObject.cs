using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Hessian
{
    public class HessianObject : IReadOnlyCollection<Tuple<string, object>>
    {
        private readonly string typeName;
        private readonly IDictionary<string, object> fields; 

        public string TypeName
        {
            get { return typeName; }
        }

        public object this[string key]
        {
            get { return fields[key]; }
        }

        public int Count
        {
            get { return fields.Count; }
        }

        private HessianObject(string typeName)
        {
            this.typeName = Conditions.CheckNotNull(typeName, "typeName");
            fields = new Dictionary<string, object>();
        }

        public IEnumerator<Tuple<string, object>> GetEnumerator()
        {
            return fields.Select(kvp => Tuple.Create(kvp.Key, kvp.Value)).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public class Builder
        {
            private readonly HessianObject obj;

            public HessianObject Object
            {
                get { return obj; }
            }

            private Builder(string typeName)
            {
                obj = new HessianObject(typeName);
            }

            public static Builder New(string typeName)
            {
                return new Builder(typeName);
            }

            public Builder Add(string field, object value)
            {
                obj.fields.Add(field, value);
                return this;
            }

            public HessianObject Create()
            {
                return obj;
            }
        }
    }
}
