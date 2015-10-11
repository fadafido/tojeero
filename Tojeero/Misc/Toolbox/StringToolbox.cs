using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

namespace Tojeero.Core.Toolbox
{
	public static class StringToolbox
	{
		private static Regex stringTokenizer = new Regex(@"[^A-Za-z\u0600-\u06FF]+");

		/// <summary>
		/// Tokenize the specified string value.
		/// </summary>
		/// <returns>
		/// Returns string list which will contain only the unique word tokens in lower case which have >= 2 length from initial value.
		/// </returns>
		/// <param name="value">String value.</param>
		public static List<string> Tokenize(this string value)
		{
			if (value == null)
				return null;
			var tokens = stringTokenizer.Split(value);
			HashSet<string> uniqueTokens = new HashSet<string>();
			uniqueTokens.AddRange(tokens.Where(t => t.Length >= 2).Select(t => t.ToLower()));
			return uniqueTokens.ToList();
		}

		/// <summary>
		/// Tokenize the specified string values.
		/// </summary>
		/// <returns>
		/// Returns string list which will contain only the unique word tokens in lower case from each of the initial string values.
		/// </returns>
		/// <param name="values">Values.</param>
		public static List<string> Tokenize(this IEnumerable<string> values)
		{
			if (values == null)
				return null;
			HashSet<string> tokens = new HashSet<string>();
			foreach (var value in values)
			{
				tokens.AddRange(value.Tokenize());
			}
			return tokens.ToList();
		}
	}
}

