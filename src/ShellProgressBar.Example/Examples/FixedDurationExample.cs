using System;
using System.Threading;
using System.Threading.Tasks;

namespace ShellProgressBar.Example.Examples
{
	public class FixedDurationExample : ExampleBase
	{
		protected override Task StartAsync()
		{
			var options = new ProgressBarOptions
			{
				ForegroundColor = ConsoleColor.Yellow,
				ForegroundColorDone = ConsoleColor.DarkGreen,
				BackgroundColor = ConsoleColor.DarkGray,
				BackgroundCharacter = '\u2593'
			};
			var wait = TimeSpan.FromSeconds(5);
			using (var pbar = new FixedDurationBar(wait, "", options))
			{
				var t = new Thread(()=> LongRunningTask(pbar));
				t.Start();

				if (!pbar.CompletedHandle.WaitOne(wait))
					Console.Error.WriteLine($"{nameof(FixedDurationBar)} did not signal {nameof(FixedDurationBar.CompletedHandle)} after {wait}");

			}

			return Task.CompletedTask;
		}

		private static void LongRunningTask(FixedDurationBar bar)
		{
			for (var i = 0; i < 1_000_000; i++)
			{
				bar.Message = $"{i} events";
				if (bar.IsCompleted) break;
				Thread.Sleep(1);
			}
		}
	}
}
