using System.Threading;
using System.Threading.Tasks;

namespace ShellProgressBar.Example.TestCases
{
	public class NeverCompletesExample : IProgressBarExample
	{
		public Task Start(CancellationToken token)
		{
			var ticks = 5;
			using (var pbar = new ProgressBar(ticks, "A console progress bar does not complete"))
			{
				pbar.Tick();
				pbar.Tick();
				pbar.Tick();
				pbar.Tick();
			}
			return Task.FromResult(1);
		}
	}
}