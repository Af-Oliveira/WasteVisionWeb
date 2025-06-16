@echo off

REM Clean the project
echo Cleaning the project...
dotnet clean

REM Restore dependencies
echo Restoring dependencies...
dotnet restore

REM Build the project
echo Building the project...
dotnet build

REM Run tests
echo Running tests...
dotnet test --logger "console;verbosity=detailed"

echo Build process completed.
