#!/bin/bash

# Get the directory where this script is located
SCRIPT_DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"

# Project root is two directories up from the script location
PROJECT_DIR="$(cd "$SCRIPT_DIR/../.." && pwd)"

cd "$PROJECT_DIR/GameVault.Server"

echo "🖥️  Starting Server with Firebase Emulator settings..."
echo "📁 Working directory: $(pwd)"
echo ""
echo "Environment Variables:"
echo "  FIRESTORE_EMULATOR_HOST=localhost:8080"
echo "  FIREBASE_AUTH_EMULATOR_HOST=localhost:9099"
echo ""

FIRESTORE_EMULATOR_HOST="localhost:8080" FIREBASE_AUTH_EMULATOR_HOST="localhost:9099" dotnet run
