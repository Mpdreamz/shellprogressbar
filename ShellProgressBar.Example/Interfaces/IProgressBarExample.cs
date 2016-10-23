using System.Threading;
using System.Threading.Tasks;

namespace ShellProgressBar.Example.Interfaces
{
  public interface IProgressBarExample
  {
    Task Start(CancellationToken token);
  }
}