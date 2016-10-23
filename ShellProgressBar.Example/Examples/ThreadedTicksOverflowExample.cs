using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ShellProgressBar.Example.Interfaces;

namespace ShellProgressBar.Example.Examples
{
  public class ThreadedTicksOverflowExample : IProgressBarExample
  {
    public Task Start(CancellationToken token)
    {
      var ticks = 200;
      using (var pbar = new ProgressBar(ticks/10, "My operation that ticks to often using threads", ConsoleColor.Cyan))
      {
        var threads = Enumerable.Range(0, ticks).Select(i => new Thread(() => pbar.Tick("threaded Tick " + i))).ToList();
        foreach (var thread in threads)
        {
          thread.Start();
        }

        foreach (var thread in threads)
        {
          thread.Join();
        }
        return Task.FromResult(1);
      }
    }
  }
}