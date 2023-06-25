using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using CollectionHelper;
using UnityEditor;
using UnityEngine;

namespace TextHelper
{
	public static class Strings
	{
		public readonly static Regex BRACKETS =
			new(@"(?<!\\)\[((\\\[|\\\])*|[^\[\]]*)*(?<!\\)\]");
				
		public static int StringIndexOf
			(this IEnumerable<string> enumerable,
			string value,
			int @default = -1,
			StringComparison comparison = StringComparison.OrdinalIgnoreCase)
		{
			int index = 0;

			foreach (string item in enumerable)
			{
				if (string.Equals(value, item, comparison))
					return index;

				index++;
			}

			return @default;
		}

		public static bool Is
			(this string a,
			string b,
			StringComparison comparison = StringComparison.OrdinalIgnoreCase) =>
			string.Equals(a, b, comparison);

		public static bool Is
			(this string a,
			IEnumerable<string> b,
			StringComparison comparison = StringComparison.OrdinalIgnoreCase) =>
			b.Any(v => string.Equals(a, v, comparison));

		/// <summary>
		/// Trims whitespaces on ends,
		/// replaces dots `.` with underscores `_`,
		/// and all uppercase.
		/// </summary>
		public static string IDfy(this string id)
		{
			if (string.IsNullOrEmpty(id))
				return string.Empty;

			if (id.Contains('.'))
				Debug.LogWarning("IDs should not contain dots!");

			return id.Trim().Replace('.', '_').ToUpper();
		}

		/// <summary>
		/// Splits the string, then trims it.
		/// `RemoveEmptyEntries` also applies after trimmed.
		/// </summary>
		public static IEnumerable<string> SplitTrim
			(this string text,
			string separator,
			StringSplitOptions options = StringSplitOptions.RemoveEmptyEntries)
		{
			if (string.IsNullOrWhiteSpace(text))
				return Enumerable.Empty<string>();

			IEnumerable<string> split =
				text.Split(separator, options);

			if (options == StringSplitOptions.RemoveEmptyEntries)
				split = split.Where(v => !string.IsNullOrWhiteSpace(v));

			return split.Select(v => v.Trim());
		}

		/// <summary>
		/// Splits the string into 2, where the separator is.
		/// Separator is excluded.
		/// </summary>
		public static string[] SplitAt(this string self, string separator)
		{
			int index = self.IndexOf(separator);

			if (index == -1)
				return new string[] { self, string.Empty };

			return new string[]
			{
				self.Substring(0, index),
				self[(index + separator.Length)..]
			};
		}

		/// <summary>
		/// Stringifies the bound's center and size properties
		/// into 6 comma-separated numbers.
		/// </summary>
		public static string Stringify(this Bounds v) =>
			$"{v.center.x}, {v.center.y}, {v.center.z}, {v.size.x}, {v.size.y}, {v.size.z}";

		public static string Stringify(this Vector2 v) =>
			$"{v.x}, {v.y}";

		/// <summary>
		/// Clears and trims any extra whitespaces.
		/// <br></br>
		/// Restricts only 1 whitespace character
		/// between non-whitespace characters.
		/// <br></br>
		/// Restricts only 1 newline character
		/// between non-newline characters.
		/// </summary>
		public static string NormalizeWhiteSpace(this string text)
		{
			string[] texts = text.Trim().Split(
				new string[] { "\r\n", "\n" },
				StringSplitOptions.None
			);
			StringBuilder sb = new();
			// Adds a newline when `true`.
			bool newline = true;
			// Adds space after each newline when `true`.
			bool newline_space = false;

			for (int i = 0; i < texts.Length; i++)
			{
				if (string.IsNullOrWhiteSpace(texts[i]))
				{
					if (newline)
					{
						sb.AppendLine();
						newline_space = false;
						newline = false;
					}

					continue;
				}

				string text0 = texts[i].Trim();
				// Registers spaces when `true`.
				bool whitespace = true;

				if (newline_space)
					sb.Append(" ");

				foreach (char c in text0)
				{
					if (char.IsWhiteSpace(c))
						if (whitespace)
							whitespace = false;
						else
							continue;
					else
						whitespace = true;

					sb.Append(c);
				}

				newline = true;
				newline_space = true;
			}

			return sb.ToString();
		}

