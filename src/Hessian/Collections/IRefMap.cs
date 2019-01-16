namespace Hessian.Collections
{
    /// <summary>
    /// Represents a map that associates objects with a zero-based integer
    /// index.  Specified in Hessian 2.0.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IRefMap<T>
    {
        /// <summary>
        /// Adds an element to the ref map and returns its ID.
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        int Add(T entry);

        /// <summary>
        /// Retrieves the element identified by the given ID.
        /// </summary>
        /// <param name="refId"></param>
        /// <returns></returns>
        T Get(int refId);

        /// <summary>
        /// Looks up an element in the ref map and, if present, returns its ID.
        /// </summary>
        /// <remarks>
        /// Performance of this method is not guaranteed and is implementation-specific.
        /// </remarks>
        int? Lookup(T entry);
    }
}
