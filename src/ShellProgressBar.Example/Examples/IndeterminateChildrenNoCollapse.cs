using System;
using System.Threading;

namespace ShellProgressBar.Example.Examples
{
	public class IndeterminateChildrenNoCollapseExample : ExampleBase
	{
		protected override void Start()
		{
			const int totalChildren = 10;
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
						Thread.Sleep(1000 * random.Next(5, 15));
						child.Finished();
					}
					pbar.Tick();
				}
			}
		}
	}
}
