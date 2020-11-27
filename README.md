ShellProgressBar
===================
visualize (concurrent) progress in your console application

This is a great little library to visualize long running command line tasks.

.NET Core ready!

It also supports spawning child progress bars which allows you to visualize dependencies and concurrency rather nicely.

Tested on OSX 

![example osx](https://github.com/Mpdreamz/shellprogressbar/raw/master/doc/pbar-osx.gif)

and Windows 

![example win cmd](https://github.com/Mpdreamz/shellprogressbar/raw/master/doc/pbar-windows.gif)

(Powershell works too, see example further down)

# Install 

Get it on nuget: http://www.nuget.org/packages/ShellProgressBar/

# Usage 

Usage is really straightforward

```csharp
const int totalTicks = 10;
var options = new ProgressBarOptions
{
    ProgressCharacter = '─',
    ProgressBarOnBottom = true
};
using (var pbar = new ProgressBar(totalTicks, "Initial message", options))
{
    pbar.Tick(); //will advance pbar to 1 out of 10.
    //we can also advance and update the progressbar text
    pbar.Tick("Step 2 of 10"); 
}
```

## Reporting progression

There are two ways to report progression. You can use the `Tick()` function as described above. Alternatively you can report progression through an [`IProgress<T>`](https://docs.microsoft.com/en-us/dotnet/api/system.iprogress-1) instance that you obtain by calling `AsProgress<T>()` on the progress bar object.

For a simple case where the progress type is a `float` value between 0.0 and 1.0 that represents the completion percentage, use `progressBar.AsProgress<float>()`:

```csharp
using ProgressBar progressBar = new ProgressBar(10000, "My Progress Message");
IProgress progress = progressBar.AsProgress<float>();
progress.Report(0.25); // Advances the progress bar to 25%
```

See `IntegrationWithIProgressExample.cs` and `IntegrationWithIProgressPercentageExample.cs` in the [src/ShellProgressBar.Example/Examples](src/ShellProgressBar.Example/Examples) directory for full examples.

## Options

### Progress bar position

```csharp
const int totalTicks = 10;
var options = new ProgressBarOptions
{
	ProgressCharacter = '─',
	ProgressBarOnBottom = true
};
using (var pbar = new ProgressBar(totalTicks, "progress bar is on the bottom now", options))
{
	TickToCompletion(pbar, totalTicks, sleep: 500);
}
```

By default the progress bar is at the top and the message at the bottom.
This can be flipped around if so desired.

![bar_on_bottom](https://github.com/Mpdreamz/shellprogressbar/raw/master/doc/bar-on-bottom-osx.gif)

### Styling changes

```csharp
const int totalTicks = 10;
var options = new ProgressBarOptions
{
	ForegroundColor = ConsoleColor.Yellow,
	ForegroundColorDone = ConsoleColor.DarkGreen,
	BackgroundColor = ConsoleColor.DarkGray,
	BackgroundCharacter = '\u2593'
};
using (var pbar = new ProgressBar(totalTicks, "showing off styling", options))
{
	TickToCompletion(pbar, totalTicks, sleep: 500);
}
```

Many aspects can be styled including foreground color, background (inactive portion)
and changing the color on completion.

![styling](https://github.com/Mpdreamz/shellprogressbar/raw/master/doc/styling-windows.gif)


### No real time update

By default a timer will draw the screen every 500ms. You can configure the progressbar 
to only be drawn when `.Tick()` is called.

```csharp
const int totalTicks = 5;
var options = new ProgressBarOptions
{
	DisplayTimeInRealTime = false
};
using (var pbar = new ProgressBar(totalTicks, "only draw progress on tick", options))
{
	TickToCompletion(pbar, totalTicks, sleep:1750);
}
```

If you look at the time passed you will see it skips `02:00`


![update_on_tick](https://github.com/Mpdreamz/shellprogressbar/raw/master/doc/update-on-tick-osx.gif)

### Descendant progressbars

A progressbar can spawn child progress bars and each child can spawn
its own progressbars. Each child can have its own styling options.

This is great to visualize concurrent running tasks.

```csharp
const int totalTicks = 10;
var options = new ProgressBarOptions
{
	ForegroundColor = ConsoleColor.Yellow,
	BackgroundColor = ConsoleColor.DarkYellow,
	ProgressCharacter = '─'
};
var childOptions = new ProgressBarOptions
{
	ForegroundColor = ConsoleColor.Green,
	BackgroundColor = ConsoleColor.DarkGreen,
	ProgressCharacter = '─'
};
using (var pbar = new ProgressBar(totalTicks, "main progressbar", options))
{
	TickToCompletion(pbar, totalTicks, sleep: 10, childAction: () =>
	{
		using (var child = pbar.Spawn(totalTicks, "child actions", childOptions))
		{
			TickToCompletion(child, totalTicks, sleep: 100);
		}
	});
}
```

![children](https://github.com/Mpdreamz/shellprogressbar/raw/master/doc/children-osx.gif)

By default children will collapse when done, making room for new/concurrent progressbars.

You can keep them around by specifying `CollapseWhenFinished = false`

```csharp
var childOptions = new ProgressBarOptions
{
	ForegroundColor = ConsoleColor.Green,
	BackgroundColor = ConsoleColor.DarkGreen,
	ProgressCharacter = '─',
	CollapseWhenFinished = false
};
```

![children_no_collapse](https://github.com/Mpdreamz/shellprogressbar/raw/master/doc/children-no-collapse-windows.gif)


# FixedDurationBar

`ProgressBar` is great for visualizing tasks with an unknown runtime. If you have a task that you know takes a fixed amount of time there is also a `FixedDurationBar` subclass.
`FixedDurationBar` will `Tick()` automatically but other then that all the options and usage are the same. Except it relies on the real time update feature so disabling that 
will throw.

`FixedDurationBar` exposes an `IsCompleted` and `CompletedHandle` 


### Credits 

The initial implementation was inspired by this article.
http://www.bytechaser.com/en/articles/ckcwh8nsyt/display-progress-bar-in-console-application-in-c.aspx

And obviously anyone who sends a PR to this repository :+1:
