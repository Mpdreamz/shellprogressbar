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
//			new DrawsOnlyOnTickExample(),
//			new ThreadedTicksOverflowExample(),
//			new TicksOverflowExample(),
//			new NegativeMaxTicksExample(),
//			new ZeroMaxTicksExample(),
//			new LongRunningExample(),
//			new NeverCompletesExample(),
//			new UpdatesMaxTicksExample(),
//			new NeverTicksExample(),
		};

		static void Main(string[] args)
		{
			try
			{
				Console.WindowWidth = Console.LargestWindowWidth / 2;
				Console.WindowHeight = Console.LargestWindowHeight / 3;
			}
			catch
			{
				// ignored
			}

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
			Console.Write("Shown all examples!");

			Console.ReadLine();
		}
		public static void BusyWait(int milliseconds)
		{
			Thread.Sleep(milliseconds);
		}

	}
}
