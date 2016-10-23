using System;
using System.Threading;
using System.Threading.Tasks;
using ShellProgressBar.Example.Interfaces;

namespace ShellProgressBar.Example.Examples
{
  public class UpdatesMaxTicksExample : IProgressBarExample
  {
    public Task Start(CancellationToken token)
    {
      var ticks = 10;
      using (var pbar = new ProgressBar(ticks, "My operation that updates maxTicks", ConsoleColor.Cyan))
      {
        var sleep = 1000;
        for (var i = 0; i < ticks; i++)
        {
          pbar.Tick("Updating maximum ticks " + i);
          if (i == 5)
          {
            ticks = 120;
            pbar.UpdateMaxTicks(ticks);
            sleep = 50;
          }
          Thread.Sleep(sleep);
        }
      }
      return Task.FromResult(1);
    }
  }
}