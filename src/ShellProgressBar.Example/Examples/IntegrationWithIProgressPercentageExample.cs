using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ShellProgressBar.Example.Examples
{
	public class IntegrationWithIProgressPercentageExample : IProgressBarExample
	{
		public Task Start(CancellationToken token)
		{
			using (var pbar = new ProgressBar(100, "A console progress that integrates with IProgress<float>"))
			{
				ProcessFiles(pbar.AsProgress<float>());
			}
			return Task.FromResult(1);
		}

		public static void ProcessFiles(IProgress<float> progress)
		{
			var files = Enumerable.Range(1, 10).Select(e => new FileInfo($"Data{e:D2}.csv")).ToList();
			var i = 0;
			foreach (var file in files)
			{
				DoWork(file);
				progress?.Report(++i / (float)files.Count);
			}
		}

		private static void DoWork(FileInfo file)
		{
			Thread.Sleep(200);
		}
	}
}