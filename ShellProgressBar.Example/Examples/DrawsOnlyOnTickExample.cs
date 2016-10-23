using System.Threading;
using System.Threading.Tasks;
using ShellProgressBar.Example.Interfaces;

namespace ShellProgressBar.Example.Examples
{
  public class DrawsOnlyOnTickExample : IProgressBarExample
  {
    public Task Start(CancellationToken token)
    {
      var ticks = 5;
      var options = new ProgressBarOptions { DisplayTimeInRealTime = false };
      using (var pbar = new ProgressBar(ticks, "only update time on ticks", options))
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