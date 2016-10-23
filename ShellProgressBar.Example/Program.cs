using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using ShellProgressBar.Example.Examples;
using ShellProgressBar.Example.Interfaces;

namespace ShellProgressBar.Example
{
    public class Program
  {
    private static readonly IList<IProgressBarExample> ExampleProgressBars = new List<IProgressBarExample>
    {
      new ZeroMaxTicksExample(),
      new UpdatesMaxTicksExample(),
      new TicksOverflowExample(),
      new ThreadedTicksOverflowExample(),
      new NeverCompletesExample(),
      new NestedProgressBarPerStepProgress(),
      new DeeplyNestedProgressBarTreeExample(),
      new NegativeMaxTicksExample(),
      new DrawsOnlyOnTickExample(),
      new LongRunningExample(),
      new NeverTicksExample()
    };

    public static void Main(string[] args)
    {
      // Console.WindowWidth = Console.LargestWindowWidth / 2;
      // Console.WindowHeight = Console.LargestWindowHeight / 3;

      var cts = new CancellationTokenSource();

      Console.CancelKeyPress += (s, e ) =>
      {
        e.Cancel = true;
        cts.Cancel();
      };

      MainAsync(args, cts.Token).GetAwaiter().GetResult();
    }

    static async Task MainAsync(string[] args, CancellationToken token)
    {
      foreach (var example in ExampleProgressBars)
      {
        await example.Start(token);
        Console.Clear();
      }

      Console.ReadLine();
    }

    public static void BusyWait(int milliseconds)
    {
      var sw = Stopwatch.StartNew();

      while(sw.ElapsedMilliseconds < milliseconds)
      {
        Thread.Sleep(1000);
      }
    }
  }
}
