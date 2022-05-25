using System;
using System.Threading;
using System.Threading.Tasks;

namespace ShellProgressBar.Example.Examples
{
	public class AlternateFinishedColorExample : ExampleBase
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
						for (var i = 0; i < 100; i++)
						{
							pbar.Message= $"The progress is beating to its own drum (indeterminate) {i}";
							Task.Delay(10).Wait();
						}
					}).Wait();
				pbar.Finished();
				pbar.ForegroundColorDone = ConsoleColor.Red;
				pbar.Message= "Indicate the task is done, but the status is not Green.";
			}

			Task.Delay(5000).Wait();
		}


	}
}
