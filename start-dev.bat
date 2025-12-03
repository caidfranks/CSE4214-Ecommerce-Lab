@echo off
REM Start GameVault in Development Mode with Firebase Emulators

echo Starting GameVault in Development Mode...

REM Set Firebase credentials
set GOOGLE_APPLICATION_CREDENTIALS=%USERPROFILE%\firebase-credentials.json

REM Check if Firebase emulators are already running
netstat -ano | findstr ":8080" >nul 2>&1
if %errorlevel% equ 0 (
    echo Firebase emulators already running
) else (
    echo Starting Firebase emulators...
    start "Firebase Emulators" cmd /k "firebase emulators:start --import=./firebase-data --export-on-exit"

    echo Waiting for emulators to be ready...
    timeout /t 5 /nobreak >nul
)

REM Start the server in development mode
echo Starting server (Development mode with emulators)...
start "GameVault Server" cmd /k "cd GameVault.Server && set ASPNETCORE_ENVIRONMENT=Development && dotnet run"

REM Wait a moment for server to start
timeout /t 3 /nobreak >nul

REM Start the client
echo Starting client...
cd GameVault.Client
set ASPNETCORE_ENVIRONMENT=Development
dotnet run

echo.
echo Server running at http://localhost:5080
echo Client running at http://localhost:5166
echo Firebase emulators at http://localhost:4000
