using System;

namespace Tojeero.Core.Toolbox
{
    public static class DateToolbox
    {
        public static DateTimeOffset UnixTimestampToDateTimeOffset(this long unixTime)
        {
            var epoch = new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero);
            return epoch.AddSeconds(unixTime);
        }

        public static DateTimeOffset TimeTokenToDateTimeOffset(this long timeToken)
        {
            var unixTimestamp = (long) (timeToken/1E+7);
            var date = unixTimestamp.UnixTimestampToDateTimeOffset();
            return date;
        }

        public static long ToUnixTime(this DateTimeOffset date)
        {
            var epoch = new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero);
            var unixTime = (date.ToUniversalTime() - epoch).TotalSeconds;
            return (long) unixTime;
        }

        public static long ToTimeToken(this DateTimeOffset date)
        {
            var unixTimestamp = date.ToUnixTime();
            var timeToken = (long) (unixTimestamp*1E+7);
            return timeToken;
        }
    }
}