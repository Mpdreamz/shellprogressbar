using System;
using System.Threading;
using System.Threading.Tasks;

namespace ShellProgressBar.Example.TestCases
{
	public class TicksOverflowExample : IProgressBarExample
	{
		public Task Start(CancellationToken token)
		{
			var ticks = 10;
			using (var pbar = new ProgressBar(ticks, "My operation that ticks to often", ConsoleColor.Cyan))
			{
				for (var i = 0; i < ticks*10; i++)
				{
					pbar.Tick("too many steps " + i);
					Thread.Sleep(50);
				}
			}
			return Task.FromResult(1);
		}
	}
}