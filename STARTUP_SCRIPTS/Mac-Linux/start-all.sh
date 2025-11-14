#!/bin/bash

# Get the directory where this script is located
SCRIPT_DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"

# Project root is two directories up from the script location
PROJECT_DIR="$(cd "$SCRIPT_DIR/../.." && pwd)"

echo "📁 Project directory: $PROJECT_DIR"
echo ""

# Start Firebase Emulators with data import/export
echo "🔥 Starting Firebase Emulators with data persistence..."
osascript -e "tell app \"Terminal\" to do script \"cd '$PROJECT_DIR' && firebase emulators:start --import ./.firebase_data --export-on-exit\""
sleep 8

# Start Server with emulator environment variables
echo "🖥️  Starting Server with Firebase Emulator settings..."
osascript -e "tell app \"Terminal\" to do script \"cd '$PROJECT_DIR/GameVault.Server' && FIRESTORE_EMULATOR_HOST='localhost:8080' FIREBASE_AUTH_EMULATOR_HOST='localhost:9099' dotnet run\""
sleep 3

# Start Client
echo "🌐 Starting Client..."
osascript -e "tell app \"Terminal\" to do script \"cd '$PROJECT_DIR/GameVault.Client' && dotnet run\""

echo ""
echo "✅ All services started in separate Terminal windows!"
echo ""
echo "🌐 Client: http://localhost:5287"
echo "🖥️  Server: http://localhost:5080"
echo "🔥 Firebase UI: http://localhost:4000"
echo ""
echo "📋 Default Credentials:"
echo "   Customer: customer@gmail.com / password"
echo "   Vendor:   vendor@gmail.com / password"
echo "   Admin:    admin@gmail.com / password"
echo ""
echo "To stop all services, run: $SCRIPT_DIR/stop-all.sh"
