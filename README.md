ShellProgressBar
===================
ShellProgressBar - display progress in your console application

# Example

![example](https://github.com/Mpdreamz/shellprogressbar/raw/master/doc/pbar-windows.gif)

# Install 

Get it on nuget:

http://www.nuget.org/packages/ShellProgressBar/

# Usage 

Usage is really straightforward

```
var maxTicks = 9000;
using (var pbar = new ProgressBar(maxTicks, "Starting", ConsoleColor.Cyan, '\u2593'))
{
	for (var i = 0; i<maxTicks; i++)
	{
		pbar.Tick("Currently processing " + i);
	}
}
```


### Credits 

The bulk of the code was taken from this article:
http://www.bytechaser.com/en/articles/ckcwh8nsyt/display-progress-bar-in-console-application-in-c.aspx
