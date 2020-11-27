using System;

namespace ShellProgressBar
{
	internal class Progress<T> : IProgress<T>, IDisposable
	{
		private readonly WeakReference<IProgressBar> _progressBar;
		private readonly Func<T, string> _message;
		private readonly Func<T, double?> _percentage;

		public Progress(IProgressBar progressBar, Func<T, string> message, Func<T, double?> percentage)
		{
			_progressBar = new WeakReference<IProgressBar>(progressBar);
			_message = message;
			_percentage = percentage ?? (value => value as double? ?? value as float?);
		}

		public void Report(T value)
		{
			if (!_progressBar.TryGetTarget(out var progressBar)) return;

			var message = _message?.Invoke(value);
			var percentage = _percentage(value);
			if (percentage.HasValue)
				progressBar.Tick((int)(percentage * progressBar.MaxTicks), message);
			else
				progressBar.Tick(message);
		}

		public void Dispose()
		{
			if (_progressBar.TryGetTarget(out var progressBar))
			{
				progressBar.Dispose();
			}
		}
	}
}
