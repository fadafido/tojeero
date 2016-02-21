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
			var unixTimestamp = (long)(timeToken/1E+7);
			var date = unixTimestamp.UnixTimestampToDateTimeOffset();
			return date;
		}
	}
}

