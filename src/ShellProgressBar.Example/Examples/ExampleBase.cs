using System;
using System.Threading;
using System.Threading.Tasks;

namespace ShellProgressBar.Example.Examples
{
	public abstract class ExampleBase : IProgressBarExample
	{
		private bool RequestToQuit { get; set; }

		protected void TickToCompletion(IProgressBar pbar, int ticks, int sleep = 1750, Action<int> childAction = null)
		{
			var initialMessage = pbar.Message;
			for (var i = 0; i < ticks && !RequestToQuit; i++)
			{
				pbar.Message = $"Start {i + 1} of {ticks} {Console.CursorTop}/{Console.WindowHeight}: {initialMessage}";
				childAction?.Invoke(i);
				Thread.Sleep(sleep);
				pbar.Tick($"End {i + 1} of {ticks} {Console.CursorTop}/{Console.WindowHeight}: {initialMessage}");
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
