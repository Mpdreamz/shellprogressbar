using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;

using Microsoft.CodeAnalysis;

namespace ShellProgressBar.UnicodeSourceGenerator
{
	[Generator]
	public class UnicodeUtilsSourceGenerator : ISourceGenerator
	{

		public void Execute(GeneratorExecutionContext context)
		{
			HttpClient client = new HttpClient();
			string res = client.GetStringAsync("https://www.unicode.org/Public/UCD/latest/ucd/EastAsianWidth.txt").Result;
			var group = res.Split('\n')
				.Select(i => i.Split('#')[0].Trim())
				.Where(i => !string.IsNullOrEmpty(i))
				.Select(i =>
				{
					var match = new Regex(@"^([0-9a-fA-F]{4,6})(\.\.([0-9a-fA-F]{4,6}))?\s+;\s+([AFHNW]a?)").Match(i);
					uint start = uint.Parse(match.Groups[1].Value, NumberStyles.HexNumber);
					uint? end = null;
					if (match.Groups[3].Success)
						end = uint.Parse(match.Groups[3].Value, NumberStyles.HexNumber);

					var type = AsUnicodeCharacterWidthType(match.Groups[4].Value);
					return new { start, end, type };
				})
				.ToList()
				.GroupBy(i => IsFullWidth(i.type));

			StringBuilder code = new StringBuilder();
			code.AppendLine("namespace System");
			code.AppendLine("{");
			code.AppendLine("internal static class UnicodeUtils");
			code.AppendLine("{");

			//code.AppendLine("private static bool IsFullWidthCharacter(int unicode) => unicode switch");
			//code.AppendLine("{");
			//code.AppendLine(string.Join("\r\n", group.Select(i => $"    {string.Join(" or ", i.Select(j => $"{(j.end.HasValue ? $"(>= {j.start} and <= {j.end})" : $"{j.start}")}"))} => {i.Key.ToString().ToLowerInvariant()},")));
			//code.AppendLine("    _ => false,");
			//code.AppendLine("};");

			code.AppendLine("private static bool IsFullWidthCharacter(int unicode)");
			code.AppendLine("{");
			code.AppendLine(string.Join("\r\n", group.Select(i => $"    if ({string.Join(" || ", i.Select(j => $"{(j.end.HasValue ? $"({j.start} <= unicode && unicode <= {j.end})" : $"(unicode == {j.start})")}"))}) return {i.Key.ToString().ToLowerInvariant()};"))); code.AppendLine("    return false;");
			code.AppendLine("}");
			code.AppendLine();
			code.AppendLine("public static int GetWidth(string str)");
			code.AppendLine("{");
			code.AppendLine("    int result = 0;");
			code.AppendLine("    for (int i = 0; i < str.Length; i++)");
			code.AppendLine("    {");
			code.AppendLine("        int unicode = 0;");
			code.AppendLine("        if (char.IsSurrogatePair(str, i))");
			code.AppendLine("        {");
			code.AppendLine("            unicode |= str[i] & 0x03FF;");
			code.AppendLine("            unicode <<= 10;");
			code.AppendLine("            i++;");
			code.AppendLine("            unicode |= str[i] & 0x03FF;");
			code.AppendLine("            unicode += 0x10000;");
			code.AppendLine("        }");
			code.AppendLine("        else");
			code.AppendLine("        {");
			code.AppendLine("            unicode = str[i];");
			code.AppendLine("        }");
			code.AppendLine("        result += IsFullWidthCharacter(unicode) ? 2 : 1;");
			code.AppendLine("    }");
			code.AppendLine("    return result;");
			code.AppendLine("}");

			code.AppendLine("}");
			code.AppendLine("}");

			context.AddSource("UnicodeUtils.g.cs", code.ToString());
		}

		internal static UnicodeCharacterWidthType AsUnicodeCharacterWidthType(string value)
		{
			//return value switch
			//{
			//	"A" => UnicodeCharacterWidthType.Ambiguous,
			//	"F" => UnicodeCharacterWidthType.Fullwidth,
			//	"H" => UnicodeCharacterWidthType.Halfwidth,
			//	"N" => UnicodeCharacterWidthType.Neutral,
			//	"Na" => UnicodeCharacterWidthType.Narrow,
			//	"W" => UnicodeCharacterWidthType.Wide,
			//	_ => throw new FormatException(),
			//};
			switch (value)
			{
				case "A":
					return UnicodeCharacterWidthType.Ambiguous;
				case "F":
					return UnicodeCharacterWidthType.Fullwidth;
				case "H":
					return UnicodeCharacterWidthType.Halfwidth;
				case "N":
					return UnicodeCharacterWidthType.Neutral;
				case "Na":
					return UnicodeCharacterWidthType.Narrow;
				case "W":
					return UnicodeCharacterWidthType.Wide;
				default:
					throw new FormatException();
			}
		}

		internal static bool IsFullWidth(UnicodeCharacterWidthType value)
		{
			//return value switch
			//{
			//	UnicodeCharacterWidthType.Ambiguous => false,
			//	UnicodeCharacterWidthType.Fullwidth => true,
			//	UnicodeCharacterWidthType.Halfwidth => false,
			//	UnicodeCharacterWidthType.Neutral => false,
			//	UnicodeCharacterWidthType.Narrow => false,
			//	UnicodeCharacterWidthType.Wide => true,
			//	_ => throw new InvalidCastException(),
			//};
			switch (value)
			{
				case UnicodeCharacterWidthType.Ambiguous:
				case UnicodeCharacterWidthType.Halfwidth:
				case UnicodeCharacterWidthType.Neutral:
				case UnicodeCharacterWidthType.Narrow:
					return false;
				case UnicodeCharacterWidthType.Fullwidth:
				case UnicodeCharacterWidthType.Wide:
					return true;
				default:
					throw new InvalidCastException();
			}
		}


		public void Initialize(GeneratorInitializationContext context) { }
	}

	internal enum UnicodeCharacterWidthType : byte
	{
		Ambiguous,
		Fullwidth,
		Halfwidth,
		Neutral,
		Narrow,
		Wide,
	}

}
