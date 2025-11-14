#!/bin/bash

# Get the directory where this script is located
SCRIPT_DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"

# Project root is two directories up from the script location
PROJECT_DIR="$(cd "$SCRIPT_DIR/../.." && pwd)"

cd "$PROJECT_DIR/GameVault.Client"

echo "🌐 Starting Client..."
echo "📁 Working directory: $(pwd)"
echo ""

dotnet run
