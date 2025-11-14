#!/bin/bash

echo "🛑 Stopping all GameVault services..."
echo ""

# Kill Firebase emulators
echo "Stopping Firebase emulators..."
pkill -f "firebase emulators" && echo "✅ Firebase stopped" || echo "ℹ️  Firebase not running"

# Kill dotnet processes (server and client)
echo "Stopping .NET Server and Client..."
pkill -f "dotnet run" && echo "✅ .NET processes stopped" || echo "ℹ️  No .NET processes running"
pkill -f "dotnet exec" && echo "✅ Exec processes stopped" || echo "ℹ️  No exec processes running"

echo ""
echo "✅ All services stopped!"
