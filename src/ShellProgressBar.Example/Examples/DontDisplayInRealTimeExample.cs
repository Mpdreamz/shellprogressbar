namespace ShellProgressBar.Example.Examples
{
    public class DontDisplayInRealTimeExample : ExampleBase
    {
        protected override void Start()
        {
            const int totalTicks = 5;
            var options = new ProgressBarOptions
            {
                DisplayTimeInRealTime = false
            };
            using (var pbar = new ProgressBar(totalTicks, "only update time on ticks", options))
            {
                TickToCompletion(pbar, totalTicks);
            }
        }
    }
}