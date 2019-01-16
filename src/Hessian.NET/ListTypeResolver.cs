using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Hessian.Net
{
    public class ListTypeResolver
    {
        private readonly Dictionary<string, Func<IList<object>>> constructors = new Dictionary<string, Func<IList<object>>>();
        private readonly Dictionary<string, Func<int, IList<object>>> length_constructors = new Dictionary<string, Func<int, IList<object>>>();
        private readonly Func<IList<object>> empty_list_ctor = () => new List<object>();
        private readonly Func<int, IList<object>> empty_list_ctor_with_length = length => new List<object>(length);

        public ListTypeResolver()
        {
            constructors.Add("System.Collections.ArrayList", empty_list_ctor);
            constructors.Add("System.Collections.List", empty_list_ctor);
            constructors.Add("System.Collections.IList", empty_list_ctor);
            constructors.Add("System.Collections.Generic.List`1", empty_list_ctor);
            constructors.Add("System.Collections.Generic.IList`1", empty_list_ctor);
            constructors.Add("System.Collections.ObjectModel.Collection`1", () => new Collection<object>());
            constructors.Add("java.util.List", empty_list_ctor);
            constructors.Add("java.util.Vector", empty_list_ctor);
            constructors.Add("java.util.ArrayList", empty_list_ctor);
            constructors.Add("java.util.LinkedList", empty_list_ctor);

            length_constructors.Add("System.Collections.List", empty_list_ctor_with_length);
            length_constructors.Add("System.Collections.IList", empty_list_ctor_with_length);
            length_constructors.Add("System.Collections.Generic.List`1", empty_list_ctor_with_length);
            length_constructors.Add("System.Collections.Generic.IList`1", empty_list_ctor_with_length);
            length_constructors.Add("java.util.List", empty_list_ctor_with_length);
            length_constructors.Add("java.util.Vector", empty_list_ctor_with_length);
            length_constructors.Add("java.util.ArrayList", empty_list_ctor_with_length);
            length_constructors.Add("java.util.LinkedList", empty_list_ctor_with_length);
        }

        public bool TryGetListInstance(string type, out IList<object> list)
        {
            list = null;

            Func<IList<object>> ctor;
            if (!constructors.TryGetValue(type, out ctor))
            {
                return false;
            }

            list = ctor();
            return true;
        }

        public bool TryGetListInstance(string type, int length, out IList<object> list)
        {
            list = null;

            Func<int, IList<object>> ctor;
            if (!length_constructors.TryGetValue(type, out ctor))
            {
                return false;
            }

            list = ctor(length);
            return true;
        }
    }
}
