using System;
using System.Collections.Concurrent;
using System.Text;
using System.Threading;

namespace ShellProgressBar
{
	public abstract class ProgressBarBase
	{
		static ProgressBarBase()
		{
			Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
		}
			
		protected readonly DateTime _startDate = DateTime.Now;
		private int _maxTicks;
		private int _currentTick;
		private string _message;

		protected ProgressBarBase(int maxTicks, string message, ProgressBarOptions options)
		{
			this._maxTicks = Math.Max(0, maxTicks);
			this._message = message;
			this.Options = options ?? ProgressBarOptions.Default;
		}

		internal ProgressBarOptions Options { get; }
		internal ConcurrentBag<ChildProgressBar> Children { get; } = new ConcurrentBag<ChildProgressBar>();

		public DateTime? EndTime { get; protected set; }

		public ConsoleColor ForeGroundColor => 
			EndTime.HasValue ? this.Options.ForegroundColorDone ?? this.Options.ForegroundColor : this.Options.ForegroundColor;

		public int CurrentTick => _currentTick;
		public int MaxTicks => _maxTicks;
		public string Message => _message;

		public double Percentage
		{
			get
			{
				var percentage = Math.Max(0, Math.Min(100, (100.0 / this._maxTicks) * this._currentTick));
				// Gracefully handle if the percentage is NaN due to division by 0
				if (double.IsNaN(percentage) || percentage < 0) percentage = 100;
				return percentage;
			}
		}

		public bool Collapse => this.EndTime.HasValue && this.Options.CollapseWhenFinished;

		protected abstract void DisplayProgress();

		public ChildProgressBar Spawn(int maxTicks, string message, ProgressBarOptions options = null)
		{
			var pbar = new ChildProgressBar(maxTicks, message, DisplayProgress, options, this.Grow);
			this.Children.Add(pbar);
			DisplayProgress();
			return pbar;
		}

		protected virtual void Grow(ProgressBarHeight direction)
		{

		}
		protected virtual void OnDone()
		{

		}

		public void Tick(string message = null)
		{
            FinishTick(message);
		}

        public void Tick(string message, int prec)
        {
            Interlocked.Exchange(ref _currentTick, prec);

            FinishTick(message);
        }

        public void UpdateMaxTicks(int maxTicks)
		{
			Interlocked.Exchange(ref _maxTicks, maxTicks);
		}

		public void UpdateMessage(string message)
		{
            Interlocked.Exchange(ref _message, message);

			DisplayProgress();
		}

        private void FinishTick(string message)
        {
            Interlocked.Increment(ref _currentTick);
            if (message != null)
                Interlocked.Exchange(ref _message, message);

            if (_currentTick >= _maxTicks)
            {
                this.EndTime = DateTime.Now;
                this.OnDone();
            }
            DisplayProgress();
        }
	}
}