#!/bin/bash

# Clean the project
echo "Cleaning the project..."
dotnet clean

# Restore dependencies
echo "Restoring dependencies..."
dotnet restore

# Build the project
echo "Building the project..."
dotnet build

# Run tests
echo "Running tests..."
dotnet test --logger "console;verbosity=detailed"

echo "Build process completed."
