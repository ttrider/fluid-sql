rem @echo off
@SET mypath=%~dp0
pushd %mypath%

mkdir nuget

rd /s /q TTRider.FluidSQL\bin
rd /s /q TTRider.FluidSQL.MySql\bin
rd /s /q TTRider.FluidSQL.Postgres\bin
rd /s /q TTRider.FluidSQL.Postgres.Core\bin
rd /s /q TTRider.FluidSQL.Redshift\bin
rd /s /q TTRider.FluidSQL.RequestResponse\bin
rd /s /q TTRider.FluidSQL.Sqlite\bin
rd /s /q TTRider.FluidSQL\obj
rd /s /q TTRider.FluidSQL.MySql\obj
rd /s /q TTRider.FluidSQL.Postgres\obj
rd /s /q TTRider.FluidSQL.Postgres.Core\obj
rd /s /q TTRider.FluidSQL.Redshift\obj
rd /s /q TTRider.FluidSQL.RequestResponse\obj
rd /s /q TTRider.FluidSQL.Sqlite\obj

del .\nuget\*.nupkg

dotnet restore

dotnet build -c Release

copy .\TTRider.FluidSQL\bin\Release\*.nupkg .\nuget\
copy .\TTRider.FluidSQL.MySql\bin\Release\*.nupkg .\nuget\
copy .\TTRider.FluidSQL.Postgres\bin\Release\*.nupkg .\nuget\
copy .\TTRider.FluidSQL.Postgres.Core\bin\Release\*.nupkg .\nuget\
copy .\TTRider.FluidSQL.Redshift\bin\Release\*.nupkg .\nuget\
copy .\TTRider.FluidSQL.RequestResponse\bin\Release\*.nupkg .\nuget\
copy .\TTRider.FluidSQL.Sqlite\bin\Release\*.nupkg .\nuget\

for %%i in (.\nuget\*.nupkg) do dotnet nuget push -s nuget.org %%i





