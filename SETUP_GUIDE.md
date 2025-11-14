## Prerequisites

Install these first:
- **.NET 9 SDK** - https://dotnet.microsoft.com/download
- **Node.js 18+** - https://nodejs.org/
- **Firebase CLI** - Run: `npm install -g firebase-tools`
- **Google Cloud SDK** - https://cloud.google.com/sdk/docs/install
- **Git** - https://git-scm.com/downloads

**Get Firebase Credentials**
- Get `firebase-credentials.json`
- Save it to:
  - **Windows**: `C:\Users\YourUsername\firebase-credentials.json`
  - **Mac/Linux**: `~/firebase-credentials.json`

### Login to Firebase
firebase login

# Login to Google Cloud
gcloud auth login
gcloud config set project gamevault-9a27e


## Local Development

### Run Both Server & Client Locally
**Mac/Linux:**
```bash
./start-dev.sh
```

**Windows:**
```cmd
start-dev.bat
```

This starts:
- **Server**: http://localhost:5080
- **Client**: http://localhost:5166
- **Firebase Emulators**: http://localhost:4000

### Deploy Server to Cloud Run
**Mac/Linux:**
```bash
./deploy-server.sh
```

**Windows:**
```cmd
deploy-server.bat
```


### Deploy Client to Firebase Hosting
**Mac/Linux:**
```bash
./deploy-client.sh
```

**Windows:**
```cmd
deploy-client.bat
```

### Deploy Both
**Mac/Linux:**
```bash
./deploy-server.sh && ./deploy-client.sh
```

**Windows:**
```cmd
deploy-server.bat
deploy-client.bat
```

## Useful Commands
All commands work on all platforms:

```bash
# View server logs
gcloud run services logs read gamevault-server --limit 50

# View server status
gcloud run services describe gamevault-server --region us-central1

# View Firebase hosting status
firebase hosting:channel:list

# Build server locally
dotnet build GameVault.Server

# Build client locally
dotnet build GameVault.Client

# Run tests (if any)
dotnet test

# Format code
dotnet format
```

## Team Workflow

### Making Changes
1. Pull latest: `git pull origin main`
2. Create branch: `git checkout -b feature/your-feature`
3. Make changes
4. Test locally (see Local Development section for your OS)
5. Commit: `git commit -m "Description"`
6. Push: `git push origin feature/your-feature`
7. Create Pull Request

### Deploying to Production
1. Merge PR to `main`
2. Pull latest: `git pull origin main`
3. Deploy server (see Deployment section for your OS)
4. Deploy client (see Deployment section for your OS)
5. Test production with checklist

## Platform-Specific Tips
### Windows Users
- Set execution policy if scripts don't run: `Set-ExecutionPolicy RemoteSigned`

### Mac/Linux Users
- Make scripts executable: `chmod +x *.sh`


## Test Accounts (Production)
### Customers
- alice@gamevault.com / Password123
- bob@gamevault.com / Password123
- carol@gamevault.com / Password123

### Vendors
- gamestoppro@gamevault.com / Password123
- retrogamescentral@gamevault.com / Password123
- digitaldreams@gamevault.com / Password123
- epicdealsgaming@gamevault.com / Password123

### Admin
- admin@gamevault.com / Admin123