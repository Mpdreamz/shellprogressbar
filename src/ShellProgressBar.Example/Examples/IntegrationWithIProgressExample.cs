using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ShellProgressBar.Example.Examples
{
	public class IntegrationWithIProgressExample : IProgressBarExample
	{
		public Task Start(CancellationToken token)
		{
			var files = Enumerable.Range(1, 10).Select(e => new FileInfo($"Data{e:D2}.csv")).ToList();
			using (var pbar = new ProgressBar(files.Count, "A console progress that integrates with IProgress<T>"))
			{
				ProcessFiles(files, pbar.AsProgress<FileInfo>(e => $"Processed {e.Name}"));
			}
			return Task.FromResult(1);
		}

		public static void ProcessFiles(IEnumerable<FileInfo> files, IProgress<FileInfo> progress)
		{
			foreach (var file in files)
			{
				DoWork(file);
				progress?.Report(file);
			}
		}

		private static void DoWork(FileInfo file)
		{
			Thread.Sleep(200);
		}
	}
}