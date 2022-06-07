using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace ShellProgressBar
{
	public class ProgressBar : ProgressBarBase, IProgressBar
	{
		private static readonly bool IsWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

		private readonly ConsoleColor _originalColor;
		private readonly Func<ConsoleOutLine, int> _writeMessageToConsole;
		private readonly int _originalWindowTop;
		private readonly int _originalWindowHeight;
		private readonly bool _startedRedirected;
		private int _originalCursorTop;
		private int _isDisposed;

		private Timer _timer;
		private int _visibleDescendants = 0;
		private readonly AutoResetEvent _displayProgressEvent;
		private readonly Task _displayProgress;

		public ProgressBar(int maxTicks, string message, ConsoleColor color)
			: this(maxTicks, message, new ProgressBarOptions {ForegroundColor = color})
		{
		}

		public ProgressBar(int maxTicks, string message, ProgressBarOptions options = null)
			: base(maxTicks, message, options)
		{

			_writeMessageToConsole = this.Options.WriteQueuedMessage ?? DefaultConsoleWrite;
			_startedRedirected = Console.IsOutputRedirected;

			try
			{
				_originalCursorTop = Console.CursorTop;
				_originalWindowTop = Console.WindowTop;
				_originalWindowHeight = Console.WindowHeight + _originalWindowTop;
				_originalColor = Console.ForegroundColor;
			}
			catch
			{
				_startedRedirected = true;
			}

			if (!_startedRedirected)
				Console.CursorVisible = false;

			if (this.Options.EnableTaskBarProgress)
				TaskbarProgress.SetState(TaskbarProgress.TaskbarStates.Normal);

			if (this.Options.DisplayTimeInRealTime)
				_timer = new Timer((s) => OnTimerTick(), null, 500, 500);
			else //draw once
				_timer = new Timer((s) =>
				{
					_timer.Dispose();
					DisplayProgress();
				}, null, 0, 1000);

			_displayProgressEvent = new AutoResetEvent(false);
			_displayProgress = Task.Run(() =>
			{
				while (_isDisposed == 0)
				{
					if (!_displayProgressEvent.WaitOne(TimeSpan.FromSeconds(10)))
						continue;
					if (_isDisposed > 0) return;
					try
					{
						UpdateProgress();
					}
					catch
					{
						//don't want to crash background thread
					}
				}
			});
		}

		protected virtual void OnTimerTick() => DisplayProgress();

		protected override void Grow(ProgressBarHeight direction)
		{
			switch (direction)
			{
				case ProgressBarHeight.Increment:
					Interlocked.Increment(ref _visibleDescendants);
					break;
				case ProgressBarHeight.Decrement:
					Interlocked.Decrement(ref _visibleDescendants);
					break;
			}
		}

		private void EnsureMainProgressBarVisible(int extraBars = 0)
		{
			var pbarHeight = this.Options.DenseProgressBar ? 1 : 2;
			var neededPadding = Math.Min(_originalWindowHeight - pbarHeight, (1 + extraBars) * pbarHeight);
			var difference = _originalWindowHeight - _originalCursorTop;
			var write = difference <= neededPadding ? Math.Max(0, Math.Max(neededPadding, difference)) : 0;

			var written = 0;
			for (; written < write; written++)
				Console.WriteLine();
			if (written == 0) return;

			Console.CursorTop = _originalWindowHeight - (written);
			_originalCursorTop = Console.CursorTop - 1;
		}

		private void GrowDrawingAreaBasedOnChildren() => EnsureMainProgressBarVisible(_visibleDescendants);

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

		private static void CondensedProgressBar(
			double percentage,
			string message,
			char progressCharacter,
			char? progressBackgroundCharacter,
			ConsoleColor? backgroundColor,
			Indentation[] indentation,
			bool progressBarOnTop)
		{
			var depth = indentation.Length;
			var messageWidth = 30;
			var maxCharacterWidth = Console.WindowWidth - (depth * 2) + 2;
			var truncatedMessage = StringExtensions.Excerpt(message, messageWidth - 2) + " ";
			var width = (Console.WindowWidth - (depth * 2) + 2) - truncatedMessage.Length;

			if (!string.IsNullOrWhiteSpace(ProgressBarOptions.ProgressMessageEncodingName))
			{
				width = width + message.Length - System.Text.Encoding.GetEncoding(ProgressBarOptions.ProgressMessageEncodingName).GetBytes(message).Length;
			}

			var newWidth = (int) ((width * percentage) / 100d);
			var progBar = new string(progressCharacter, newWidth);
			DrawBottomHalfPrefix(indentation, depth);
			Console.Write(truncatedMessage);
			Console.Write(progBar);
			if (backgroundColor.HasValue)
			{
				Console.ForegroundColor = backgroundColor.Value;
				Console.Write(new string(progressBackgroundCharacter ?? progressCharacter, width - newWidth));
			}
			else Console.Write(new string(' ', width - newWidth));

			Console.ForegroundColor = indentation[depth - 1].ConsoleColor;
		}


		private static void ProgressBarBottomHalf(double percentage, DateTime startDate, DateTime? endDate,
			string message, Indentation[] indentation, bool progressBarOnBottom, bool showEstimatedDuration,
			TimeSpan estimatedDuration, bool disableBottomPercentage, string percentageFormat)
		{
			var depth = indentation.Length;
			var maxCharacterWidth = Console.WindowWidth - (depth * 2) + 2;
			var duration = ((endDate ?? DateTime.Now) - startDate);
			var durationString = GetDurationString(duration);

			if (showEstimatedDuration)
				durationString +=
					$" / {GetDurationString(estimatedDuration)}";

			var column1Width = Console.WindowWidth - durationString.Length - (depth * 2) + 2;
			var column2Width = durationString.Length;

			if (!string.IsNullOrWhiteSpace(ProgressBarOptions.ProgressMessageEncodingName))
			{
				column1Width = column1Width + message.Length - System.Text.Encoding.GetEncoding(ProgressBarOptions.ProgressMessageEncodingName).GetBytes(message).Length;
			}

			if (progressBarOnBottom)
				DrawTopHalfPrefix(indentation, depth);
			else
				DrawBottomHalfPrefix(indentation, depth);

			var format = $"{{0, -{column1Width}}}{{1,{column2Width}}}";
			var percentageFormatedString = string.Format(percentageFormat, percentage);
			var truncatedMessage = StringExtensions.Excerpt(percentageFormatedString + message, column1Width);

			if (disableBottomPercentage)
			{
				truncatedMessage = StringExtensions.Excerpt(message, column1Width);
			}

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

		protected override void DisplayProgress() => _displayProgressEvent.Set();

		private readonly ConcurrentQueue<ConsoleOutLine> _stickyMessages = new ConcurrentQueue<ConsoleOutLine>();

		public override void WriteLine(string message)
		{
			_stickyMessages.Enqueue(new ConsoleOutLine(message));
			DisplayProgress();
		}
		public override void WriteErrorLine(string message)
		{
			this.ObservedError = true;
			_stickyMessages.Enqueue(new ConsoleOutLine(message, error: true));
			DisplayProgress();
		}

		private void UpdateProgress()
		{
			var mainPercentage = this.Percentage;

			if (this.Options.EnableTaskBarProgress)
				TaskbarProgress.SetValue(mainPercentage, 100);

			// write queued console messages, displayprogress is signaled straight after but
			// just in case make sure we never write more then 5 in a display progress tick
			for (var i = 0; i < 5 && _stickyMessages.TryDequeue(out var m); i++)
				WriteConsoleLine(m);

			if (_startedRedirected) return;

			Console.CursorVisible = false;
			Console.ForegroundColor = this.ForegroundColor;

			GrowDrawingAreaBasedOnChildren();
			var cursorTop = _originalCursorTop;
			var indentation = new[] {new Indentation(this.ForegroundColor, true)};

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

			if (this.Options.DenseProgressBar)
			{
				CondensedProgressBar(mainPercentage,
					this.Message,
					this.Options.ProgressCharacter,
					this.Options.BackgroundCharacter,
					this.Options.BackgroundColor,
					indentation,
					this.Options.ProgressBarOnBottom
				);

			}
			else if (this.Options.ProgressBarOnBottom)
			{
				ProgressBarBottomHalf(mainPercentage, this._startDate, null, this.Message, indentation,
					this.Options.ProgressBarOnBottom, Options.ShowEstimatedDuration, EstimatedDuration, this.Options.DisableBottomPercentage,
					Options.PercentageFormat);
				Console.SetCursorPosition(0, ++cursorTop);
				TopHalf();
			}
			else
			{
				TopHalf();
				Console.SetCursorPosition(0, ++cursorTop);
				ProgressBarBottomHalf(mainPercentage, this._startDate, null, this.Message, indentation,
					this.Options.ProgressBarOnBottom, Options.ShowEstimatedDuration, EstimatedDuration, this.Options.DisableBottomPercentage,
					Options.PercentageFormat);
			}

			DrawChildren(this.Children, indentation, ref cursorTop, Options.PercentageFormat);

			ResetToBottom(ref cursorTop);

			Console.SetCursorPosition(0, _originalCursorTop);
			Console.ForegroundColor = _originalColor;

			if (!(mainPercentage >= 100)) return;
			_timer?.Dispose();
			_timer = null;
		}

		private void WriteConsoleLine(ConsoleOutLine m)
		{
			var resetString = new string(' ', Console.WindowWidth);
			Console.Write(resetString);
			Console.Write("\r");
			var foreground = Console.ForegroundColor;
			var background = Console.BackgroundColor;
			var written = _writeMessageToConsole(m);
			Console.ForegroundColor = foreground;
			Console.BackgroundColor = background;
			_originalCursorTop += written;
		}

		private static int DefaultConsoleWrite(ConsoleOutLine line)
		{
			if (line.Error) Console.Error.WriteLine(line.Line);
			else Console.WriteLine(line.Line);
			return 1;
		}

		private void ResetToBottom(ref int cursorTop)
		{
			var resetString = new string(' ', Console.WindowWidth);
			var windowHeight = _originalWindowHeight;
			if (cursorTop >= (windowHeight - 1)) return;
			do
			{
				Console.Write(resetString);
			} while (++cursorTop < (windowHeight - 1));
		}

		private static void DrawChildren(IEnumerable<ChildProgressBar> children, Indentation[] indentation,
			ref int cursorTop, string percentageFormat)
		{
			var view = children.Where(c => !c.Collapse).Select((c, i) => new {c, i}).ToList();
			if (!view.Any()) return;

			var windowHeight = Console.WindowHeight;
			var lastChild = view.Max(t => t.i);
			foreach (var tuple in view)
			{
				//Dont bother drawing children that would fall off the screen
				if (cursorTop >= (windowHeight - 2))
					return;

				var child = tuple.c;
				var currentIndentation = new Indentation(child.ForegroundColor, tuple.i == lastChild);
				var childIndentation = NewIndentation(indentation, currentIndentation);

				var percentage = child.Percentage;
				Console.ForegroundColor = child.ForegroundColor;

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

				Console.SetCursorPosition(0, ++cursorTop);

				if (child.Options.DenseProgressBar)
				{
					CondensedProgressBar(percentage,
						child.Message,
						child.Options.ProgressCharacter,
						child.Options.BackgroundCharacter,
						child.Options.BackgroundColor,
						childIndentation,
						child.Options.ProgressBarOnBottom
					);
				}
				else if (child.Options.ProgressBarOnBottom)
				{
					ProgressBarBottomHalf(percentage, child.StartDate, child.EndTime, child.Message, childIndentation,
						child.Options.ProgressBarOnBottom, child.Options.ShowEstimatedDuration,
						child.EstimatedDuration, child.Options.DisableBottomPercentage,
						percentageFormat);
					Console.SetCursorPosition(0, ++cursorTop);
					TopHalf();
				}
				else
				{
					TopHalf();
					Console.SetCursorPosition(0, ++cursorTop);
					ProgressBarBottomHalf(percentage, child.StartDate, child.EndTime, child.Message, childIndentation,
						child.Options.ProgressBarOnBottom, child.Options.ShowEstimatedDuration,
						child.EstimatedDuration, child.Options.DisableBottomPercentage,
						percentageFormat);
				}

				DrawChildren(child.Children, childIndentation, ref cursorTop, percentageFormat);
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
			if (Interlocked.CompareExchange(ref _isDisposed, 1, 0) != 0)
				return;

			_timer?.Dispose();
			_timer = null;

			// make sure background task is stopped before we clean up
			_displayProgressEvent.Set();
			_displayProgress.Wait();

			// update one last time - needed because background task might have
			// been already in progress before Dispose was called and it might
			// have been running for a very long time due to poor performance
			// of System.Console
			UpdateProgress();

			//make sure we pop all pending messages
			while (_stickyMessages.TryDequeue(out var m))
				WriteConsoleLine(m);

			if (this.EndTime == null)
				this.EndTime = DateTime.Now;

			if (this.Options.EnableTaskBarProgress)
				TaskbarProgress.SetState(TaskbarProgress.TaskbarStates.NoProgress);

			try
			{
				foreach (var c in this.Children) c.Dispose();
			}
			catch { }

			try
			{
				var pbarHeight = this.Options.DenseProgressBar ? 1 : 2;
				var openDescendantsPadding = (_visibleDescendants * pbarHeight);
				var newCursorTop = Math.Min(_originalWindowHeight, _originalCursorTop + pbarHeight + openDescendantsPadding);
				Console.CursorVisible = true;
				Console.SetCursorPosition(0, newCursorTop);
			}
			//This is bad and I should feel bad, but i rather eat pbar exceptions in production then causing false negatives
			catch { }
		}

		public IProgress<T> AsProgress<T>(Func<T, string> message = null, Func<T, double?> percentage = null)
		{
			return new Progress<T>(this, message, percentage);
		}
	}
}
