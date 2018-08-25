using System;
using System.Collections.Generic;
using System.Text;
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
		};
		private static readonly IList<IProgressBarExample> Examples = new List<IProgressBarExample>
		{
			new DontDisplayInRealTimeExample(),
			new StylingExample(),
			new ProgressBarOnBottomExample(),
			new ChildrenExample(),
			new ChildrenNoCollapseExample(),
		};

		static void Main(string[] args)
		{
			Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
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
			var command = args.Length > 0 ? args[0] : "test";
			switch (command)
			{
				case "test":
					await RunTestCases(token);
					return;
				case "example":
					var nth = args.Length > 1 ? int.Parse(args[1]) : 0;
					await RunExample(token, nth);
					return;
				default:
					Console.Error.WriteLine($"Unknown command:{command}");
					return;
			}

		}

		private static async Task RunExample(CancellationToken token, int nth)
		{
			if (nth > Examples.Count -1 || nth < 0)
			{
				Console.Error.WriteLine($"There are only {Examples.Count} examples, {nth} is not valid");
			}
			var example = Examples[nth];
			var requestToQuit = false;
			token.Register(() => requestToQuit = true);


			while (!requestToQuit)
			{
				Console.WriteLine();
				await example.Start(token);
				var c = Console.Read();
				if (c == 'q') break;
				Console.Clear();
			}
		}

		private static async Task RunTestCases(CancellationToken token)
		{
			foreach (var example in TestCases)
			{
				Console.Clear(); //not necessary but for demo/recording purposes.
				await example.Start(token);
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
