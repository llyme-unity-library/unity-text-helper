using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace TextHelper
{
	public static class Regexes
	{
		/// <summary>
		/// Splits the given string with
		/// respect to the matched patterns.
		/// </summary>
		public static IEnumerable<SplitResult> Split2
			(this Regex regex,
			string text)
		{
			int index = 0;

			foreach (Match match in regex.Matches(text))
			{
				if (match.Index > index)
					yield return new()
					{
						Value = text[index..match.Index],
						IsMatch = false
					};

				yield return new()
				{
					Value = match.Value,
					IsMatch = true
				};

				index = match.Index + match.Length;
			}

			if (index < text.Length)
				yield return new()
				{
					Value = text[index..],
					IsMatch = false
				};
		}
	}
}
