using System;
using System.Collections.Concurrent;
using System.Threading;

namespace ShellProgressBar
{
  public abstract class ProgressBarBase
  {
    protected readonly DateTime _startDate = DateTime.Now;
    private int _maxTics;
    private int _currentTick;
    private string _message;

    protected ProgressBarBase(int maxTicks, string message, ProgressBarOptions options)
    {
      this._maxTics = Math.Max(0, maxTicks);
      this._message = message;
      this.Options = options ?? ProgressBarOptions.Default;
    }

    internal ProgressBarOptions Options { get; }
    internal ConcurrentBag<ChildProgressBar> Children { get; } = new ConcurrentBag<ChildProgressBar>();

    public DateTime? EndTime { get; protected set; }

    public ConsoleColor ForeGroundColor => 
      EndTime.HasValue ? this.Options.ForeGroundColorDone ?? this.Options.ForeGroundColor : this.Options.ForeGroundColor;

    public int CurrentTick => _currentTick;
    public int MaxTicks => _maxTics;
    public string Message => _message;

    public double Percentage
    {
      get
      {
        var percentage = Math.Max(0, Math.Min(100, (100.0 / this._maxTics) * this._currentTick));
        // Gracefully handle if the percentage is NaN due to division by 0
        
        if (double.IsNaN(percentage) || percentage < 0)
        {
          percentage = 100;
        }

        return percentage;
      }
    }

    public bool Collapse => this.EndTime.HasValue && this.Options.CollapseWhenFinished;

    protected abstract void DisplayProgress();

    public ChildProgressBar Spawn(int maxTicks, string message, ProgressBarOptions options = null)
    {
      var pbar = new ChildProgressBar(maxTicks, message, DisplayProgress, options, this.Grow);
      this.Children.Add(pbar);
      DisplayProgress();
      return pbar;
    }

    protected virtual void Grow(ProgressBarHeight direction) { }

    protected virtual void OnDone() { }

    public void Tick(string message = null)
    {
      Interlocked.Increment(ref _currentTick);
      
      if (string.IsNullOrEmpty(message) == false)
      {
        Interlocked.Exchange(ref _message, message);
      }

      if (_currentTick >= _maxTics)
      {
        this.EndTime = DateTime.Now;
        this.OnDone();
      }

      DisplayProgress();
    }

    public void UpdateMaxTicks(int maxTicks)
    {
      Interlocked.Exchange(ref _maxTics, maxTicks);
    }

    public void UpdateMessage(string message)
    {
      Interlocked.Exchange(ref _message, message);
      DisplayProgress();
    }
  }
}