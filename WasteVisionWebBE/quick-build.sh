#!/bin/bash

# Color definitions
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
NC='\033[0m' # No Color

echo -e "${YELLOW}Starting quick build...${NC}"

# Skip clean and restore, just build the project
echo "Building the project (skipping clean and restore)..."
dotnet build --no-restore

if [ $? -eq 0 ]; then
    echo -e "${GREEN}Quick build completed successfully${NC}"
    exit 0
else
    echo -e "${RED}Build failed${NC}"
    exit 1
fi