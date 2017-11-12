using System.Threading;
using System.Threading.Tasks;

namespace ShellProgressBar.Example.TestCases
{
	public class DrawsOnlyOnTickExample : IProgressBarExample
	{
		public Task Start(CancellationToken token)
		{
			var ticks = 5;
			var updateOnTicksOnlyOptions = new ProgressBarOptions {DisplayTimeInRealTime = false};
			using (var pbar = new ProgressBar(ticks, "only update time on ticks", updateOnTicksOnlyOptions))
			{
				for (var i = 0; i < ticks; i++)
				{
					pbar.Tick("only update time on ticks, current: " + i);
					Thread.Sleep(1750);
				}
			}
			return Task.FromResult(1);
		}
	}
}
