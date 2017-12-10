using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;

namespace ShellProgressBar
{
	public class ProgressBar : ProgressBarBase, IProgressBar
	{
		private static readonly object Lock = new object();
		private static readonly bool IsWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

		private readonly ConsoleColor _originalColor;
		private readonly int _originalCursorTop;
		private readonly int _originalWindowTop;
		private bool _isDisposed;

		private Timer _timer;

		private int _visisbleDescendants = 0;

		public ProgressBar(int maxTicks, string message, ConsoleColor color)
			: this(maxTicks, message, new ProgressBarOptions {ForegroundColor = color})
		{
		}

		public ProgressBar(int maxTicks, string message, ProgressBarOptions options = null)
			: base(maxTicks, message, options)
		{
			_originalCursorTop = Console.CursorTop;
			_originalWindowTop = Console.WindowTop;
			_originalColor = Console.ForegroundColor;

			Console.CursorVisible = false;

			if (this.Options.EnableTaskBarProgress)
				TaskbarProgress.SetState(TaskbarProgress.TaskbarStates.Normal);

			if (this.Options.DisplayTimeInRealTime)
				_timer = new Timer((s) => DisplayProgress(), null, 500, 500);
			else //draw once
				_timer = new Timer((s) =>
				{
					_timer.Dispose();
					DisplayProgress();
				}, null, 0, 1000);
		}

		protected override void Grow(ProgressBarHeight direction)
		{
			switch (direction)
			{
				case ProgressBarHeight.Increment:
					Interlocked.Increment(ref _visisbleDescendants);
					break;
				case ProgressBarHeight.Decrement:
					Interlocked.Decrement(ref _visisbleDescendants);
					break;
			}
		}

		private struct Indentation
		{
			public Indentation(ConsoleColor color, bool lastChild)
			{
				this.ConsoleColor = color;
				this.LastChild = lastChild;
			}

			public string Glyph => !LastChild ? "├─" : "└─";

			public readonly ConsoleColor ConsoleColor;
			public readonly bool LastChild;
		}

		private static void ProgressBarBottomHalf(double percentage, DateTime startDate, DateTime? endDate, string message,
			Indentation[] indentation, bool progressBarOnBottom)
		{
			var depth = indentation.Length;
			var maxCharacterWidth = Console.WindowWidth - (depth * 2) + 2;
			var duration = ((endDate ?? DateTime.Now) - startDate);
			var durationString = $"{duration.Hours:00}:{duration.Minutes:00}:{duration.Seconds:00}";

			var column1Width = Console.WindowWidth - durationString.Length - (depth * 2) + 2;
			var column2Width = durationString.Length;

			if (progressBarOnBottom)
				DrawTopHalfPrefix(indentation, depth);
			else
				DrawBottomHalfPrefix(indentation, depth);

			var format = $"{{0, -{column1Width}}}{{1,{column2Width}}}";

			var truncatedMessage = StringExtensions.Excerpt($"{percentage:N2}%" + " " + message, column1Width);
			var formatted = string.Format(format, truncatedMessage, durationString);
			var m = formatted + new string(' ', Math.Max(0, maxCharacterWidth - formatted.Length));
			Console.Write(m);
		}

		private static void DrawBottomHalfPrefix(Indentation[] indentation, int depth)
		{
			for (var i = 1; i < depth; i++)
			{
				var ind = indentation[i];
				Console.ForegroundColor = indentation[i - 1].ConsoleColor;
				if (!ind.LastChild)
					Console.Write(i == (depth - 1) ? ind.Glyph : "│ ");
				else
					Console.Write(i == (depth - 1) ? ind.Glyph : "  ");
			}
			Console.ForegroundColor = indentation[depth - 1].ConsoleColor;
		}

		private static void ProgressBarTopHalf(
			double percentage,
			char progressCharacter,
			char? progressBackgroundCharacter,
			ConsoleColor? backgroundColor,
			Indentation[] indentation, bool progressBarOnTop)
		{
			var depth = indentation.Length;
			var width = Console.WindowWidth - (depth * 2) + 2;

			if (progressBarOnTop)
				DrawBottomHalfPrefix(indentation, depth);
			else
				DrawTopHalfPrefix(indentation, depth);

			var newWidth = (int) ((width * percentage) / 100d);
			var progBar = new string(progressCharacter, newWidth);
			Console.Write(progBar);
			if (backgroundColor.HasValue)
			{
				Console.ForegroundColor = backgroundColor.Value;
				Console.Write(new string(progressBackgroundCharacter ?? progressCharacter, width - newWidth));
			}
			else Console.Write(new string(' ', width - newWidth));
			Console.ForegroundColor = indentation[depth - 1].ConsoleColor;
		}

		private static void DrawTopHalfPrefix(Indentation[] indentation, int depth)
		{
			for (var i = 1; i < depth; i++)
			{
				var ind = indentation[i];
				Console.ForegroundColor = indentation[i - 1].ConsoleColor;
				if (ind.LastChild && i != (depth - 1))
					Console.Write("  ");
				else
					Console.Write("│ ");
			}
			Console.ForegroundColor = indentation[depth - 1].ConsoleColor;
		}

		private static string ResetString() => new string(' ', Console.WindowWidth);

