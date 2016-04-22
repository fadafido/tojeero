using System;
using Java.Util;

namespace Tojeero.Droid.Toolbox
{
    public static class DateTimeToolbox
    {
        public static DateTime ToDateTime(this Date date)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddMilliseconds(date.Time);
        }
    }
}