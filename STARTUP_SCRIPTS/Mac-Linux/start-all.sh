#!/bin/bash

# Start Firebase Emulators
echo "🔥 Starting Firebase Emulators..."
osascript -e 'tell app "Terminal" to do script "cd \"'$(pwd)'\" && firebase emulators:start"'
sleep 5

# Start Server
echo "🖥️  Starting Server..."
osascript -e 'tell app "Terminal" to do script "cd \"'$(pwd)'/GameVault.Server\" && dotnet run"'
sleep 3

# Start Client
echo "🌐 Starting Client..."
osascript -e 'tell app "Terminal" to do script "cd \"'$(pwd)'/GameVault.Client\" && dotnet run"'

echo "✅ All services started!"
echo "Client: http://localhost:5287"
echo "Server: http://localhost:5080"
echo "Firebase UI: http://localhost:4000"