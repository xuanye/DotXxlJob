using System.Collections;
using System.Collections.Generic;

namespace Hessian.Collections
{
    public abstract class ForwardingDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        protected abstract IDictionary<TKey, TValue> Delegate { get; }

        public virtual ICollection<TKey> Keys
        {
            get { return Delegate.Keys; }
        }

        public virtual ICollection<TValue> Values
        {
            get { return Delegate.Values; }
        }

        public virtual IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return Delegate.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public virtual void Add(KeyValuePair<TKey, TValue> item)
        {
            Delegate.Add(item);
        }

        public virtual void Clear()
        {
            Delegate.Clear();
        }

        public virtual bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return Delegate.Contains(item);
        }

        public virtual void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            Delegate.CopyTo(array, arrayIndex);
        }

        public virtual bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return Delegate.Remove(item);
        }

        public virtual int Count
        {
            get { return Delegate.Count; }
        }

        public virtual bool IsReadOnly
        {
            get { return Delegate.IsReadOnly; }
        }

        public virtual bool ContainsKey(TKey key)
        {
            return Delegate.ContainsKey(key);
        }

        public virtual void Add(TKey key, TValue value)
        {
            Delegate.Add(key, value);
        }

        public virtual bool Remove(TKey key)
        {
            return Delegate.Remove(key);
        }

        public virtual bool TryGetValue(TKey key, out TValue value)
        {
            return Delegate.TryGetValue(key, out value);
        }

        public virtual TValue this[TKey key]
        {
            get { return Delegate[key]; }
            set { Delegate[key] = value; }
        }
    }
}
