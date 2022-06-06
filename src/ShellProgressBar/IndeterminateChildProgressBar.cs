using System;
using System.Collections.Concurrent;
using System.Threading;

namespace ShellProgressBar
{
	public class IndeterminateChildProgressBar : ChildProgressBar
	{
		private const int MaxTicksForIndeterminate = 20;
		private Timer _timer;
		internal IndeterminateChildProgressBar(
			string message,
			Action scheduleDraw,
			Action<string> writeLine,
			Action<string> writeError,
			ProgressBarOptions options = null,
			Action<ProgressBarHeight> growth = null
		)
			: base(MaxTicksForIndeterminate, message, scheduleDraw, writeLine, writeError, options, growth)
		{
			if (options == null)
			{
				options = new ProgressBarOptions();
			}

			options.DisableBottomPercentage = true;
			_timer = new Timer((s) => OnTimerTick(), null, 500, 500);
		}

		private long _seenTicks = 0;

		protected void OnTimerTick()
		{
			Interlocked.Increment(ref _seenTicks);
			if (_seenTicks == MaxTicksForIndeterminate - 1)
			{
				this.Tick(0);
				Interlocked.Exchange(ref _seenTicks, 0);
			}
			else
			{
				this.Tick();
			}
			DisplayProgress();
		}

		public void Finished()
		{
			_timer.Change(Timeout.Infinite, Timeout.Infinite);
			_timer.Dispose();
			Tick(MaxTicksForIndeterminate);
		}

		public void Dispose()
		{
			if (_timer != null) _timer.Dispose();
			foreach (var c in this.Children) c.Dispose();
			OnDone();
		}
	}
}
