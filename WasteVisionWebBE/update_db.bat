@echo off
setlocal EnableDelayedExpansion

:: Check if running in CI environment
if "%CI%"=="true" (
    echo Running in CI environment, skipping Docker setup...
) else (
    echo Starting database services...
    docker compose up --build -d

    echo Waiting for database to be ready...
    :wait_loop
    docker compose exec db mysqladmin ping -h localhost --silent > nul 2>&1
    if !ERRORLEVEL! neq 0 (
        echo Waiting for database connection...
        timeout /t 2 /nobreak > nul
        goto wait_loop
    )
)

echo Checking for pending migrations...
:: Create timestamp for migration name
for /f "tokens=2 delims==" %%I in ('wmic os get localdatetime /format:list') do set datetime=%%I
set "migration_name=%datetime:~0,14%"

:: Check migrations status
dotnet ef migrations list > migrations_status.tmp
findstr /C:"No migrations were found" migrations_status.tmp > nul
if !ERRORLEVEL! equ 0 (
    echo No migrations found. Creating initial migration...
    dotnet ef migrations add InitialCreate
) else (
    findstr /C:"(Pending)" migrations_status.tmp > nul
    if !ERRORLEVEL! equ 0 (
        echo Pending migrations found.
    ) else (
        echo Database is up to date.
    )
)

echo Applying migrations...
dotnet ef database update

:: Cleanup
if exist migrations_status.tmp del migrations_status.tmp

endlocal