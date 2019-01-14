using System;
using System.Collections.Generic;

namespace Hessian.Net
{
    internal class ObjectPropertyComparer : IComparer<PropertyElement>
    {
        private readonly IComparer<int> comparer;

        public ObjectPropertyComparer()
        {
            comparer = Comparer<int>.Default;
        }

        public int Compare(PropertyElement x, PropertyElement y)
        {
            var eq = comparer.Compare(x.PropertyOrder, y.PropertyOrder);
            return 0 == eq
                ? String.Compare(x.PropertyName, y.PropertyName, StringComparison.Ordinal)
                : eq;
        }
    }
}