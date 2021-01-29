using System;
using Xunit;

namespace ShellProgressBar.Tests
{
	public class UnitTest1
	{
		[Fact]
		public void Test1()
		{
			using var pbar = new ProgressBar(1000, "task");
			pbar.Tick();
			pbar.WriteLine("Asdad");
		}
	}
}
