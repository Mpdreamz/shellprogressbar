using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ShellProgressBar.Example.Examples {
	public class AutomaticEstimatedDurationExample  : ExampleBase {
		protected override Task StartAsync()
		{
			const int totalTicks = 10;
			var options = new ProgressBarOptions
			{
				ProgressCharacter = '─',
				AutomaticEstimatedDuration = true,
				ShowEstimatedDuration = true
			};
			using (var pbar = new ProgressBar(totalTicks, "It can attempt to automatically calculate the duration as well", options))
			{
				var rand = new Random();

				var initialMessage = pbar.Message;
				for (var i = 0; i < totalTicks; i++)
				{
					pbar.Message = $"Start {i + 1} of {totalTicks}: {initialMessage}";
					Thread.Sleep(rand.Next(400,1200));

					pbar.Tick( $"End {i + 1} of {totalTicks}: {initialMessage}");
				}
			}

			return Task.CompletedTask;
		}
	}
}
