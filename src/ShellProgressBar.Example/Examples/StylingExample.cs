using System;
using System.Threading.Tasks;

namespace ShellProgressBar.Example.Examples
{
    public class StylingExample : ExampleBase
    {
		protected override Task StartAsync()
        {
            const int totalTicks = 10;
            var options = new ProgressBarOptions
            {
                ForegroundColor = ConsoleColor.Yellow,
	            ForegroundColorDone = ConsoleColor.DarkGreen,
	            BackgroundColor = ConsoleColor.DarkGray,
	            BackgroundCharacter = '\u2593'
            };
            using var pbar = new ProgressBar(totalTicks, "showing off styling", options);
            TickToCompletion(pbar, totalTicks, sleep: 500, i =>
            {
	            if (i > 5) pbar.ForegroundColor = ConsoleColor.Red;
            });

            return Task.CompletedTask;
        }
    }
}