		protected override void DisplayProgress()
		{
			if (_isDisposed) return;

			Console.CursorVisible = false;
			var indentation = new[] {new Indentation(this.ForeGroundColor, true)};
			var mainPercentage = this.Percentage;

			lock (Lock)
			{
				Console.ForegroundColor = this.ForeGroundColor;

				void TopHalf()
				{
					ProgressBarTopHalf(mainPercentage,
						this.Options.ProgressCharacter,
						this.Options.BackgroundCharacter,
						this.Options.BackgroundColor,
						indentation,
						this.Options.ProgressBarOnBottom
					);
				}

				if (this.Options.ProgressBarOnBottom)
				{
					Console.CursorLeft = 0;
					ProgressBarBottomHalf(mainPercentage, this._startDate, null, this.Message, indentation, this.Options.ProgressBarOnBottom);

					if (!IsWindows) Console.CursorTop = Console.CursorTop + 1;

					Console.CursorLeft = 0;
					TopHalf();
				}
				else
				{
					Console.CursorLeft = 0;
					TopHalf();
					if (!IsWindows) Console.CursorTop = Console.CursorTop + 1;

					Console.CursorLeft = 0;
					ProgressBarBottomHalf(mainPercentage, this._startDate, null, this.Message, indentation, this.Options.ProgressBarOnBottom);
				}

				if (this.Options.EnableTaskBarProgress)
					TaskbarProgress.SetValue(mainPercentage, 100);

				DrawChildren(this.Children, indentation);

				ResetToBottom();

				Console.CursorLeft = 0;
				Console.CursorTop = _originalCursorTop;
				Console.ForegroundColor = _originalColor;

				if (!(mainPercentage >= 100)) return;
				_timer?.Dispose();
				_timer = null;
			}
		}

		private static void ResetToBottom()
		{
			if (Console.CursorTop >= (Console.WindowHeight - 1)) return;
			do
			{
				Console.Write(ResetString());
			} while (Console.CursorTop < (Console.WindowHeight - 1));
		}

		private static void DrawChildren(IEnumerable<ChildProgressBar> children, Indentation[] indentation)
		{
			var view = children.Where(c => !c.Collapse).Select((c, i) => new {c, i}).ToList();
			if (!view.Any()) return;

			var lastChild = view.Max(t => t.i);
			foreach (var tuple in view)
			{
				//Dont bother drawing children that would fall off the screen
				if (Console.CursorTop >= (Console.WindowHeight - 2))
					return;

				var child = tuple.c;
				var currentIndentation = new Indentation(child.ForeGroundColor, tuple.i == lastChild);
				var childIndentation = NewIndentation(indentation, currentIndentation);

				var percentage = child.Percentage;
				Console.ForegroundColor = child.ForeGroundColor;

				void TopHalf()
				{
					ProgressBarTopHalf(percentage,
						child.Options.ProgressCharacter,
						child.Options.BackgroundCharacter,
						child.Options.BackgroundColor,
						childIndentation,
						child.Options.ProgressBarOnBottom
					);
				}
				if (!IsWindows) Console.CursorTop = Console.CursorTop + 1;
				if (child.Options.ProgressBarOnBottom)
				{
					Console.CursorLeft = 0;
					ProgressBarBottomHalf(percentage, child.StartDate, child.EndTime, child.Message, childIndentation, child.Options.ProgressBarOnBottom);
					if (!IsWindows) Console.CursorTop = Console.CursorTop + 1;

					Console.CursorLeft = 0;
					TopHalf();
				}
				else
				{
					Console.CursorLeft = 0;
					TopHalf();
					if (!IsWindows) Console.CursorTop = Console.CursorTop + 1;

					Console.CursorLeft = 0;
					ProgressBarBottomHalf(percentage, child.StartDate, child.EndTime, child.Message, childIndentation, child.Options.ProgressBarOnBottom);
				}

				DrawChildren(child.Children, childIndentation);
			}
		}

		private static Indentation[] NewIndentation(Indentation[] array, Indentation append)
		{
			var result = new Indentation[array.Length + 1];
			Array.Copy(array, result, array.Length);
			result[array.Length] = append;
			return result;
		}

		public void Dispose()
		{
			if (this.EndTime == null) this.EndTime = DateTime.Now;
			var openDescendantsPadding = (_visisbleDescendants * 2);

			if (this.Options.EnableTaskBarProgress)
				TaskbarProgress.SetState(TaskbarProgress.TaskbarStates.NoProgress);

			try
			{
				var moveDown = 0;
				var currentWindowTop = Console.WindowTop;
				if (currentWindowTop != _originalWindowTop)
				{
					var x = Math.Max(0, Math.Min(2, currentWindowTop - _originalWindowTop));
					moveDown = _originalCursorTop + x;
				}
				else moveDown = _originalCursorTop + 2;

				Console.CursorVisible = true;
				Console.CursorLeft = 0;
				Console.CursorTop = (openDescendantsPadding + moveDown);
			}
			// This is bad and I should feel bad, but i rather eat pbar exceptions in productions then causing false negatives
			catch
			{
			}
			Console.WriteLine();
			_isDisposed = true;
			_timer?.Dispose();
			_timer = null;
			foreach (var c in this.Children) c.Dispose();
		}
	}
}
