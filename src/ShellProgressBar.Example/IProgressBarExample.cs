using System.Threading;
using System.Threading.Tasks;

namespace ShellProgressBar.Example
{
	public interface IProgressBarExample
	{
		Task Start(CancellationToken token);
	}
}