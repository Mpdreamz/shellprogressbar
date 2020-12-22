using System;
using System.Threading;

namespace ShellProgressBar
{
	public class IndeterminateProgressBar : ProgressBar
	{
		private const int MaxTicksForIndeterminate = 20;

		public IndeterminateProgressBar(string message, ConsoleColor color) : this(
			message,
			new ProgressBarOptions { ForegroundColor = color })
		{
		}

		public IndeterminateProgressBar(string message, ProgressBarOptions options = null) : base(
			MaxTicksForIndeterminate,
			message,
			options)
		{
			if (options == null)
			{
				options = new ProgressBarOptions();
			}

			options.DisableBottomPercentage = true;
			options.DisplayTimeInRealTime = true;

			if (!this.Options.DisplayTimeInRealTime)
				throw new ArgumentException(
					$"{nameof(ProgressBarOptions)}.{nameof(ProgressBarOptions.DisplayTimeInRealTime)} has to be true for {nameof(FixedDurationBar)}",
					nameof(options)
				);
		}

		private long _seenTicks = 0;

		protected override void OnTimerTick()
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

			base.OnTimerTick();
		}

		public void Finished()
		{
			Tick(MaxTicksForIndeterminate);
		}
	}
}
