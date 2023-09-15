using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace TextHelper
{
	public static partial class Strings
	{
		public static string Stringify(this Gradient gradient)
		{
			StringBuilder result = new();
			int colorMax = gradient.colorKeys.Length - 1;
			int alphaMax = gradient.alphaKeys.Length - 1;
			
			for (int i = 0; i < gradient.colorKeys.Length; i++)
			{
				GradientColorKey key = gradient.colorKeys[i];
				result.Append(key.color.r);
				result.Append(',');
				result.Append(key.color.g);
				result.Append(',');
				result.Append(key.color.b);
				result.Append('@');
				result.Append(key.time);
				
				if (i != colorMax)
					result.Append('|');
			}
			
			for (int i = 0; i < gradient.alphaKeys.Length; i++)
			{
				GradientAlphaKey key = gradient.alphaKeys[i];
				result.Append(key.alpha);
				result.Append('@');
				result.Append(key.time);
				
				if (i != alphaMax)
					result.Append('|');
			}
			
			return result.ToString();
		}
		
		public static bool Gradient
		(this string text,
		out Gradient gradient)
		{
			if (string.IsNullOrEmpty(text))
			{
				gradient = null;
				return false;
			}

			IEnumerable<string[]> keys =
				text
				.Split('|', StringSplitOptions.RemoveEmptyEntries)
				.Select(v => v.Split('@', StringSplitOptions.RemoveEmptyEntries))
				.Select(v => v.Trim())
				.Where(v => v.Length > 0);
			List<GradientColorKey> colors = new();
			List<GradientAlphaKey> alphas = new();
				
			foreach (string[] split in keys)
			{
				bool ok = split[0].Color(out Color color);
				float alpha =
					split.Length > 1
					? split[1].Float()
					: 0f;
				
				if (ok)
					colors.Add(new(color, alpha));
				else
					alphas.Add(new(split[0].Float(), alpha));
			}
			
			gradient = new();
			
			gradient.SetKeys(
				colors.ToArray(),
				alphas.ToArray()
			);
			
			return true;
		}
	}
}
