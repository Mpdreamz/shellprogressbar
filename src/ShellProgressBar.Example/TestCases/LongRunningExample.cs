using System;
using System.Threading;
using System.Threading.Tasks;

namespace ShellProgressBar.Example.TestCases
{
	public class LongRunningExample : IProgressBarExample
	{
		public Task Start(CancellationToken token)
		{
			var ticks = 100;
			using (var pbar = new ProgressBar(ticks, "my long running operation", ConsoleColor.Green))
			{
				for (var i = 0; i < ticks; i++)
				{
					pbar.Tick("step " + i);
					Thread.Sleep(50);
				}
			}
			return Task.FromResult(1);
		}
	}
}