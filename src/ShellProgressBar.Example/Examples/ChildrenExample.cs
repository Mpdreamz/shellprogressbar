using System;
using System.Threading.Tasks;

namespace ShellProgressBar.Example.Examples
{
	public class ChildrenExample : ExampleBase
	{
		protected override Task StartAsync()
		{
			const int totalTicks = 10;
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
				ProgressCharacter = '─'
			};
			using var pbar = new ProgressBar(totalTicks, "main progressbar", options);
			TickToCompletion(pbar, totalTicks, sleep: 10, childAction: i =>
			{
				using var child = pbar.Spawn(totalTicks, "child actions", childOptions);
				TickToCompletion(child, totalTicks, sleep: 100);
			});
			return Task.CompletedTask;
		}
	}
}
