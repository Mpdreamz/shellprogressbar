using System;

namespace ShellProgressBar.Example.Examples
{
    public class ProgressBarOnBottomExample : ExampleBase
    {
        protected override void Start()
        {
            const int totalTicks = 10;
            var options = new ProgressBarOptions
            {
				ProgressCharacter = '─',
	            ProgressBarOnBottom = true
            };
            using (var pbar = new ProgressBar(totalTicks, "color changes when done", options))
            {
                TickToCompletion(pbar, totalTicks, sleep: 500);
            }
        }
    }
}
