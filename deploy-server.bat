@echo off
REM Deploy GameVault Server to Google Cloud Run

echo Deploying GameVault Server to Cloud Run...

REM Set project
gcloud config set project gamevault-9a27e

REM Build and deploy using Cloud Build
echo Building Docker image and deploying...
gcloud builds submit --config cloudbuild.yaml

if %errorlevel% equ 0 (
    echo.
    echo Server deployed successfully
    echo Server URL: https://gamevault-server-819772831155.us-central1.run.app
    echo.
    echo To view logs:
    echo   gcloud run services logs read gamevault-server --limit 50
) else (
    echo.
    echo ❌ Deployment failed! Check the error messages above.
    exit /b 1
)
