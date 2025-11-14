#!/bin/bash


set -e 

RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m'

print_success() {
    echo -e "${GREEN}✓ $1${NC}"
}

print_error() {
    echo -e "${RED}✗ $1${NC}"
}

print_info() {
    echo -e "${YELLOW}→ $1${NC}"
}

# Check if firebase is installed
if ! command -v firebase &> /dev/null; then
    print_error "Firebase CLI is not installed. Install with:"
    echo "  npm install -g firebase-tools"
    exit 1
fi

# Check if dotnet is installed
if ! command -v dotnet &> /dev/null; then
    print_error ".NET SDK is not installed. Please install it first:"
    echo "https://dotnet.microsoft.com/download"
    exit 1
fi

echo "Step 1: Building Client"
echo "--------------------------------"
print_info "Building Blazor WebAssembly in Release mode..."
cd GameVault.Client
dotnet publish -c Release -o ../release || {
    print_error "Build failed"
    exit 1
}
cd ..
print_success "Client built successfully"
echo ""

echo "Step 2: Deploying to Firebase Hosting"
echo "--------------------------------"
print_info "Deploying to Firebase..."
firebase deploy --only hosting || {
    print_error "Firebase deployment failed. Make sure you're logged in:"
    echo "  firebase login"
    exit 1
}
print_success "Client deployed successfully"
echo ""

echo "======================================"
print_success "Deployment Complete"
echo ""
echo "Your app is now live at:"
echo "  https://gamevault-9a27e.web.app"
echo ""
