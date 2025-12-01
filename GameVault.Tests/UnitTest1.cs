using Microsoft.Playwright;
using Xunit;

public class HomePageTests
{
    [Fact]
    public async Task Home_Page_Loads_And_Shows_Public_Buttons()
    {
        using var playwright = await Playwright.CreateAsync();

        var browser = await playwright.Chromium.LaunchAsync(new()
        {
            Headless = false,
            SlowMo = 50
        });

        var context = await browser.NewContextAsync(new()
        {
            //prevents redirect due to old login
            StorageState = null,
            IgnoreHTTPSErrors = true
        });

        // Clears any saved auth tokens
        await context.ClearCookiesAsync();
        await context.ClearPermissionsAsync();

        var page = await context.NewPageAsync();


        await page.GotoAsync("http://localhost:5166/", new()
        {
            WaitUntil = WaitUntilState.NetworkIdle
        });

        // Wait for Blazor WebAssembly to fully boot
        await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Wait for a stable root element
        await page.WaitForSelectorAsync(".home-container", new() { Timeout = 15000 });

        // Title
        Assert.True(await page.IsVisibleAsync("h1.home-title:text-is('Welcome to GameVault')"));

        // Subtitle
        Assert.True(await page.IsVisibleAsync("p.home-subtitle:text-is('Games for gamers')"));

        // Public buttons
        Assert.True(await page.IsVisibleAsync("button.home-button-primary:has-text('Join as Customer')"));
        Assert.True(await page.IsVisibleAsync("button.home-button-secondary:has-text('Become a Vendor')"));
        Assert.True(await page.IsVisibleAsync("a.home-button.home-button-outline:has-text('Sign In')"));

        // Footer text
        Assert.True(await page.IsVisibleAsync(".auth-footer-text:has-text('New to GameVault?')"));

        // Final sanity check: ensure page did NOT redirect due to auth
        Assert.EndsWith("/", page.Url);
    }

    // [Fact]
    // public void InstallBrowsers()
    // {

    //     // Test
    //     var exitCode = Microsoft.Playwright.Program.Main(new[] { "install" });
    //     if (exitCode != 0)
    //     {
    //         throw new Exception($"Playwright exited with code {exitCode}");
    //     }

    // }
}
