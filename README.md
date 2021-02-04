ShellProgressBar
===================
This is just a fork on https://github.com/Mpdreamz/shellprogressbar

So full credit goes to https://github.com/Mpdreamz

# Reason

The reason for the fork is just to strip the build down to run on the dotnet 5.0 framework. I have removed the previous supported frameworks.



# Usage (exerpt source:// https://github.com/Mpdreamz/shellprogressbar )

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

....


### Credits listed on https://github.com/Mpdreamz

The initial implementation was inspired by this article.
http://www.bytechaser.com/en/articles/ckcwh8nsyt/display-progress-bar-in-console-application-in-c.aspx

