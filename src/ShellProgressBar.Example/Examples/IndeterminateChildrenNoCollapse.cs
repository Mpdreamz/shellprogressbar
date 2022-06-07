using System;
using System.Threading;
using System.Threading.Tasks;

namespace ShellProgressBar.Example.Examples
{
	public class IndeterminateChildrenNoCollapseExample : ExampleBase
	{
		protected override async Task StartAsync()
		{
			const int totalChildren = 2;
			Random random = new Random();
			var options = new ProgressBarOptions
			{
				ForegroundColor = ConsoleColor.Yellow,
				BackgroundColor = ConsoleColor.DarkGray,
				ProgressCharacter = '─'
			};
			var childOptions = new ProgressBarOptions
			{
				ForegroundColor = ConsoleColor.Green,
				BackgroundColor = ConsoleColor.DarkGray,
				ProgressCharacter = '─',
				CollapseWhenFinished = false
			};
			using (var pbar = new ProgressBar(totalChildren, "main progressbar", options))
			{
				for (int i = 1; i <= totalChildren; i++)
				{
					pbar.Message = $"Start {i} of {totalChildren}: main progressbar";
					using (var child = pbar.SpawnIndeterminate("child action " + i, childOptions))
					{
						await Task.Delay(1000 * random.Next(5, 15));
						child.Finished();
					}
					pbar.Tick();
				}
			}
		}
	}
}
