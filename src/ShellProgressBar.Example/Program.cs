using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ShellProgressBar.Example.Examples;
using ShellProgressBar.Example.TestCases;

namespace ShellProgressBar.Example
{
	class Program
	{
		private static readonly IList<IProgressBarExample> TestCases = new List<IProgressBarExample>
		{
			new PersistMessageExample(),
			new FixedDurationExample(),
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
			new EstimatedDurationExample(),
			new IndeterminateProgressExample(),
			new IndeterminateChildrenNoCollapseExample(),
			new AlternateFinishedColorExample()
		};

		private static readonly IList<IProgressBarExample> Examples = new List<IProgressBarExample>
		{
			new DontDisplayInRealTimeExample(),
			new StylingExample(),
			new ProgressBarOnBottomExample(),
			new ChildrenExample(),
			new ChildrenNoCollapseExample(),
			new IntegrationWithIProgressExample(),
			new IntegrationWithIProgressPercentageExample(),
			new MessageBeforeAndAfterExample(),
			new DeeplyNestedProgressBarTreeExample(),
			new EstimatedDurationExample(),
			new DownloadProgressExample(),
			new AlternateFinishedColorExample(),
			new AutomaticEstimatedDurationExample()
		};

		public static async Task Main(string[] args)
		{
			var cts = new CancellationTokenSource();
			Console.CancelKeyPress += (s, e) =>
			{
				cts.Cancel();
			};

			await MainAsync(args, cts.Token);
		}

		private static async Task MainAsync(string[] args, CancellationToken token)
		{
			var command = args.Length > 0 ? args[0] : "test";
			switch (command)
			{
				case "test":
					await RunTestCases(token);
					return;
				case "scrolltest":
					await RunTestCases(token, Console.WindowHeight+5);
					return;
				case "example":
					var nth = args.Length > 1 ? int.Parse(args[1]) : 0;
					await RunExample(nth, token);
					return;
				default:
					await Console.Error.WriteLineAsync($"Unknown command:{command}");
					return;
			}
		}

		private static async Task RunExample(int nth, CancellationToken token)
		{
			if (nth < 0)
				nth = Examples.Count + nth;
			if (nth > Examples.Count - 1)
			{
				await Console.Error.WriteLineAsync($"There are only {Examples.Count} examples, {nth} is not valid");
			}

			var example = Examples[nth];

			await example.Start(token);
		}

		private static async Task RunTestCases(CancellationToken token, int writeNumOfRowBefore = 0)
		{
			var i = 0;
			foreach (var example in TestCases)
			{
				if (i > 0) Console.Clear(); //not necessary but for demo/recording purposes.

				for (int r = 0; r< writeNumOfRowBefore; r++)
					Console.WriteLine($"Writing output before test. Row {r+1}/{writeNumOfRowBefore}");

				await example.Start(token);
				i++;
			}
			Console.Write("Shown all examples!");
		}

		public static void BusyWait(int milliseconds)
		{
			Thread.Sleep(milliseconds);
		}
	}
}
