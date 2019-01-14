using System;

namespace Hessian.Net.Extension
{
    /// <summary>
    /// 
    /// </summary>
    internal static class DateTimeExtension
    {
        private const long Era = 62135596800000L;
        private const long Millis = 60000;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static long GetTotalMilliseconds(this DateTime dt)
        {
            return dt.ToUniversalTime().Ticks / 10000 - Era; 
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static int GetTotalMinutes(this DateTime dt)
        {
            var val = GetTotalMilliseconds(dt);
            return (int)(val / Millis);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static DateTime FromMinutes(int value)
        {
            var ticks = (value * Millis + Era) * 10000;
            return new DateTime(ticks, DateTimeKind.Utc);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static DateTime FromMilliseconds(long value)
        {
            var ticks = (value + Era) * 10000;
            return new DateTime(ticks, DateTimeKind.Utc);
        }
    }
}
