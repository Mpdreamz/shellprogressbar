"C:\Program Files (x86)\MSBuild\14.0\Bin\MsBuild.exe" ^
  build\Build.proj /p:NuspecFile=build\ShellProgressBar.nuspec;BUILD_NUMBER=%1 /t:NugetPackage