@echo off
REM Deploy GameVault Client to Firebase Hosting

echo Deploying GameVault Client to Firebase Hosting...

REM Build the client for production
echo Building Blazor WebAssembly app...
cd GameVault.Client
dotnet publish -c Release -o ../release

if %errorlevel% neq 0 (
    echo Build failed. Check error messages above.
    cd ..
    exit /b 1
)

REM Return to root directory
cd ..

REM Deploy to Firebase Hosting
echo Deploying to Firebase Hosting...
firebase deploy --only hosting

if %errorlevel% equ 0 (
    echo.
    echo Client deployed successfully
    echo.
    echo To view your site:
    echo   firebase hosting:channel:list
    echo.
    echo Then open the hosting URL in your browser.
) else (
    echo.
    echo Deployment failed! Check the error messages above.
    exit /b 1
)
