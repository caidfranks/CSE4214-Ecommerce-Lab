#!/bin/bash

echo "Starting GameVault in Development Mode..."

if lsof -Pi :8080 -sTCP:LISTEN -t >/dev/null ; then
    echo "Firebase emulators already running"
else
    echo "Starting Firebase emulators..."
    firebase emulators:start --import=./firebase-data --export-on-exit &
    
    echo "Waiting for emulators to be ready..."
    sleep 5
fi

echo "Starting server (Development mode with emulators)..."
cd GameVault.Server
ASPNETCORE_ENVIRONMENT=Development dotnet run

echo "Server running at http://localhost:5080"
echo "Firebase emulators at http://localhost:4000"
