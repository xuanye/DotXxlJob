using System;

namespace Hessian.Net.Extension
{
    /// <summary>
    /// 
    /// </summary>
    internal static class Maybe
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TInput"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="value"></param>
        /// <param name="eval"></param>
        /// <returns></returns>
        public static TResult With<TInput, TResult>(this TInput value, Func<TInput, TResult> eval)
            where TResult : class
        {
            return (null == value) ? null : eval(value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TInput"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="value"></param>
        /// <param name="eval"></param>
        /// <param name="failure"></param>
        /// <returns></returns>
        public static TResult Return<TInput, TResult>(this TInput value, Func<TInput, TResult> eval, TResult failure)
            where TResult : class
        {
            return (null == value) ? failure : eval(value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TInput"></typeparam>
        /// <param name="value"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public static TInput If<TInput>(this TInput value, Predicate<TInput> condition)
            where TInput : class
        {
            if (null == value)
            {
                return null;
            }

            return condition(value) ? value : null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TInput"></typeparam>
        /// <param name="value"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public static TInput Unless<TInput>(this TInput value, Predicate<TInput> condition)
            where TInput : class
        {
            if (null == value)
            {
                return null;
            }

            return condition(value) ? null : value;
        }
    }
}
