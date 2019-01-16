using System;

namespace DotXxlJob.Core
{
    public static class DateTimeExtensions
    {
        private const long UnixEpochTicks = 621355968000000000;
        private const long UnixEpochSeconds = 62135596800;
        private const long UnixEpochMilliseconds = 62135596800000;

        public static DateTimeOffset FromUnixTimeSeconds(this long seconds)
        {
            long ticks = seconds * TimeSpan.TicksPerSecond + UnixEpochTicks;
            return new DateTime(ticks, DateTimeKind.Utc);
        }

        public static DateTime FromUnixTimeMilliseconds(this long milliseconds)
        {
            long ticks = milliseconds * TimeSpan.TicksPerMillisecond + UnixEpochTicks;
            return new DateTime(ticks, DateTimeKind.Utc);
        }

        public static long ToUnixTimeSeconds(this DateTime dateTime)
        {
            long seconds = dateTime.ToUniversalTime().Ticks / TimeSpan.TicksPerSecond;
            return seconds - UnixEpochSeconds;
        }

        public static long ToUnixTimeMilliseconds(this DateTime dateTime)
        {
            long milliseconds = dateTime.ToUniversalTime().Ticks / TimeSpan.TicksPerMillisecond;
            return milliseconds - UnixEpochMilliseconds;
        }
    }
}