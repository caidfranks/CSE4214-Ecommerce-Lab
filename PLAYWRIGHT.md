# Instructions for setting up Playwright

1. New xunit project
```sh
dotnet new xunit -o GameVault.Tests
```

2. Install playwright
```sh
dotnet add package Microsoft.Playwright.Xunit
```

3. Install browsers (on Mac)
    1. Paste following code into UnitTest1.cs
    ```csharp
        [Fact]
        public void InstallBrowsers()
        {

            Test
            var exitCode = Microsoft.Playwright.Program.Main(new[] { "install" });
            if (exitCode != 0)
            {
                throw new Exception($"Playwright exited with code {exitCode}");
            }

        }
    ```
    2. Run ```dotnet test``` and it should fail but then download
    3. Comment code out
    4. Run ```dotnet test``` and it should succeed

# Instructions for running Playwright tests

After installing Playwright and the browsers it needs:

1. First, run the Firestore emulators using the fresh emulator_baseline data:
```firebase emulators:start --import ./emulator_baseline```
from the root of the repository.

2. Start the client and server as normal (```dotnet watch``` and setting corresponding local emulator environment vairables)

3. Navigate to the ```GameVault.Tests``` directory

4. Run ```dotnet test```

If you want to watch the tests happening, uncomment lines 17-18 of TestBase.cs, then run dotnet test again.

If you want to test specific modules, call
```dotnet test --filter "ClassName"```
or
```dotnet test --filter "ClassName.Test_Name"```