		public static string ExtendedFormat
			(this string text,
			string format)
		{
			switch (format.Enum(TextHelper.ExtendedFormat.Unknown))
			{
				case TextHelper.ExtendedFormat.Abs:
					if (float.TryParse(text, out float abs))
						text = Mathf.Abs(abs).ToString();

					break;

				case TextHelper.ExtendedFormat.PercentCompliment:
					if (float.TryParse(text, out float compliment))
						text = (1f - compliment).ToString();

					break;

				case TextHelper.ExtendedFormat.PercentMinusOne:
					if (float.TryParse(text, out float percentMinusOne))
						text = (percentMinusOne - 1f).ToString();

					break;

				case TextHelper.ExtendedFormat.Unknown:
				default:
					string format0 = $"{{0:{format}}}";

					if (float.TryParse(text, out float number))
						text = string.Format(format0, number);
					else
						text = string.Format(format0, text);

					break;
			}

			return text;
		}

		public static string FormatParams
			(this string text,
			Func<string, string> predicate)
		{
			IEnumerable<SplitResult> split =
				BRACKETS.Split2(text);

			string Select(SplitResult result)
			{
				if (!result.IsMatch)
					return result.Value;

				if (result.Value.Length <= 2)
					return result.Value;

				string[] split = result.Value[1..^1].Split(
					':',
					StringSplitOptions.RemoveEmptyEntries
				);

				if (split.Length == 0)
					return result.Value;

				split[0] = predicate(split[0]);

				if (split.Length == 1)
					return split[0];

				for (int i = 1; i < split.Length; i++)
					split[0] = split[0].ExtendedFormat(split[i]);

				return split[0];
			}

			return
				string.Join(string.Empty, split.Select(Select))
				.Replace("\\[", "[")
				.Replace("\\]", "]");
		}

		public static T Enum<T>
			(this string text,
			T @default = default)
			where T : struct =>
			System.Enum.TryParse(text, true, out T value)
			? value
			: @default;

		/// <summary>
		/// Creates a `Bounds` by center and size
		/// in a form of 6 vectors separated
		/// by commas.
		/// </summary>
		public static Bounds Bounds(this string text)
		{
			float[] array = text
				.Split(',')
				.Select(v => float.TryParse(v, out float value) ? value : 0)
				.TakeExactly(6);

			return new Bounds(
				new Vector3(array[0], array[1], array[2]),
				new Vector3(array[3], array[4], array[5])
			);
		}

		public static bool Bool(this string text, bool @default = false) =>
			bool.TryParse(text, out bool result) ? result : @default;

		public static Color Color(this string text)
		{
			if (string.IsNullOrEmpty(text))
				return UnityEngine.Color.white;

			text = text.Trim();


			// Hexadecimal and Words

			if (ColorUtility.TryParseHtmlString(text, out Color color))
				return color;


			// RGB(A)

			if (text.StartsWith("(") && text.EndsWith(")"))
				text = text[1..^1];
			else if (text.StartsWith("RGB(", StringComparison.OrdinalIgnoreCase) && text.EndsWith(")"))
				text = text[4..^1];
			else if (text.StartsWith("RGBA(", StringComparison.OrdinalIgnoreCase) && text.EndsWith(")"))
				text = text[5..^1];

			float[] array = text
				.Split(',')
				.Select(v => float.TryParse(v, out float value) ? value : 1f)
				.TakeExactly(4);

			return new Color(array[0], array[1], array[2], array[3]);
		}

		public static Vector2Int Vector2Int(this string text)
		{
			if (string.IsNullOrEmpty(text))
				return UnityEngine.Vector2Int.zero;

			if (text.StartsWith("(") && text.EndsWith(")"))
				text = text[1..^1];

			int[] array = text
				.Split(',')
				.Select(v => int.TryParse(v, out int value) ? value : 0)
				.TakeExactly(2);

			return new Vector2Int(array[0], array[1]);
		}

		public static Vector2 Vector2(this string text, Vector2 @default = default)
		{
			if (Vector2(text, out Vector2 value))
				return value;

			return @default;
		}

