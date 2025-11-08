@echo off
title GameVault - Launcher
color 0A

REM Get project root (two levels up from STARTUP_SCRIPTS\Windows)
set PROJECT_ROOT=%~dp0..\..

echo ================================================
echo    GAMEVAULT STARTUP SEQUENCE
echo ================================================
echo.

REM Start Firebase Emulators (Yellow)
echo [1/3] Starting Firebase Emulators with data import/export...
start "Firebase Emulators" cmd /k "color 0E && cd /d "%PROJECT_ROOT%" && firebase emulators:start --import ./.firebase_data --export-on-exit"
timeout /t 5 /nobreak > nul

REM Start Server with environment variables (Cyan)
echo [2/3] Starting Server with emulator environment variables...
start "GameVault Server" cmd /k "color 0B && cd /d "%PROJECT_ROOT%\GameVault.Server" && set FIRESTORE_EMULATOR_HOST=localhost:8080 && set FIREBASE_AUTH_EMULATOR_HOST=localhost:9099 && dotnet run"
timeout /t 3 /nobreak > nul

REM Start Client (Green)
echo [3/3] Starting Client...
start "GameVault Client" cmd /k "color 0A && cd /d "%PROJECT_ROOT%\GameVault.Client" && dotnet run"

echo.
echo ================================================
echo    ALL SERVICES STARTED
echo ================================================
echo.
echo Client: http://localhost:5287
echo Server: http://localhost:5080
echo Firebase UI: http://localhost:4000
echo.
echo Default Login Credentials:
echo   Customer: customer@gmail.com / password
echo   Vendor: vendor@gmail.com / password
echo   Admin: admin@gmail.com / password
echo.
echo Press any key to close this window...
pause > nul