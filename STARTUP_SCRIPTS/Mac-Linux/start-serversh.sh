#!/bin/bash

PROJECT_DIR="$HOME/Downloads/CSE4214-Ecommerce-Lab"

cd "$PROJECT_DIR/GameVault.Server"

echo "🖥️  Starting Server with Firebase Emulator settings..."
echo "📁 Working directory: $(pwd)"
echo ""
echo "Environment Variables:"
echo "  FIRESTORE_EMULATOR_HOST=localhost:8080"
echo "  FIREBASE_AUTH_EMULATOR_HOST=localhost:9099"
echo ""

FIRESTORE_EMULATOR_HOST="localhost:8080" FIREBASE_AUTH_EMULATOR_HOST="localhost:9099" dotnet run
