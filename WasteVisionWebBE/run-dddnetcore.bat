@echo off
set ASPNETCORE_ENVIRONMENT=Development

REM Run the update_db.bat script

REM Build the project
echo .
echo Building the project...
call build.bat

REM Update the database
echo .
echo Updating the database...
call update_db.bat

REM Run tests
echo .
echo Running tests...
dotnet test

REM Display URLs
echo .
echo Starting the .NET application...
echo.
echo The .NET application is running locally
echo.
echo Project URLs:
echo Web (HTTP): http://localhost:3000
echo Web (HTTPS): https://localhost:3001
echo MySQL: localhost:3306
echo phpMyAdmin (LOCAL): http://localhost:8080
echo phpMyAdmin (PROD): http://localhost:8081

REM Run the application
dotnet run