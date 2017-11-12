using System;

namespace ShellProgressBar
{
	/// <summary>
	/// Control the behaviour of your progressbar
	/// </summary>
	public class ProgressBarOptions
	{
		public static readonly ProgressBarOptions Default = new ProgressBarOptions();

		/// <summary> The foreground color for text messages</summary>
		public ConsoleColor ForegroundColor { get; set; } = ConsoleColor.Green;
		
		/// <summary> The foreground color for text messages when the progressbar finishes</summary>
		public ConsoleColor? ForegroundColorDone { get; set; } 
		
		/// <summary> The background color for the progressbar iteself</summary>
		public ConsoleColor? BackgroundColor { get; set; }
		
		/// <summary> The character to use to draw the progressbar</summary>
		public char ProgressCharacter { get; set; } = '\u2588';
		
		/// <summary> 
		/// When true will redraw the progressbar using a timer, otherwise only update when 
		/// <see cref="ProgressBarBase.Tick"/> is called.
		/// Defaults to true
		///  </summary>
		public bool DisplayTimeInRealTime { get; set; } = true;
		
		/// <summary>
		/// Collapse the progressbar when done, very useful for child progressbars
		/// </summary>
		public bool CollapseWhenFinished { get; set; } = true;
			
		/// <summary>
		/// By default the text and time information is displayed at the bottom and the progress bar at the top.
		/// This setting swaps their position
		/// </summary>
		public bool ProgressBarOnBottom { get; set; }
	}
}