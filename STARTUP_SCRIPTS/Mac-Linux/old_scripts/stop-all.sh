#!/bin/bash
echo "🛑 Stopping all GameVault services..."

# Kill Firebase emulators
pkill -f "firebase emulators"

# Kill dotnet processes (server and client)
pkill -f "dotnet run"
pkill -f "dotnet exec"

echo "✅ All services stopped!"