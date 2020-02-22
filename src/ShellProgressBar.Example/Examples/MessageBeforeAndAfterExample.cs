using System;

namespace ShellProgressBar.Example.Examples
{
	public class MessageBeforeAndAfterExample : ExampleBase
	{
		protected override void Start()
		{
			Console.WriteLine("This should not be overwritten");
			const int totalTicks = 10;
			var options = new ProgressBarOptions
			{
				ForegroundColor = ConsoleColor.Yellow,
				ForegroundColorDone = ConsoleColor.DarkGreen,
				BackgroundColor = ConsoleColor.DarkGray,
				BackgroundCharacter = '\u2593'
			};
			using (var pbar = new ProgressBar(totalTicks, "showing off styling", options))
			{
				TickToCompletion(pbar, totalTicks, sleep: 500);
			}

			Console.WriteLine("This should not be overwritten either afterwards");
		}
	}
}
