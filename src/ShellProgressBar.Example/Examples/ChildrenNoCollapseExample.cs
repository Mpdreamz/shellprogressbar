using System;

namespace ShellProgressBar.Example.Examples
{
	public class ChildrenNoCollapseExample : ExampleBase
	{
		protected override void Start()
		{
			const int totalTicks = 10;
			var options = new ProgressBarOptions
			{
				ForegroundColor = ConsoleColor.Yellow,
				BackgroundColor = ConsoleColor.DarkYellow,
				ProgressCharacter = '─'
			};
			var childOptions = new ProgressBarOptions
			{
				ForegroundColor = ConsoleColor.Green,
				BackgroundColor = ConsoleColor.DarkGreen,
				ProgressCharacter = '─',
				CollapseWhenFinished = false
			};
			using (var pbar = new ProgressBar(totalTicks, "main progressbar", options))
			{
				TickToCompletion(pbar, totalTicks, sleep: 10, childAction: () =>
				{
					using (var child = pbar.Spawn(totalTicks, "child actions", childOptions))
					{
						TickToCompletion(child, totalTicks, sleep: 100);
					}
				});
			}
		}
	}
}
