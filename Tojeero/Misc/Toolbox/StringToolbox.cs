using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Tojeero.Core.Toolbox
{
	public static class StringToolbox
	{
		private static Regex stringTokenizer = new Regex(@"\s");
		public static string[] Tokenize(this string value)
		{
			if (value == null)
				return null;
			var tokens = stringTokenizer.Split(value);
			return tokens;
		}

		public static string[] Tokenize(this IEnumerable<string> values)
		{
			if (values == null)
				return null;
			List<string> tokens = new List<string>();
			foreach (var value in values)
			{
				tokens.AddRange(value.Tokenize());
			}
			return tokens.ToArray();
		}
	}
}

