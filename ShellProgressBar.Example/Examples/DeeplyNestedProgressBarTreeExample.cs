using System;
using System.Threading;
using System.Threading.Tasks;
using ShellProgressBar.Example.Interfaces;

namespace ShellProgressBar.Example.Examples
{
    public class DeeplyNestedProgressBarTreeExample : IProgressBarExample
  {
    public Task Start(CancellationToken token)
    {
      var random = new Random();

      var numberOfSteps = 7;

      var options = new ProgressBarOptions
      {
        BackgroundColor = ConsoleColor.Gray
      };

      using (var pbar = new ProgressBar(numberOfSteps, "overall progress", options))
      {
        var stepBarOptions = new ProgressBarOptions
        {
          ForeGroundColor = ConsoleColor.Cyan,
          ForeGroundColorDone = ConsoleColor.DarkGreen,
          ProgressCharacter = '-',
          BackgroundColor = ConsoleColor.DarkGray,
          CollapseWhenFinished = false
        };

        Parallel.For(0, numberOfSteps, (i) =>
        {
          var workBarOptions = new ProgressBarOptions
          {
            ForeGroundColor = ConsoleColor.Yellow,
            ProgressCharacter = '-',
            BackgroundColor = ConsoleColor.DarkGray
          };

          var childSteps = random.Next(1,5);

          using (var childProgress = pbar.Spawn(childSteps, $"step {i} progress", stepBarOptions))
            Parallel.For(0, childSteps, (ci) =>
            {
              var childTicks = random.Next(50, 250);
              using(var innerChildProgress = childProgress.Spawn(childTicks, $"step {i}::{ci} progress", workBarOptions))
              {
                for (var r = 0; r < childTicks; r++)
                {
                  innerChildProgress.Tick();
                  Program.BusyWait(50);
                }
              }
              childProgress.Tick();
            });
          pbar.Tick();

        });
      }
      return Task.FromResult(1);
    }
  }
}