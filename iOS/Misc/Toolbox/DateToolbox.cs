using System;
using Foundation;
using System.Globalization;

namespace Tojeero.iOS
{
	public static class DateToolbox
	{
		
		public static DateTime ToDateTime(this NSDate date)
		{
			var reference = new DateTime(2001, 1, 1, 0, 0, 0, DateTimeKind.Utc);
			return reference.AddSeconds(date.SecondsSinceReferenceDate);
		}

	}
}

