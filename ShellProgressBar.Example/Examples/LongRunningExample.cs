using System;
using System.Threading;
using System.Threading.Tasks;
using ShellProgressBar.Example.Interfaces;

namespace ShellProgressBar.Example.Examples
{
  public class LongRunningExample : IProgressBarExample
  {
    public Task Start(CancellationToken token)
    {
      var ticks = 100;
      var options = new ProgressBarOptions { ForeGroundColor = ConsoleColor.Gray, ProgressBarOnBottom = false };
      using (var pbar = new ProgressBar(ticks, "My Long Running Operation", options))
      {
        for (var i = 0; i < ticks; i++)
        {
          pbar.Tick("My long running operation step " + i);
          Thread.Sleep(50);
        }
      }

      return Task.FromResult(1);
    }
  }
}