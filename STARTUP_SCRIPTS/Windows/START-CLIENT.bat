@echo off
title GameVault Client
color 0A

REM Get project root (two levels up from STARTUP_SCRIPTS\Windows)
set PROJECT_ROOT=%~dp0..\..

echo Starting GameVault Client...
cd /d "%PROJECT_ROOT%\GameVault.Client"
dotnet run