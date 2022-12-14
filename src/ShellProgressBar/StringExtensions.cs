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

        /// <summary>
        /// Splits a string into it's indiviudal lines and then again splits these individual lines
        /// into multiple lines if they exceed the width of the console.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static IEnumerable<string> SplitToConsoleLines(this string str)
        {
	        int width = Console.BufferWidth;
	        var lines = str.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

	        foreach (var line in lines)
	        {
		        if (line.Length > width)
		        {
			        for (int i = 0; i < line.Length; i += width)
			        {
				        yield return line.Substring(i, Math.Min(width, line.Length - i));
			        }
		        }
		        else
		        {
			        yield return line;
		        }
	        }
        }
    }
}
