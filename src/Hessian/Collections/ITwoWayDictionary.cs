using System.Collections.Generic;

namespace Hessian.Collections
{
    /// <summary>
    /// Represents a dictionary which maintains uniqueness of both keys and values,
    /// allowing for the lookup of a key by its value in additon to the normal
    /// operations expected of a dictionary.
    /// </summary>
    public interface ITwoWayDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        /// <summary>
        /// Exposes a view of the current dictionary that reverses keys and
        /// values.  Note that the same underlying data is shared.
        /// </summary>
        ITwoWayDictionary<TValue, TKey> Inverse { get; }

        /// <summary>
        /// Gets or sets a key by the given value.
        /// </summary>
        /// <param name="value">
        /// The value with which to get or set a key.
        /// </param>
        /// <returns>
        /// Returns the key indexing the given <paramref name="value"/>.
        /// </returns>
        TKey this[TValue valueKey] { get; set; }

        /// <summary>
        /// Gets a value indicating whether the given <paramref name="value"/>
        /// is contained in this dictionary.
        /// </summary>
        /// <param name="value">
        /// The value whose presence is to be determined.
        /// </param>
        /// <returns>
        /// Returns <see langword="true"/> if <paramref name="value"/> is in
        /// this dictionary, and <see langword="false"/> otherwise.
        /// </returns>
        bool ContainsValue(TValue value);

        /// <summary>
        /// Attempts to look up a key by the given value.  A return value
        /// indicates whether the lookip is successful.
        /// </summary>
        /// <param name="value">
        /// The value whose corresponding key is to be retrieved.
        /// </param>
        /// <param name="key">
        /// When the method returns, contains the looked-up key if the lookup
        /// succeeded.
        /// </param>
        /// <returns>
        /// Returns <see langword="true"/> if the lookup succeeded, and
        /// <see langword="false"/> otherwise.
        /// </returns>
        bool TryGetKey(TValue value, out TKey key);
    }
}
