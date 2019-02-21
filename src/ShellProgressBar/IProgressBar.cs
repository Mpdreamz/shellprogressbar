using System;

namespace ShellProgressBar
{
	public interface IProgressBar : IDisposable
	{
		ChildProgressBar Spawn(int maxTicks, string message, ProgressBarOptions options = null);

		void Tick(string message = null);
		void Tick(int newTickCount, string message = null);

		int MaxTicks { get; set; }
		string Message { get; set; }

		double Percentage { get; }
		int CurrentTick { get; }

		ConsoleColor ForeGroundColor { get; }

		IProgress<T> AsProgress<T>(Func<T, string> message = null, Func<T, double?> percentage = null);
	}
}
