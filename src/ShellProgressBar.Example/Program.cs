using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ShellProgressBar.Example
{
	class Program
	{
		static void Main(string[] args)
		{
			var ticks = 10;
			using (var pbar = new ProgressBar(ticks, "A console progress bar that never ticks"))
			{
			}
			using (var pbar = new ProgressBar(ticks, "A console progress bar does not complete"))
			{
				pbar.Tick();
				pbar.Tick();
				pbar.Tick();
				pbar.Tick();
			}
			ticks = 100;
			using (var pbar = new ProgressBar(ticks, "my long running operation", ConsoleColor.Green))
			{
				for (var i = 0; i < ticks; i++)
				{
					pbar.Tick("step " + i);
					Thread.Sleep(50);
				}
			}
			ticks = 0;
			using (var pbar = new ProgressBar(ticks, "my operation with zero ticks", ConsoleColor.Cyan))
			{
				for (var i = 0; i < ticks; i++)
				{
					pbar.Tick("step " + i);
					Thread.Sleep(50);
				}
			}
			ticks = -100;
			using (var pbar = new ProgressBar(ticks, "my operation with negative ticks", ConsoleColor.Cyan))
			{
				for (var i = 0; i < ticks; i++)
				{
					pbar.Tick("step " + i);
					Thread.Sleep(50);
				}
			}
			ticks = 10;
			using (var pbar = new ProgressBar(ticks, "My operation that ticks to often", ConsoleColor.Cyan))
			{
				for (var i = 0; i < ticks * 10; i++)
				{
					pbar.Tick("too many steps " + i);
					Thread.Sleep(50);
				}
			}

            AsyncTest().Wait();
			Console.ReadLine();
		}

        static async Task AsyncTest()
        {
            await ProgressBar.FromTasks(
                Enumerable.Range(0, 200).Select(async i =>
                {
                    await Task.Delay(i * 10);
                    return $"threaded tick {i}";
                }),
                "My operation that ticks to often using threads");
        }
	}
}
