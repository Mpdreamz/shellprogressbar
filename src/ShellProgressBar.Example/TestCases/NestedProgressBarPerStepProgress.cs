using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ShellProgressBar.Example.TestCases
{
	public class NestedProgressBarPerStepProgress : IProgressBarExample
	{
		public Task Start(CancellationToken token)
		{
			var outerTicks = 10;
			using (var pbar = new ProgressBar(outerTicks, "outer progress", ConsoleColor.Cyan))
			{
				for (var i = 0; i < outerTicks; i++)
				{
					InnerProgressBars(pbar);
					pbar.Tick();
				}
			}
			return Task.FromResult(1);
		}

		private static void InnerProgressBars(ProgressBar pbar)
		{
			var innerProgressBars = Enumerable.Range(0, new Random().Next(2, 6))
				.Select(s => pbar.Spawn(new Random().Next(2, 5), $"inner bar {s}"))
				.ToList();

			var maxTicks = innerProgressBars.Max(p => p.MaxTicks);

			for (var ii = 0; ii < maxTicks; ii++)
			{
				foreach (var p in innerProgressBars)
				{
					InnerInnerProgressBars(p);
					p.Tick();
				}


				Thread.Sleep(4);
			}
			foreach (var p in innerProgressBars) p.Dispose();
		}

		private static void InnerInnerProgressBars(ChildProgressBar pbar)
		{
			var progressBarOption = new ProgressBarOptions { ForegroundColor = ConsoleColor.Yellow };
			var innerProgressBars = Enumerable.Range(0, new Random().Next(1, 3))
				.Select(s => pbar.Spawn(new Random().Next(5, 10), $"inner bar {s}", progressBarOption))
				.ToList();
			if (!innerProgressBars.Any()) return;

			var maxTicks = innerProgressBars.Max(p => p.MaxTicks);

			for (var ii = 0; ii < maxTicks; ii++)
			{
				foreach (var p in innerProgressBars)
					p.Tick();
			}
			foreach (var p in innerProgressBars) p.Dispose();
		}
	}
}