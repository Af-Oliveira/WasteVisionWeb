#!/bin/bash

# Color definitions
RED='\033[0;31m'
GREEN='\033[0;32m'
BLUE='\033[0;34m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color
CHECK_MARK="✅"
CROSS_MARK="❌"

# Store the PID of the .NET application
DOTNET_PID=""

# Trap Ctrl+C and cleanup
cleanup() {
    echo -e "\n${YELLOW}Shutting down services...${NC}"
    if [ ! -z "$DOTNET_PID" ]; then
        kill $DOTNET_PID 2>/dev/null
    fi
    exit 0
}

trap cleanup SIGINT SIGTERM

# Function to check if a port is in use
check_port() {
    local port=$1
    if lsof -Pi :$port -sTCP:LISTEN -t >/dev/null ; then
        return 0
    else
        return 1
    fi
}

# Function to print separator line
print_separator() {
    printf "%0.s-" {1..70}
    printf "\n"
}

# Function to print status
print_status() {
    local service=$1
    local url=$2
    local port=$3
    
    printf "%-20s | %-30s | " "$service" "$url"
    if check_port $port; then
        printf "%s\n" "$CHECK_MARK"
    else
        printf "%s\n" "$CROSS_MARK"
    fi
}

# Set environment
export ASPNETCORE_ENVIRONMENT=Development

# Print header
clear
echo -e "${BLUE}Quick Development Environment Startup${NC}"
echo -e "${YELLOW}Starting services (skipping database updates)...${NC}\n"

# Make scripts executable
chmod +x quick-build.sh

# Run quick build script
echo -e "${YELLOW}Running quick build script...${NC}"
if ./quick-build.sh; then
    echo -e "${GREEN}Quick build completed successfully${NC}"
else
    echo -e "${RED}Build failed${NC}"
    exit 1
fi

# Start the application
echo -e "\n${YELLOW}Starting .NET application...${NC}"
dotnet run & 
DOTNET_PID=$!

# Wait for services to start
sleep 3

# Print service status table
echo -e "\n${BLUE}Service Status:${NC}"
print_separator
printf "%-20s | %-30s | %-10s\n" "SERVICE" "URL" "STATUS"
print_separator
print_status "Web (HTTP)" "http://localhost:3000" "3000"
print_status "Web (HTTPS)" "https://localhost:3001" "3001"
print_status "MySQL" "localhost:3306" "3306"
print_status "phpMyAdmin" "http://localhost:8080" "8080"
print_status "MailHog UI" "http://localhost:8025" "8025"
print_status "MailHog SMTP" "localhost:1025" "1025"
print_status "Keycloak UI" "http://localhost:8082" "8082"
print_separator

echo -e "\n${YELLOW}Services are running. Press Ctrl+C to stop all services${NC}"
echo -e "${YELLOW}Note: Database updates were skipped for faster startup${NC}"

# Wait for the dotnet process
wait $DOTNET_PID