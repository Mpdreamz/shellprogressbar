using System;
using System.Threading;
using System.Threading.Tasks;
using ShellProgressBar.Example.Interfaces;

namespace ShellProgressBar.Example.Examples
{
  public class ZeroMaxTicksExample : IProgressBarExample
  {
    public Task Start(CancellationToken token)
    {
      var ticks = 0;
      using (var pbar = new ProgressBar(ticks, "my operation with zero ticks", ConsoleColor.Cyan))
      {
        for (var i = 0; i < ticks; i++)
        {
          pbar.Tick("step " + i);
          Thread.Sleep(50);
        }
      }
      return Task.FromResult(1);
    }
  }
}