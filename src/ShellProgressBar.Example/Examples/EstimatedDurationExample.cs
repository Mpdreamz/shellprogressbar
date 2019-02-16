using System;
using System.Threading;

namespace ShellProgressBar.Example.Examples
{
	public class EstimatedDurationExample : ExampleBase
	{
		protected override void Start()
		{
			const int totalTicks = 10;
			var options = new ProgressBarOptions
			{
				ProgressCharacter = '─',
				ProgressBarOnBottom = true,
				ShowEstimatedDuration = true
			};
			using (var pbar = new ProgressBar(totalTicks, "progress bar is on the bottom now", options))
			{
				pbar.EstimatedDuration = TimeSpan.FromMilliseconds(totalTicks * 500);

				var initialMessage = pbar.Message;
				for (var i = 0; i < totalTicks; i++)
				{
					pbar.Message = $"Start {i + 1} of {totalTicks}: {initialMessage}";
					Thread.Sleep(500);
					pbar.Tick(i, TimeSpan.FromMilliseconds(500 * totalTicks) + TimeSpan.FromMilliseconds(500 * i), $"End {i + 1} of {totalTicks}: {initialMessage}");
				}
			}
		}
	}
}
