using UnityEngine;

namespace Quinn
{
	public static class StringExtensions
	{
		public static string ToBold(this string str)
		{
			return $"<b>{str}</b>";
		}
		public static string ToItalic(this string str)
		{
			return $"<i>{str}</i>";
		}
		public static string ToBoldItalic(this string str)
		{
			return $"<b><i>{str}</i></b>";
		}

		public static string ToColor(this string str, string color)
		{
			return $"<color={color}>{str}</color>";
		}
		public static string ToColor(this string str, Color color)
		{
			string sCol = color.ToHexString();
			return $"<color=#{sCol}>{str}</color>";
		}

		public static string ToHexString(this Color color)
		{
			return
				((byte)(color.r * 255)).ToString("X2") +
				((byte)(color.g * 255)).ToString("X2") +
				((byte)(color.b * 255)).ToString("X2") +
				((byte)(color.a * 255)).ToString("X2");
		}

		public static string Colorized(this string str)
		{
			return str
				.Replace("$$", "</color>")
				.Replace("$red", "<color=red>")
				.Replace("$blu", "<color=blue>")
				.Replace("$gre", "<color=green>")
				.Replace("$cya", "<color=cyan>")
				.Replace("$mag", "<color=magenta>")
				.Replace("$bla", "<color=black>")
				.Replace("$whi", "<color=white>");
		}
	}
}
