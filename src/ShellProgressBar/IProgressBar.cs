using System;

namespace ShellProgressBar
{
	public interface IProgressBar : IDisposable
	{
		ChildProgressBar Spawn(int maxTicks, string message, ProgressBarOptions options = null);

		void Tick(string message = "");

		int MaxTicks { get; set; }
		string Message { get; set; }

		double Percentage { get; }
		int CurrentTick { get; }

		ConsoleColor ForeGroundColor { get; }
	}
}
