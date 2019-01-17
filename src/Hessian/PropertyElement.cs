using System;
using System.Collections.Generic;
using System.Reflection;

namespace Hessian
{
    public class PropertyElement
    {
        public string Name { get; set; }
        public int Order { get; set; }
        public PropertyInfo PropertyInfo { get; set; }
    }
    
    internal class PropertyComparer : IComparer<PropertyElement>
    {
        private readonly IComparer<int> comparer;

        public PropertyComparer()
        {
            comparer = Comparer<int>.Default;
        }

        public int Compare(PropertyElement x, PropertyElement y)
        {
            var eq = comparer.Compare(x.Order, y.Order);
            return 0 == eq
                ? String.Compare(x.Name, y.Name, StringComparison.Ordinal)
                : eq;
        }
    }
}