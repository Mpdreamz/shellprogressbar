using System;
using System.Collections.Generic;
using System.Text;

namespace ShellProgressBar
{
	internal static class StringExtensions
	{
		public static string Excerpt(string phrase, int length = 60)
		{
			if (string.IsNullOrEmpty(phrase) || phrase.Length < length)
				return phrase;
			return phrase.Substring(0, length - 3) + "...";
		}

		public static int CalcStringWidth(this string message)
		{
#if NETSTANDARD1_3
			return message.Length;
#else
			return UnicodeUtils.GetWidth(message);
#endif
		}
	}
}
