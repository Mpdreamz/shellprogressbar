using System;
using System.Threading.Tasks;

namespace ShellProgressBar.Example.Examples
{
	public class MessageBeforeAndAfterExample : ExampleBase
	{
		protected override Task StartAsync()
		{
			Console.WriteLine("This should not be overwritten");
			int totalTicks = Console.WindowHeight;
			var options = new ProgressBarOptions
			{
				ForegroundColor = ConsoleColor.Yellow,
				ForegroundColorDone = ConsoleColor.DarkGreen,
				BackgroundColor = ConsoleColor.DarkGray,
				BackgroundCharacter = '\u2593'
			};
			using (var pbar = new ProgressBar(totalTicks, "showing off styling", options))
			{
				TickToCompletion(pbar, totalTicks, sleep: 250, i =>
				{
					if (i % 5 == 0)
					{
						// Single line
						pbar.WriteErrorLine($"[{i}] This{Environment.NewLine}[{i}] is{Environment.NewLine}[{i}] over{Environment.NewLine}[{i}] 4 lines");
						return;
					}
					if (i % 4 == 0)
					{
						// Single line
						pbar.WriteErrorLine($"[{i}] This has{Environment.NewLine}[{i}] 2 lines.");
						return;
					}
					if (i % 3 == 0)
					{
						// Single line
						pbar.WriteErrorLine($"[{i}] This is a very long line {new string('.', Console.BufferWidth)} and should be split over 2 lines");
						return;
					}
					pbar.WriteErrorLine($"[{i}] This should appear above");
				});
			}

			Console.WriteLine("This should not be overwritten either afterwards");
			return Task.CompletedTask;
		}
	}
}
