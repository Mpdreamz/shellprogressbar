using System;
using System.Collections.Concurrent;
using System.Threading;

namespace ShellProgressBar
{
	public class ChildProgressBar : ProgressBarBase, IProgressBar
	{
		private readonly Action _scheduleDraw;
		private readonly Action<string> _writeLine;
		private readonly Action<string> _writeError;
		private readonly Action<ProgressBarHeight> _growth;

		public DateTime StartDate { get; }  = DateTime.Now;

		protected override void DisplayProgress() => _scheduleDraw?.Invoke();

		internal ChildProgressBar(
			int maxTicks,
			string message,
			Action scheduleDraw,
			Action<string> writeLine,
			Action<string> writeError,
			ProgressBarOptions options = null,
			Action<ProgressBarHeight> growth = null
		)
			: base(maxTicks, message, options)
		{
			_scheduleDraw = scheduleDraw;
			_writeLine = writeLine;
			_writeError = writeError;
			_growth = growth;
			_growth?.Invoke(ProgressBarHeight.Increment);
		}

		protected override void Grow(ProgressBarHeight direction) => _growth?.Invoke(direction);

		private bool _calledDone;
		private readonly object _callOnce = new object();
		protected override void OnDone()
		{
			if (_calledDone) return;
			lock(_callOnce)
			{
				if (_calledDone) return;

				if (this.EndTime == null)
					this.EndTime = DateTime.Now;

				if (this.Options.CollapseWhenFinished)
					_growth?.Invoke(ProgressBarHeight.Decrement);

				_calledDone = true;
			}
		}

		public override void WriteLine(string message) => _writeLine(message);
		public override void WriteErrorLine(string message) => _writeError(message);

		public void Dispose()
		{
			foreach (var c in this.Children) c.Dispose();
			OnDone();
		}

		public IProgress<T> AsProgress<T>(Func<T, string> message = null, Func<T, double?> percentage = null)
		{
			return new Progress<T>(this, message, percentage);
		}
	}
}
