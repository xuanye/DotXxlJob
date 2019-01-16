using System.Collections.Generic;

namespace Hessian.Collections
{
    public class TwoWayDictionary<TKey, TValue> : ForwardingDictionary<TKey, TValue>, ITwoWayDictionary<TKey, TValue>
    {
        private readonly IDictionary<TKey, TValue> dict;
        private readonly TwoWayDictionary<TValue, TKey> inverse;

        protected override IDictionary<TKey, TValue> Delegate {
            get { return dict; }
        }

        public override bool IsReadOnly {
            get { return false; }
        }

        public ITwoWayDictionary<TValue, TKey> Inverse {
            get { return inverse; }
        }

        public override TValue this[TKey key] {
            set { UpdateDictAndInverse(key, value, false); }
        }

        public TKey this[TValue valueKey] {
            get { return inverse[valueKey]; }
            set { inverse[valueKey] = value; }
        }

        public override ICollection<TValue> Values {
            get { return inverse.dict.Keys; }
        } 

        public TwoWayDictionary()
            : this(new Dictionary<TKey, TValue>(), new Dictionary<TValue, TKey>())
        {
        }

        public TwoWayDictionary(IDictionary<TKey, TValue> forwards, IDictionary<TValue, TKey> backwards)
        {
            dict = Conditions.CheckNotNull(forwards, "forwards");
            inverse = new TwoWayDictionary<TValue, TKey>(backwards, this);
        }

        private TwoWayDictionary(IDictionary<TKey, TValue> dict, TwoWayDictionary<TValue, TKey> inverse)
        {
            this.dict = Conditions.CheckNotNull(dict, "dict");
            this.inverse = inverse;
        }

        public bool ContainsValue(TValue value)
        {
            return inverse.ContainsKey(value);
        }

        public bool TryGetKey(TValue value, out TKey key)
        {
            return inverse.TryGetValue(value, out key);
        }

        public override void Add(TKey key, TValue value)
        {
            UpdateDictAndInverse(key, value, true);
        }

        public override bool Remove(TKey key)
        {
            return RemoveFromDictAndInverse(key);
        }

        private void UpdateDictAndInverse(TKey key, TValue value, bool throwIfContained)
        {
            if (!throwIfContained) {
                dict.Remove(key);
                inverse.dict.Remove(value);
            }

            dict.Add(key, value);
            inverse.dict.Add(value, key);
        }

        private bool RemoveFromDictAndInverse(TKey key)
        {
            TValue value;
            if (!TryGetValue(key, out value)) {
                return false;
            }

            return RemoveFromDictAndInverse(key, value);
        }

        private bool RemoveFromDictAndInverse(TKey key, TValue value)
        {
            if (!ContainsKey(key) || !ContainsValue(value)) {
                return false;
            }

            return dict.Remove(key) && inverse.dict.Remove(value);
        }

        #region ICollection<KeyValuePair<TKey, TValue>>


        public override bool Remove(KeyValuePair<TKey, TValue> kvp)
        {
            return RemoveFromDictAndInverse(kvp.Key, kvp.Value);
        }

        public override void Clear()
        {
            dict.Clear();
            inverse.dict.Clear();
        }

        public override bool Contains(KeyValuePair<TKey, TValue> kvp)
        {
            return ContainsKey(kvp.Key) && ContainsValue(kvp.Value);
        }

        #endregion
    }
}
