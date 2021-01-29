using System;
using System.Threading;
using System.Threading.Tasks;

namespace ShellProgressBar.Example.Examples
{
	public class IndeterminateProgressExample : ExampleBase
	{
		protected override void Start()
		{
			var options = new ProgressBarOptions
			{
				ForegroundColor = ConsoleColor.Yellow,
				ForegroundColorDone = ConsoleColor.DarkGreen,
				BackgroundColor = ConsoleColor.DarkGray,
				BackgroundCharacter = '\u2593'
			};

			using (var pbar = new IndeterminateProgressBar("Indeterminate", options))
			{
				Task.Run(
					() =>
					{
						for (var i = 0; i < 1_000; i++)
						{
							pbar.Message= $"The progress is beating to its own drum (indeterminate) {i}";
							Task.Delay(10).Wait();
						}
					}).Wait();
				pbar.Finished();
				pbar.Message= "Finished! Moving on to the next in 5 seconds.";
			}

			Task.Delay(5000).Wait();
		}


	}
}
