# Development Environment Setup Instructions

## Software Requirements
* IDE (recommend VSCode on Mac)
* Git
* .NET
* Firebase CLI Tools

## Mac Setup

### 1. Install .NET
[https://dotnet.microsoft.com/en-us/download](https://dotnet.microsoft.com/en-us/download)

### 2. Install the C# Dev Kit extension pack in Visual Studio Code
Make sure Visual Studio Code already installed

Helpful tutorial: [https://dotnet.microsoft.com/en-us/learn/aspnet/blazor-tutorial/intro](https://dotnet.microsoft.com/en-us/learn/aspnet/blazor-tutorial/intro)


# Notes

## Using VSCode
Solution Explorer very important

### New Project
Cmd-Shift-P: .NET: New Project...
Type "Blazor" and select "Blazor Web App"

### Debug new project
Fn-F5
Stop: Shift Fn-F5
Restart: Cmd Shift Fn-F5

## Using Dotnet CLI

### New Project
```bash
mkdir NewDir
dotnet new blazorwasm -n ProjectName -o src # uses src to put all files inside NewDir/src not NewDir/ProjectName

dotnet new gitignore
```
Manually add ```release``` to gitignore
Do Firebase Setup

Notes:
* blazorwasm, not blazor, because blazorwasm compiles to static site while blazor does not

### Run project
```bash
dotnet run --project ./src/ProjectName.csproj # use \ instead of / on Windows
```
Test through Firebase hosting:
```bash
firebase serve --only hosting
```

### Build project
```bash
dotnet publish ./src/ProjectName.csproj
```
Manual deploy
```bash
firebase deploy
```

### Firebase Setup
```bash
firebase init
```
On prompts:
Public directory: ```release/wwwroot```
Single-page app: ```Y```
Automated GitHub builds: ```Y```
Build script before every deploy? ```Y```
Build cmd: ```dotnet publish -c Release -o release src/ProjectName.csproj```
Auto-deploy on PR Merge? ```Y```

### Pre-Firebase Setup
From Firebase Console Hosting Setup Instructions

To install:
```npm install -g firebase-tools```
```firebase login```