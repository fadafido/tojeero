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
		/// The resulting tokens also will be sorted.
		/// </returns>
		/// <param name="value">String value.</param>
		public static List<string> Tokenize(this string value)
		{
			if (value == null)
				return null;
			var tokens = stringTokenizer.Split(value);
			HashSet<string> uniqueTokens = new HashSet<string>();
			uniqueTokens.AddRange(tokens.Where(t => t.Length >= 2).Select(t => t.ToLower()));
			var sorted = uniqueTokens.ToList();
			sorted.Sort();
			return sorted;
		}

		/// <summary>
		/// Tokenize the specified string values.
		/// </summary>
		/// <returns>
		/// Returns string list which will contain only the unique word tokens in lower case from each of the initial string values.
		/// The resulting tokens also will be sorted.
		/// </returns>
		/// <param name="values">Values.</param>
		public static List<string> Tokenize(this IEnumerable<string> values)
		{
			if (values == null)
				return null;
			HashSet<string> uniqueTokens = new HashSet<string>();
			foreach (var value in values)
			{
				if (value != null)
				{
					var tokens = Tokenize(value);
					uniqueTokens.AddRange(tokens);
				}
			}
			var sorted = uniqueTokens.ToList();
			sorted.Sort();
			return sorted;
		}

		/// <summary>
		/// Compares two strings ignoring case.
		/// </summary>
		/// <returns>The compare result of two strings</returns>
		/// <param name="str1">Str1.</param>
		/// <param name="str2">Str2.</param>
		public static int CompareIgnoreCase(this string str1, string str2)
		{
			return string.Compare(str1, str2, StringComparison.CurrentCultureIgnoreCase);
		}

		public static string Truncate(this string str, int maxLength)
		{
			var length = str.Length;
			string result = str;
			if (length > maxLength)
			{
				string dots = "...";
				result = str.Substring(0, maxLength - dots.Length) + dots;
			}
			return result;
		}
	}
}