		public static bool Vector2(this string text, out Vector2 value)
		{
			if (string.IsNullOrEmpty(text))
			{
				value = default;
				return false;
			}

			if (text.StartsWith("(") && text.EndsWith(")"))
				text = text[1..^1];

			float[] array = text
				.Split(',')
				.Select(v => float.TryParse(v, out float value) ? value : 0f)
				.TakeExactly(2);

			value = new Vector2(array[0], array[1]);
			return true;
		}

		public static Vector3 Vector3(this string text, params float[] defaults)
		{
			defaults = defaults.TakeExactly(3);

			if (string.IsNullOrEmpty(text))
				return new Vector3(
					defaults[0],
					defaults[1],
					defaults[2]
				);

			if (text.StartsWith("(") && text.EndsWith(")"))
				text = text[1..^1];

			float[] array = text
				.Split(',')
				.Select(v => float.TryParse(v, out float value) ? value : 0f)
				.TakeExactly(3, defaults);

			return new Vector3(array[0], array[1], array[2]);
		}

		public static Vector3[] Vector3s(this string text)
		{
			Regex regex = new(@"\([^\(\)]+\)");
			List<Vector3> result = new();

			foreach (Match match in regex.Matches(text))
				if (match.Success)
					result.Add(Vector3(match.Value));

			return result.ToArray();
		}

		public static Vector4 Vector4(this string text)
		{
			if (string.IsNullOrEmpty(text))
				return UnityEngine.Vector4.zero;

			if (text.StartsWith("(") && text.EndsWith(")"))
				text = text[1..^1];

			float[] array = text
				.Split(',')
				.Select(v => float.TryParse(v, out float value) ? value : 0f)
				.TakeExactly(4);

			return new Vector4(array[0], array[1], array[2], array[3]);
		}

		public static Quaternion Quaternion(this string text)
		{
			if (string.IsNullOrEmpty(text))
				return UnityEngine.Quaternion.identity;

			if (text.StartsWith("(") && text.EndsWith(")"))
				text = text[1..^1];

			float[] array = text
				.Split(',')
				.Select(v => float.TryParse(v, out float value) ? value : 0f)
				.TakeExactly(4);

			return new Quaternion(array[0], array[1], array[2], array[3]);
		}

		public static float Float
			(this string text,
			float @default = 0f) =>
			float.TryParse(text, out float value)
			? value
			: @default;

		public static int Int
			(this string text,
			int @default = 0) =>
			int.TryParse(text, out int value)
			? value
			: @default;

		/// <summary>
		/// Trim then uppercase.
		/// Used as an alternative for lambda functions.
		/// </summary>
		public static string TrimUpper(string text) => text?.Trim()?.ToUpper();

		/// <summary>
		/// Used as an alternative for lambda functions.
		/// </summary>
		public static string Trim(string text) => text?.Trim();

		/// <summary>
		/// Automatically trims.
		/// </summary>
		public static IEnumerable<string> Split
			(string text,
			bool uppercase = true,
			params string[] separators)
		{
			if (text == null)
				return new string[0];

			string[] texts = text.Split(
				separators,
				StringSplitOptions.RemoveEmptyEntries
			);

			return texts.Select(v =>
			{
				v = v.Trim();

				if (uppercase)
					v = v.ToUpper();

				return v;
			});
		}

		public static string LetterCase
			(this string text,
			LetterCase lettercase) =>
			string.IsNullOrEmpty(text)
			? text
			: lettercase switch
			{
				TextHelper.LetterCase.Lowercase => text.ToLower(),
				TextHelper.LetterCase.Uppercase => text.ToUpper(),
				_ => text,
			};

		/// <summary>
		/// Prints a number as a comma-separated by thousands.
		/// Decimals only appear when necessary,
		/// up to 2 decimal places.
		/// </summary>
		public static string Thousands(this float number) =>
			number.ToString("#,0.##");

		/// <summary>
		/// Prints a number as a comma-separated by thousands.
		/// Decimals only appear when necessary,
		/// up to 2 decimal places.
		/// </summary>
		public static string Thousands(this int number) =>
			number.ToString("#,0.##");
	}
}
