using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ShellProgressBar.Example.Examples;

namespace ShellProgressBar.Example
{
	class Program
	{
		private static readonly IList<IProgressBarExample> ExampleProgressBars = new List<IProgressBarExample>
		{
			new DeeplyNestedProgressBarTreeExample(),
			new NestedProgressBarPerStepProgress(),
			new DrawsOnlyOnTickExample(),
			new ThreadedTicksOverflowExample(),
			new TicksOverflowExample(),
			new NegativeMaxTicksExample(),
			new ZeroMaxTicksExample(),
			new LongRunningExample(),
			new NeverCompletesExample(),
			new UpdatesMaxTicksExample(),
			new NeverTicksExample(),
		};

		static void Main(string[] args)
		{
			Console.WindowWidth = Console.LargestWindowWidth / 2;
			Console.WindowHeight = Console.LargestWindowHeight / 3;

			var cts = new CancellationTokenSource();

			Console.CancelKeyPress += (s, e) =>
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

		    while (sw.ElapsedMilliseconds < milliseconds)
		        Thread.SpinWait(1000);
		}

	}
}
