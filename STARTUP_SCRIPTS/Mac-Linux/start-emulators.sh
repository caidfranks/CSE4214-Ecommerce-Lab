#!/bin/bash

# Get the directory where this script is located
SCRIPT_DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"

# Project root is two directories up from the script location
PROJECT_DIR="$(cd "$SCRIPT_DIR/../.." && pwd)"

cd "$PROJECT_DIR"

echo "🔥 Starting Firebase Emulators with data persistence..."
echo "📁 Working directory: $(pwd)"
echo ""
echo "Importing from: ./.firebase_data"
echo "Export on exit: enabled"
echo ""

firebase emulators:start --import ./.firebase_data --export-on-exit
