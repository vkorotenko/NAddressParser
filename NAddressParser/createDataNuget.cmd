copy ..\LICENSE LICENSE.txt /Y
nuget pack NAddressParser.Data.nuspec
md bin\release /Y
move /Y *.nupkg bin\release
del LICENSE.txt