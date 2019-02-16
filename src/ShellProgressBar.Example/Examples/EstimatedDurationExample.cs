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
				ShowEstimatedDuration = true
			};
			using (var pbar = new ProgressBar(totalTicks, "you can set the estimated duration too", options))
			{
				pbar.EstimatedDuration = TimeSpan.FromMilliseconds(totalTicks * 500);

				var initialMessage = pbar.Message;
				for (var i = 0; i < totalTicks; i++)
				{
					pbar.Message = $"Start {i + 1} of {totalTicks}: {initialMessage}";
					Thread.Sleep(500);

					// Simulate changing estimated durations while progress increases
					var estimatedDuration =
						TimeSpan.FromMilliseconds(500 * totalTicks) + TimeSpan.FromMilliseconds(300 * i);
					pbar.Tick(i, estimatedDuration, $"End {i + 1} of {totalTicks}: {initialMessage}");
				}
			}
		}
	}
}
