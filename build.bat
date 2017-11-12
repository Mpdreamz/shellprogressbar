@ECHO OFF

pushd src

dotnet restore

IF "%~1"=="" ( dotnet build )
IF NOT "%~1"=="" ( dotnet pack -c Release /p:Version=%1 )

popd 

