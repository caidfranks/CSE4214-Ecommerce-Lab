@echo off
title Firebase Emulators
color 0E

REM Get project root (two levels up from STARTUP_SCRIPTS\Windows)
set PROJECT_ROOT=%~dp0..\..

echo Starting Firebase Emulators with data import/export...
cd /d "%PROJECT_ROOT%"
firebase emulators:start --import ./.firebase_data --export-on-exit