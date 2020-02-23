using System;
using System.Threading;

namespace ShellProgressBar.Example.Examples
{
	public class PersistMessageExample : ExampleBase
	{
		protected override void Start()
		{
			var options = new ProgressBarOptions
			{
				ForegroundColor = ConsoleColor.Yellow,
				ForegroundColorDone = ConsoleColor.DarkGreen,
				BackgroundColor = ConsoleColor.DarkGray,
				BackgroundCharacter = '\u2593',
				WriteQueuedMessage = o =>
				{
					var writer = o.Error ? Console.Error : Console.Out;
					if (o.Line.StartsWith("Report 500"))
					{
						Console.ForegroundColor = ConsoleColor.DarkRed;
						writer.WriteLine("Add an extra message, because why not");

						Console.ForegroundColor = ConsoleColor.Blue;
                        writer.WriteLine(o.Line);
                        return 2; //signal to the progressbar we wrote two messages
					}
					Console.ForegroundColor = ConsoleColor.Blue;
					writer.WriteLine(o.Line);
					return 1;
				}
			};
			var wait = TimeSpan.FromSeconds(5);
			using (var pbar = new FixedDurationBar(wait, "", options))
			{
				var t = new Thread(()=> LongRunningTask(pbar));
				t.Start();

				if (!pbar.CompletedHandle.WaitOne(wait))
					Console.Error.WriteLine($"{nameof(FixedDurationBar)} did not signal {nameof(FixedDurationBar.CompletedHandle)} after {wait}");

			}
		}

		private static void LongRunningTask(FixedDurationBar bar)
		{
			for (var i = 0; i < 1_000_000; i++)
			{
				bar.Message = $"{i} events";
				if (bar.IsCompleted) break;
				if (i % 500 == 0) bar.WriteLine($"Report {i} to console above the progressbar");
				Thread.Sleep(1);
			}
		}
	}
}
