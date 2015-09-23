using System;

namespace Tojeero.Core.Toolbox
{
	public static class DateToolbox
	{
		public static DateTimeOffset UnixToDateTimeOffset(this long unixTime)
		{
			var epoch = new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero);
			return epoch.AddSeconds(unixTime);
		}
	}
}

