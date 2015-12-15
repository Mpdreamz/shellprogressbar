using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShellProgressBar
{
    public class ProgressBarTick
    {
        public ProgressBarTick()
        {
            Message = "";
            Ticks = 1;
        }

        public ProgressBarTick(string message)
            : this()
        {
            Message = message;
        }

        public ProgressBarTick(int ticks)
            : this()
        {
            Ticks = ticks;
        }

        public string Message { get; set; }
        public int Ticks { get; set; }
    }
}
