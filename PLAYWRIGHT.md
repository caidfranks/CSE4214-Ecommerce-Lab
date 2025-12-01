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