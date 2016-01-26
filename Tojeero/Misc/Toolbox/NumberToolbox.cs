using System;

namespace Tojeero.Core.Toolbox
{
	public static class NumberToolbox
	{
		private const int THOUSAND = 1000;
		private const int MILLION = 1000000;
		private const int BILLION = 1000000000;
		private const long TRILLION = 1000000000000;

		public static string GetShortString(this decimal number)
		{
			if (number < 0)
				return "";
			else if (number < THOUSAND)
				return number.ToString("n0");
			else if (number < MILLION)
				return (number / THOUSAND).ToString("n0") + "k";
			else if (number < BILLION)
				return (number / MILLION).ToString("n0") + "m";
			else if (number < TRILLION)
				return (number / BILLION).ToString("n0") + "b";
			else
				return "999b+";
		}
	}
}

