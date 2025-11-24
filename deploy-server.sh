#!/bin/bash

# Deploy Server to Google Cloud Run (Production)
echo "Building and deploying GameVault Server to production..."

# Build and push Docker image
echo "Building Docker image..."
gcloud builds submit \
  --tag us-central1-docker.pkg.dev/gamevault-9a27e/gamevault-repo/gamevault-server:latest \
  .

# Deploy to Cloud Run
echo "Deploying to Cloud Run..."
gcloud run deploy gamevault-server \
  --image us-central1-docker.pkg.dev/gamevault-9a27e/gamevault-repo/gamevault-server:latest \
  --platform managed \
  --region us-central1 \
  --allow-unauthenticated \
  --port 8080 \
  --set-secrets="/secrets/firebase-key.json=firebase-credentials:latest" \
  --set-env-vars="GOOGLE_APPLICATION_CREDENTIALS=/secrets/firebase-key.json,ASPNETCORE_ENVIRONMENT=Production"

echo "Deployment complete"
echo "Server URL: https://gamevault-server-819772831155.us-central1.run.app"
