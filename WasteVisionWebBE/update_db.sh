#!/bin/bash

if [ "$CI" = true ]; then
    echo "Running in CI environment, skipping Docker setup..."
else
    echo "Starting database services..."
    docker compose up --build -d

    echo "Waiting for database to be ready..."
    until docker compose exec db mysqladmin ping -h localhost --silent; do
        echo "Waiting for database connection..."
        sleep 2
    done
fi

echo "Checking for pending migrations..."
# define name migration + time
migration_name=$(date +"%Y%m%d%H%M%S")
dotnet ef migrations add $migration_name
if dotnet ef migrations list | grep -q "No migrations were found."; then
    echo "No migrations found. Creating initial migration..."
    dotnet ef migrations add InitialCreate
elif dotnet ef migrations list | grep -q "(Pending)"; then
    echo "Pending migrations found."
else
    echo "Database is up to date."
fi

echo "Applying migrations..."
dotnet ef database update
