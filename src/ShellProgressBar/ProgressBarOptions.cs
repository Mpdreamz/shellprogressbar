using System;

namespace ShellProgressBar
{
	public class ProgressBarOptions
	{
		public static readonly ProgressBarOptions Default = new ProgressBarOptions();

		public ConsoleColor ForeGroundColor { get; set; } = ConsoleColor.Green;
		public ConsoleColor? ForeGroundColorDone { get; set; }
		public ConsoleColor? BackgroundColor { get; set; }
		public char ProgressCharacter { get; set; } = '\u2588';
		public bool DisplayTimeInRealTime { get; set; } = true;
		public bool CollapseWhenFinished { get; set; } = true;
		public bool ProgressBarOnBottom { get; set; }
	}
}