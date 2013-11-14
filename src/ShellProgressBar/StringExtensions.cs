using System;
using System.Collections.Generic;
using System.Text;

namespace ShellProgressBar
{
    public static class StringExtensions
    {
        public static string Excerpt(string phrase, int length = 60)
        {
            if (string.IsNullOrEmpty(phrase) || phrase.Length < length)
                return phrase;
            return phrase.Substring(0, length - 3) + "...";
        }
    }
}
