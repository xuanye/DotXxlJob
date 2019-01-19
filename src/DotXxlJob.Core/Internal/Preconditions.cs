using System;

namespace DotXxlJob.Core
{
    /// <summary>
    /// Utility methods to simplify checking preconditions in the code.
    /// </summary>
    internal static class Preconditions
    {
        /// <summary>
        /// Throws <see cref="ArgumentException"/> if condition is false.
        /// </summary>
        /// <param name="condition">The condition.</param>
        public static void CheckArgument(bool condition)
        {
            if (!condition)
            {
                throw new ArgumentException();
            }
        }

        /// <summary>
        /// Throws <see cref="ArgumentException"/> with given message if condition is false.
        /// </summary>
        /// <param name="condition">The condition.</param>
        /// <param name="errorMessage">The error message.</param>
        public static void CheckArgument(bool condition, string errorMessage)
        {
            if (!condition)
            {
                throw new ArgumentException(errorMessage);
            }
        }

        /// <summary>
        /// Throws <see cref="ArgumentNullException"/> if reference is null.
        /// </summary>
        /// <param name="reference">The reference.</param>
        public static T CheckNotNull<T>(T reference)
        {
            if (reference == null)
            {
                throw new ArgumentNullException();
            }
            return reference;
        }

        /// <summary>
        /// Throws <see cref="ArgumentNullException"/> if reference is null.
        /// </summary>
        /// <param name="reference">The reference.</param>
        /// <param name="paramName">The parameter name.</param>
        public static T CheckNotNull<T>(T reference, string paramName)
        {
            if (reference == null)
            {
                throw new ArgumentNullException(paramName);
            }
            return reference;
        }

        /// <summary>
        /// Throws <see cref="InvalidOperationException"/> if condition is false.
        /// </summary>
        /// <param name="condition">The condition.</param>
        public static void CheckState(bool condition)
        {
            if (!condition)
            {
                throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// Throws <see cref="InvalidOperationException"/> with given message if condition is false.
        /// </summary>
        /// <param name="condition">The condition.</param>
        /// <param name="errorMessage">The error message.</param>
        public static void CheckState(bool condition, string errorMessage)
        {
            if (!condition)
            {
                throw new InvalidOperationException(errorMessage);
            }
        }
    }
}