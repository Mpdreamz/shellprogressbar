using System;
using System.Threading;
using System.Threading.Tasks;

namespace ShellProgressBar.Example.Examples
{
	public class AlternateFinishedColorExample : ExampleBase
	{
		protected override async Task StartAsync()
		{
			var options = new ProgressBarOptions
			{
				ForegroundColor = ConsoleColor.Yellow,
				ForegroundColorDone = ConsoleColor.DarkGreen,
				ForegroundColorError = ConsoleColor.Red,
				BackgroundColor = ConsoleColor.DarkGray,
				BackgroundCharacter = '\u2593'
			};

			using var pbar = new ProgressBar(100, "100 ticks", options);
			await Task.Run(
				() =>
				{
					for (var i = 0; i < 10; i++)
					{
						Task.Delay(10).Wait();
						pbar.Tick($"Step {i}");
					}
					pbar.WriteErrorLine("The task ran into an issue!");
					// OR pbar.ObservedError = true;
				});
			pbar.Message= "Indicate the task is done, but the status is not Green.";
		}


	}
}
