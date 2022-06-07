using System.Threading.Tasks;

namespace ShellProgressBar.Example.Examples
{
	public class DontDisplayInRealTimeExample : ExampleBase
	{
		protected override Task StartAsync()
		{
			const int totalTicks = 5;
			var options = new ProgressBarOptions
			{
				DisplayTimeInRealTime = false
			};
			using (var pbar = new ProgressBar(totalTicks, "only draw progress on tick", options))
			{
				TickToCompletion(pbar, totalTicks, sleep: 1750);
			}

			return Task.CompletedTask;
		}
	}
}
