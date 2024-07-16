using System;
using System.Collections.Generic;
using System.Linq;

namespace ShellProgressBar
{
	internal static class StringExtensions
	{
		public static string Excerpt(string phrase, int wantColumnWidth = 60)
		{
			if (string.IsNullOrEmpty(phrase))
				return phrase;

			int phraseColumnWidth = phrase.GetColumnWidth();
			if (phraseColumnWidth <= wantColumnWidth)
				return phrase;

			while (phraseColumnWidth > wantColumnWidth - 3 &&
				phrase.Length > 0)
			{
				int len = phraseColumnWidth - (wantColumnWidth - 3);
				//assume: max width of one char is 2.
				phrase = phrase.Substring(0, phrase.Length - Math.Max(len / 2, 1));
				phraseColumnWidth = phrase.GetColumnWidth();
			}

			if (phraseColumnWidth == wantColumnWidth - 4)
				return phrase + "... ";

			return phrase + "...";
		}

		public static string WhiteSpace(int n)
		{
			return new string(' ', n);
		}

		//add ' ' to the right of `s` to `wantColumnWidth` width.
		//turncated `s` if the column width is too long.
		public static string TurncatedRightPad(string s, int wantColumnWidth)
		{
			int swidth = s.GetColumnWidth();
			if (swidth <= wantColumnWidth)
				return s + WhiteSpace(wantColumnWidth - swidth);
			s = Excerpt(s, wantColumnWidth);
			return s + WhiteSpace(wantColumnWidth - s.GetColumnWidth());
		}

