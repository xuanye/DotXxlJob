namespace Hessian.Collections
{
    public class TwoWayDictionaryRefMap<T> : IRefMap<T>
    {
        private readonly TwoWayDictionary<T, int> map = new TwoWayDictionary<T, int>();

        public int Add(T value)
        {
            var refid = map.Count;
            map.Add(value, refid);
            return refid;
        }

        public T Get(int refid)
        {
            T entry;
            if (map.TryGetKey(refid, out entry)) {
                return entry;
            }

            throw new InvalidRefException(refid);
        }

        public int? Lookup(T entry)
        {
            int refId;
            if (map.TryGetValue(entry, out refId)) {
                return refId;
            }

            return null;
        }
    }
}
