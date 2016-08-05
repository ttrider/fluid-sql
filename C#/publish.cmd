@echo off
@SET mypath=%~dp0
pushd %mypath%
dotnet build -c Release -o .\nuget .\TTRider.FluidSQL.dll
dotnet build -c Release -o .\nuget .\TTRider.FluidSQL.Postgres
dotnet build -c Release -o .\nuget .\TTRider.FluidSQL.RequestResponse
dotnet build -c Release -o .\nuget .\TTRider.FluidSQL.Sqlite
dotnet build -c Release -o .\nuget .\xUnit.FluidSql
dotnet build -c Release -o .\nuget .\xUnit.Functional
dotnet build -c Release -o .\nuget .\xUnit.Postgres
dotnet build -c Release -o .\nuget .\xUnit.Sqlite

dotnet build -c Release -o .\nuget .\xUnit.FluidSql
dotnet build -c Release -o .\nuget .\xUnit.Functional
dotnet build -c Release -o .\nuget .\xUnit.Postgres
dotnet build -c Release -o .\nuget .\xUnit.Sqlite

dotnet pack -c Release -o .\nuget .\TTRider.FluidSQL.dll
dotnet pack -c Release -o .\nuget .\TTRider.FluidSQL.Postgres
dotnet pack -c Release -o .\nuget .\TTRider.FluidSQL.RequestResponse
dotnet pack -c Release -o .\nuget .\TTRider.FluidSQL.Sqlite

for %%i in (.\nuget\*.nupkg) do .\nuget\nuget push %%i

 


*.nupkg


popd