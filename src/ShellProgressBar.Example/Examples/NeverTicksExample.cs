using System.Threading;
using System.Threading.Tasks;

namespace ShellProgressBar.Example.Examples
{
	public class NeverTicksExample : IProgressBarExample
	{
		public Task Start(CancellationToken token)
		{
			var ticks = 10;
			using (var pbar = new ProgressBar(ticks, "A console progress bar that never ticks"))
			{
			}
			return Task.FromResult(1);
		}
	}
}