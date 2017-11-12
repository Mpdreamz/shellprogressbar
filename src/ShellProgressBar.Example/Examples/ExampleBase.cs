using System;
using System.Threading;
using System.Threading.Tasks;

namespace ShellProgressBar.Example.Examples
{
	public abstract class ExampleBase : IProgressBarExample
	{
		private bool RequestToQuit { get; set; }

		protected void TickToCompletion(IProgressBar pbar, int ticks, int sleep = 1750, Action childAction = null)
		{
			var initialMessage = pbar.Message;
			for (var i = 0; i < ticks && !RequestToQuit; i++)
			{
				pbar.Message = $"Start {i + 1} of {ticks}: {initialMessage}";
				childAction?.Invoke();
				Thread.Sleep(sleep);
				pbar.Tick($"End {i + 1} of {ticks}: {initialMessage}");
			}
		}

		public Task Start(CancellationToken token)
		{
			RequestToQuit = false;
			token.Register(() => RequestToQuit = true);

			this.Start();
			return Task.FromResult(1);
		}

		protected abstract void Start();
	}
}