		#region GetColumnWidth
		static uint[,] combining = new uint[,] {
			{ 0x0300, 0x036F }, { 0x0483, 0x0486 }, { 0x0488, 0x0489 },
			{ 0x0591, 0x05BD }, { 0x05BF, 0x05BF }, { 0x05C1, 0x05C2 },
			{ 0x05C4, 0x05C5 }, { 0x05C7, 0x05C7 }, { 0x0600, 0x0603 },
			{ 0x0610, 0x0615 }, { 0x064B, 0x065E }, { 0x0670, 0x0670 },
			{ 0x06D6, 0x06E4 }, { 0x06E7, 0x06E8 }, { 0x06EA, 0x06ED },
			{ 0x070F, 0x070F }, { 0x0711, 0x0711 }, { 0x0730, 0x074A },
			{ 0x07A6, 0x07B0 }, { 0x07EB, 0x07F3 }, { 0x0901, 0x0902 },
			{ 0x093C, 0x093C }, { 0x0941, 0x0948 }, { 0x094D, 0x094D },
			{ 0x0951, 0x0954 }, { 0x0962, 0x0963 }, { 0x0981, 0x0981 },
			{ 0x09BC, 0x09BC }, { 0x09C1, 0x09C4 }, { 0x09CD, 0x09CD },
			{ 0x09E2, 0x09E3 }, { 0x0A01, 0x0A02 }, { 0x0A3C, 0x0A3C },
			{ 0x0A41, 0x0A42 }, { 0x0A47, 0x0A48 }, { 0x0A4B, 0x0A4D },
			{ 0x0A70, 0x0A71 }, { 0x0A81, 0x0A82 }, { 0x0ABC, 0x0ABC },
			{ 0x0AC1, 0x0AC5 }, { 0x0AC7, 0x0AC8 }, { 0x0ACD, 0x0ACD },
			{ 0x0AE2, 0x0AE3 }, { 0x0B01, 0x0B01 }, { 0x0B3C, 0x0B3C },
			{ 0x0B3F, 0x0B3F }, { 0x0B41, 0x0B43 }, { 0x0B4D, 0x0B4D },
			{ 0x0B56, 0x0B56 }, { 0x0B82, 0x0B82 }, { 0x0BC0, 0x0BC0 },
			{ 0x0BCD, 0x0BCD }, { 0x0C3E, 0x0C40 }, { 0x0C46, 0x0C48 },
			{ 0x0C4A, 0x0C4D }, { 0x0C55, 0x0C56 }, { 0x0CBC, 0x0CBC },
			{ 0x0CBF, 0x0CBF }, { 0x0CC6, 0x0CC6 }, { 0x0CCC, 0x0CCD },
			{ 0x0CE2, 0x0CE3 }, { 0x0D41, 0x0D43 }, { 0x0D4D, 0x0D4D },
			{ 0x0DCA, 0x0DCA }, { 0x0DD2, 0x0DD4 }, { 0x0DD6, 0x0DD6 },
			{ 0x0E31, 0x0E31 }, { 0x0E34, 0x0E3A }, { 0x0E47, 0x0E4E },
			{ 0x0EB1, 0x0EB1 }, { 0x0EB4, 0x0EB9 }, { 0x0EBB, 0x0EBC },
			{ 0x0EC8, 0x0ECD }, { 0x0F18, 0x0F19 }, { 0x0F35, 0x0F35 },
			{ 0x0F37, 0x0F37 }, { 0x0F39, 0x0F39 }, { 0x0F71, 0x0F7E },
			{ 0x0F80, 0x0F84 }, { 0x0F86, 0x0F87 }, { 0x0F90, 0x0F97 },
			{ 0x0F99, 0x0FBC }, { 0x0FC6, 0x0FC6 }, { 0x102D, 0x1030 },
			{ 0x1032, 0x1032 }, { 0x1036, 0x1037 }, { 0x1039, 0x1039 },
			{ 0x1058, 0x1059 }, { 0x1160, 0x11FF }, { 0x135F, 0x135F },
			{ 0x1712, 0x1714 }, { 0x1732, 0x1734 }, { 0x1752, 0x1753 },
			{ 0x1772, 0x1773 }, { 0x17B4, 0x17B5 }, { 0x17B7, 0x17BD },
			{ 0x17C6, 0x17C6 }, { 0x17C9, 0x17D3 }, { 0x17DD, 0x17DD },
			{ 0x180B, 0x180D }, { 0x18A9, 0x18A9 }, { 0x1920, 0x1922 },
			{ 0x1927, 0x1928 }, { 0x1932, 0x1932 }, { 0x1939, 0x193B },
			{ 0x1A17, 0x1A18 }, { 0x1B00, 0x1B03 }, { 0x1B34, 0x1B34 },
			{ 0x1B36, 0x1B3A }, { 0x1B3C, 0x1B3C }, { 0x1B42, 0x1B42 },
			{ 0x1B6B, 0x1B73 }, { 0x1DC0, 0x1DCA }, { 0x1DFE, 0x1DFF },
			{ 0x200B, 0x200F }, { 0x202A, 0x202E }, { 0x2060, 0x2063 },
			{ 0x206A, 0x206F }, { 0x20D0, 0x20EF }, { 0x2E9A, 0x2E9A },
			{ 0x2EF4, 0x2EFF }, { 0x2FD6, 0x2FEF }, { 0x2FFC, 0x2FFF },
			{ 0x31E4, 0x31EF }, { 0x321F, 0x321F }, { 0xA48D, 0xA48F },
			{ 0xA806, 0xA806 }, { 0xA80B, 0xA80B }, { 0xA825, 0xA826 },
			{ 0xFB1E, 0xFB1E }, { 0xFE00, 0xFE0F }, { 0xFE1A, 0xFE1F },
			{ 0xFE20, 0xFE23 }, { 0xFE53, 0xFE53 }, { 0xFE67, 0xFE67 },
			{ 0xFEFF, 0xFEFF }, { 0xFFF9, 0xFFFB },
		};

