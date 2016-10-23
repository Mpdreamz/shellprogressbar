using System;

namespace ShellProgressBar.Interfaces
{
  public interface IProgressBar : IDisposable
  {
    /// <summary>
    /// 
    /// </summary>
    /// <param name="maxTicks"></param>
    /// <param name="message">The starting message to display</param>
    /// <param name="options"></param>
    /// <returns></returns>
    ChildProgressBar Spawn(int maxTicks, string message, ProgressBarOptions options = null);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="message">The message to display</param>
    void Tick(string message = "");

    double Percentage { get; }
    int CurrentTick { get; }
    int MaxTicks { get; }

    ConsoleColor ForeGroundColor { get; }
  }
}