using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hessian
{
    public class DictionaryTypeResolver
    {
        private readonly Dictionary<string, Func<IDictionary<object, object>>> constructors;

        public DictionaryTypeResolver()
        {
            constructors = new Dictionary<string, Func<IDictionary<object, object>>> {
                {"System.Collections.Hashtable", DefaultCtor},
                {"System.Collections.Generic.IDictionary`2", DefaultCtor},
                {"System.Collections.Generic.Dictionary`2", DefaultCtor},
                {"System.Collections.IDictionary", DefaultCtor},
                {"java.lang.Map", DefaultCtor},
                {"java.util.HashMap", DefaultCtor},
                {"java.util.EnumMap", DefaultCtor},
                {"java.util.TreeMap", DefaultCtor},
                {"java.util.concurrent.ConcurrentHashMap", DefaultCtor}
            };

        }

        public bool TryGetInstance(string type, out IDictionary<object, object> instance)
        {
            instance = null;

            if (!constructors.TryGetValue(type, out var ctor)) {
                return false;
            }

            instance = ctor();
            return true;
        }

        private static IDictionary<object, object> DefaultCtor()
        {
            return new Dictionary<object, object>();
        }
    }
}
