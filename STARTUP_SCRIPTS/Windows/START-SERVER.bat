@echo off
title GameVault Server
color 0B

REM Get project root (two levels up from STARTUP_SCRIPTS\Windows)
set PROJECT_ROOT=%~dp0..\..

echo Starting GameVault Server with emulator environment variables...
cd /d "%PROJECT_ROOT%\GameVault.Server"
set FIRESTORE_EMULATOR_HOST=localhost:8080
set FIREBASE_AUTH_EMULATOR_HOST=localhost:9099
dotnet run