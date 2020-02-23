using System;
using System.Linq;
using System.Net;
using System.Threading;

namespace ShellProgressBar.Example.Examples
{
	public class DownloadProgressExample : ExampleBase
	{
		protected override void Start()
		{
			var files = new string[]
			{
				"https://github.com/Mpdreamz/shellprogressbar/archive/4.3.0.zip",
				"https://github.com/Mpdreamz/shellprogressbar/archive/4.2.0.zip",
				"https://github.com/Mpdreamz/shellprogressbar/archive/4.1.0.zip",
				"https://github.com/Mpdreamz/shellprogressbar/archive/4.0.0.zip",
			};
			var childOptions = new ProgressBarOptions
			{
				ForegroundColor = ConsoleColor.Yellow,
				ProgressCharacter = '\u2593'
			};
			using var pbar = new ProgressBar(files.Length, "downloading");
			foreach (var (file, i) in files.Select((f, i) => (f, i)))
			{
				byte[] data = null;
				using var child = pbar.Spawn(100, "page: " + i, childOptions);
				try
				{
					using var client = new WebClient();
					client.DownloadProgressChanged += (o, args) => child.Tick(args.ProgressPercentage);
					client.DownloadDataCompleted += (o, args) => data = args.Result;
					client.DownloadDataAsync(new Uri(file));
					while (client.IsBusy)
					{
						Thread.Sleep(1);
					}

					pbar.Tick();
				}
				catch (WebException error)
				{
					pbar.WriteLine(error.Message);
				}
			}
		}
	}
}
