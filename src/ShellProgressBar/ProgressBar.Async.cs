using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShellProgressBar
{
    public partial class ProgressBar
    {
        public static Task FromTasks(IEnumerable<Task> source, string message, ConsoleColor color = ConsoleColor.Green, char progressCharacter = '█')
        {
            List<Task> tasks = source.ToList();
            int maxTicks = tasks.Count;
            return FromTasks(tasks, maxTicks, message, color, progressCharacter);
        }

        public static Task FromTasks(IEnumerable<Task> source, int maxTicks, string message, ConsoleColor color = ConsoleColor.Green, char progressCharacter = '█')
        {
            return FromTasks(
                source.Select(s => s.ContinueWith(tsk => new ProgressBarTick())), 
                maxTicks,
                message, 
                color, 
                progressCharacter);
        }

        public static Task FromTasks(IEnumerable<Task<int>> source, int maxTicks, string message, ConsoleColor color = ConsoleColor.Green, char progressCharacter = '█')
        {
            return FromTasks(
                source.Select(s => s.ContinueWith(tsk => new ProgressBarTick(tsk.Result))),
                maxTicks,
                message,
                color, 
                progressCharacter);
        }

        public static Task FromTasks(IEnumerable<Task<string>> source, int maxTicks, string message, ConsoleColor color = ConsoleColor.Green, char progressCharacter = '█')
        {
            return FromTasks(
                source.Select(s => s.ContinueWith(tsk => new ProgressBarTick(tsk.Result))), 
                maxTicks, 
                message, 
                color, 
                progressCharacter);
        }

        public static async Task FromTasks(IEnumerable<Task<ProgressBarTick>> source, int maxTicks, string message, ConsoleColor color = ConsoleColor.Green, char progressCharacter = '█')
        {
            using (var pbar = new ProgressBar(maxTicks, message, color, progressCharacter))
            {
                await pbar.Monitor(source);
            }
        }

        public Task Monitor(IEnumerable<Task<string>> source)
        {
            return Monitor(source.Select(s => s.ContinueWith(tsk => new ProgressBarTick(tsk.Result))));
        }

        public Task Monitor(IEnumerable<Task<int>> source)
        {
            return Monitor(source.Select(s => s.ContinueWith(tsk => new ProgressBarTick(tsk.Result))));
        }

        public Task Monitor(IEnumerable<Task> source)
        {
            return Monitor(source.Select(s => s.ContinueWith(tsk => new ProgressBarTick())));
        }

        public async Task Monitor(IEnumerable<Task<ProgressBarTick>> source)
        {
            List<Task<ProgressBarTick>> tasks = source.ToList();

            while (tasks.Any())
            {
                Task<ProgressBarTick> task = await Task.WhenAny(tasks);
                tasks.Remove(task);

                int ticks = task.Result.Ticks;
                while (--ticks >= 0)
                {
                    Tick(task.Result.Message);
                }
            }
        }
    }
}
