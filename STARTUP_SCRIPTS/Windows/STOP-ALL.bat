@echo off
title Stop All Services
color 0C

echo ================================================
echo    STOPPING ALL GAMEVAULT SERVICES
echo ================================================
echo.

REM Kill Firebase Emulator processes
taskkill /F /FI "WINDOWTITLE eq Firebase Emulators*" 2>nul
taskkill /F /IM java.exe /FI "MEMUSAGE gt 50000" 2>nul

REM Kill Server
taskkill /F /FI "WINDOWTITLE eq GameVault Server*" 2>nul

REM Kill Client
taskkill /F /FI "WINDOWTITLE eq GameVault Client*" 2>nul

echo.
echo ================================================
echo    ALL SERVICES STOPPED
echo ================================================
echo.
pause