		static uint[,] combiningWideChars = new uint[,] {
			/* Hangul Jamo init. consonants - 0x1100, 0x11ff */
			/* Miscellaneous Technical - 0x2300, 0x23ff */
			/* Hangul Syllables - 0x11a8, 0x11c2 */
			/* CJK Compatibility Ideographs - f900, fad9 */
			/* Vertical forms - fe10, fe19 */
			/* CJK Compatibility Forms - fe30, fe4f */
			/* Fullwidth Forms - ff01, ffee */
			/* Alphabetic Presentation Forms - 0xFB00, 0xFb4f */
			/* Chess Symbols - 0x1FA00, 0x1FA0f */

			{ 0x1100, 0x115f }, { 0x231a, 0x231b }, { 0x2329, 0x232a },
			{ 0x23e9, 0x23ec }, { 0x23f0, 0x23f0 }, { 0x23f3, 0x23f3 },
			{ 0x25fd, 0x25fe }, { 0x2614, 0x2615 }, { 0x2648, 0x2653 },
			{ 0x267f, 0x267f }, { 0x2693, 0x2693 }, { 0x26a1, 0x26a1 },
			{ 0x26aa, 0x26ab }, { 0x26bd, 0x26be }, { 0x26c4, 0x26c5 },
			{ 0x26ce, 0x26ce }, { 0x26d4, 0x26d4 }, { 0x26ea, 0x26ea },
			{ 0x26f2, 0x26f3 }, { 0x26f5, 0x26f5 }, { 0x26fa, 0x26fa },
			{ 0x26fd, 0x26fd }, { 0x2705, 0x2705 }, { 0x270a, 0x270b },
			{ 0x2728, 0x2728 }, { 0x274c, 0x274c }, { 0x274e, 0x274e },
			{ 0x2753, 0x2755 }, { 0x2757, 0x2757 }, { 0x2795, 0x2797 },
			{ 0x27b0, 0x27b0 }, { 0x27bf, 0x27bf }, { 0x2b1b, 0x2b1c },
			{ 0x2b50, 0x2b50 }, { 0x2b55, 0x2b55 }, { 0x2e80, 0x303e },
			{ 0x3041, 0x3096 }, { 0x3099, 0x30ff }, { 0x3105, 0x312f },
			{ 0x3131, 0x318e }, { 0x3190, 0x3247 }, { 0x3250, 0x4dbf },
			{ 0x4e00, 0xa4c6 }, { 0xa960, 0xa97c }, { 0xac00 ,0xd7a3 },
			{ 0xf900, 0xfaff }, { 0xfe10, 0xfe1f }, { 0xfe30 ,0xfe6b },
			{ 0xff01, 0xff60 }, { 0xffe0, 0xffe6 }, { 0x10000, 0x10ffff }
		};

		static int bisearch(uint rune, uint[,] table, int max)
		{
			int min = 0;
			int mid;

			if (rune < table[0, 0] || rune > table[max, 1])
				return 0;
			while (max >= min)
			{
				mid = (min + max) / 2;
				if (rune > table[mid, 1])
					min = mid + 1;
				else if (rune < table[mid, 0])
					max = mid - 1;
				else
					return 1;
			}

			return 0;
		}

		/// <summary>
		/// Check if the rune is a non-spacing character.
		/// </summary>
		/// <param name="rune">The rune.</param>
		/// <returns>True if is a non-spacing character, false otherwise.</returns>
		static bool IsNonSpacingChar(uint rune)
		{
			return bisearch(rune, combining, combining.GetLength(0) - 1) != 0;
		}

		/// <summary>
		/// Check if the rune is a wide character.
		/// </summary>
		/// <param name="rune">The rune.</param>
		/// <returns>True if is a wide character, false otherwise.</returns>
		static bool IsWideChar(uint rune)
		{
			return bisearch(rune, combiningWideChars, combiningWideChars.GetLength(0) - 1) != 0;
		}

		/// <summary>
		/// Number of column positions of a wide-character code.   This is used to measure runes as displayed by text-based terminals.
		/// </summary>
		/// <returns>The width in columns, 0 if the argument is the null character, -1 if the value is not printable, otherwise the number of columns that the rune occupies.</returns>
		/// <param name="irune">The rune.</param>
		static int ColumnWidth(uint irune)
		{
			//uint irune = (uint)rune;
			if (irune < 0x20 || (irune >= 0x7f && irune < 0xa0))
				return -1;
			if (irune < 0x7f)
				return 1;
			/* binary search in table of non-spacing characters */
			if (bisearch(irune, combining, combining.GetLength(0) - 1) != 0)
				return 0;
			/* if we arrive here, ucs is not a combining or C0/C1 control character */
			return 1 +
				(bisearch(irune, combiningWideChars, combiningWideChars.GetLength(0) - 1) != 0 ? 1 : 0);
		}

		public static int GetColumnWidth(this string s)
		{
			if (s.All(char.IsAscii))
				return s.Length;

			int width = 0;
#if false

			foreach (var c in s.EnumerateRunes())
			{
				int w = ColumnWidth((uint)c.Value);
				width += w >= 0 ? w : 0;
			}
#else
			for (int i = 0; i < s.Length; i += char.IsSurrogatePair(s, i) ? 2 : 1)
			{
				uint codepoint = (uint)char.ConvertToUtf32(s, i);
				int w = ColumnWidth(codepoint);
				width += w;
			}
#endif
			return width;
		}
		#endregion
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
