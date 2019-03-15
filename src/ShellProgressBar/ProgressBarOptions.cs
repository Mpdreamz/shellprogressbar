using System;
using System.Runtime.InteropServices;

namespace ShellProgressBar
{
	/// <summary>
	/// Control the behaviour of your progressbar
	/// </summary>
	public class ProgressBarOptions
	{
		private bool _enableTaskBarProgress;
		public static readonly ProgressBarOptions Default = new ProgressBarOptions();

		/// <summary> The foreground color of the progress bar, message and time</summary>
		public ConsoleColor ForegroundColor { get; set; } = ConsoleColor.Green;

		/// <summary> The foreground color the progressbar has reached a 100 percent</summary>
		public ConsoleColor? ForegroundColorDone { get; set; }

		/// <summary> The background color of the remainder of the progressbar</summary>
		public ConsoleColor? BackgroundColor { get; set; }

		/// <summary> The character to use to draw the progressbar</summary>
		public char ProgressCharacter { get; set; } = '\u2588';

		/// <summary>
		/// The character to use for the background of the progress defaults to <see cref="ProgressCharacter"/>
		/// </summary>
		public char? BackgroundCharacter { get; set; }

		/// <summary>
		/// When true will redraw the progressbar using a timer, otherwise only update when
		/// <see cref="ProgressBarBase.Tick"/> is called.
		/// Defaults to true
		///  </summary>
		public bool DisplayTimeInRealTime { get; set; } = true;

		/// <summary>
		/// Collapse the progressbar when done, very useful for child progressbars
		/// Defaults to true
		/// </summary>
		public bool CollapseWhenFinished { get; set; } = true;

		/// <summary>
		/// By default the text and time information is displayed at the bottom and the progress bar at the top.
		/// This setting swaps their position
		/// </summary>
		public bool ProgressBarOnBottom { get; set; }

		/// <summary>
		/// Use Windows' task bar to display progress.
		/// </summary>
		/// <remarks>
		/// This feature is available on the Windows platform.
		/// </remarks>
		public bool EnableTaskBarProgress
		{
			get => _enableTaskBarProgress;
			set
			{
				if (value && !RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
					throw new NotSupportedException("Task bar progress only works on Windows");

				_enableTaskBarProgress = value;
			}
		}

		/// <summary>
		/// By default child progress bars are not shown if they fall off the screen
		/// Use this setting to make sure the next layer is scrolled into view
		/// </summary>
		public bool ScrollChildrenIntoView {get; set; } = false;
	}
}
