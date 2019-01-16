using System.Collections.Generic;

namespace Hessian.Collections
{
    public class ListRefMap<T> : IRefMap<T>
    {
        private readonly List<T> list = new List<T>();

        public int Add(T entry)
        {
            list.Add(entry);
            return list.Count - 1;
        }

        public T Get(int refId)
        {
            if (refId < 0 || refId >= list.Count) {
                throw new InvalidRefException(refId);
            }
            return list[refId];
        }

        public int? Lookup(T entry)
        {
            for (var i = 0; i < list.Count; ++i) {
                if (entry.Equals(list[i])) {
                    return i;
                }
            }

            return null;
        }
    }
}
