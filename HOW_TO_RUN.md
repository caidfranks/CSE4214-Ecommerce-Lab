GameVault

To run you need,

1. **.NET SDK 9.0 or higher**
   - Download from: https://dotnet.microsoft.com/download
   - Check installation: `dotnet --version`

2. **Node.js and npm** (for Firebase CLI)
   - Download from: https://nodejs.org/
   - Check installation: `node --version` and `npm --version`

3. **Firebase CLI**
   - Install: `npm install -g firebase-tools`
   - Check installation: `firebase --version`

4. **Java Runtime Environment** (for Firebase Emulators)
   - Download from: https://www.java.com/download/
   - Check installation: `java -version`
   - Minimum version: Java 11 or higher


Restore .NET dependencies (run from root directory)
dotnet restore


Running:

First time:
1. Install firebase CLI tools
2. Run ```firebase emulators:start --import ./emulator_baseline``` from the root directory of the repository
3. In another terminal, run ```firebase emulators:export ./.firebase_data```
4. Stop the emulators (Ctrl-C in the first terminal) and follow the normal steps

Close servers w/ Ctrl + C

Now, open START-ALL.bat in STARTUP_SCRIPTS\Windows



Testing:
1. Open the Application

Navigate to http://localhost:5166

2. Test Accounts:

Customer:
Email: ```customer@gmail.com```
Password: ```password```

Vendor:
Email: ```vendor@gmail.com```
Password: ```password```

Admin:
Email: ```admin@gmail.com```
Password: ```password```

Other vendors: (same password as above)
```from@software.net```
```vendor@nebula.works```
```dev@mythic.forge```
```boo@deep.night.en```
Applicant:
```chuck@norris.net```



View Firebase Emulator UI

Visit http://localhost:4000 to:
- View registered users (Authentication tab)
- Browse Firestore data (Firestore tab)
- Monitor real-time database operations
- Debug authentication issues



Test API Directly

Visit http://localhost:5080/swagger to:
- View all API endpoints
- Test API calls directly
- See request/response schemas


