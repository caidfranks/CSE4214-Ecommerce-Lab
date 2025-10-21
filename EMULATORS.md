# How to Use the Local Firebase Emulators

First time:
1. Install firebase CLI tools
2. Run ```firebase emulators:start --import ./emulator_baseline``` from the root directory of the repository
3. In another terminal, run ```firebase emulators:export ./.firebase_data```
4. Stop the emulators (Ctrl-C in the first terminal) and follow the normal steps

Every time:
1. Run ```firebase emulators:start --import ./.firebase_data --export-on-exit``` from the root directory of the repository
2. In another terminal, navigate to the server and run:
  * On Mac/Linux ```FIRESTORE_EMULATOR_HOST="localhost:8080" FIREBASE_AUTH_EMULATOR_HOST="localhost:9099" dotnet run```
  * On Windows/Powershell ```$env:FIRESTORE_EMULATOR_HOST="localhost:8080"; $env:FIREBASE_AUTH_EMULATOR_HOST="localhost:9099"; dotnet run```
3. In a third terminal, navigate to the client and run ```dotnet run```

## Default Credentials for Emulators

Customer:
Email: ```customer@gmail.com```
Password: ```password```

Vendor:
Email: ```vendor@gmail.com```
Password: ```password```

Admin:
Email: ```admin@gmail.com```
Password: ```password```