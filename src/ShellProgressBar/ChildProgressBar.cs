using System;
using System.Collections.Concurrent;
using System.Threading;

namespace ShellProgressBar
{
	public class ChildProgressBar : ProgressBarBase, IProgressBar
	{
		private readonly Action _scheduleDraw;
		private readonly Action<ProgressBarHeight> _growth;

		public DateTime StartDate { get; }  = DateTime.Now;

		protected override void DisplayProgress() => _scheduleDraw?.Invoke();

		internal ChildProgressBar(int maxTicks, string message, Action scheduleDraw, ProgressBarOptions options = null, Action<ProgressBarHeight> growth = null)
			: base(maxTicks, message, options)
		{
			_scheduleDraw = scheduleDraw;
			_growth = growth;
			_growth?.Invoke(ProgressBarHeight.Increment);
		}

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

				if (this.Collapse)
					_growth?.Invoke(ProgressBarHeight.Decrement);

				_calledDone = true;
			}
		}

		public void Dispose()
		{
			OnDone();
			foreach (var c in this.Children) c.Dispose();
		}
	}
}
