using Microsoft.Playwright;
using System.Threading.Tasks;
using Xunit;

namespace GameVault.Tests.Client;

public class TestBase : IAsyncLifetime
{
    protected IPlaywright _playwright;
    protected IBrowser _browser;

    public async Task InitializeAsync()
    {
        _playwright = await Playwright.CreateAsync();
        _browser = await _playwright.Chromium.LaunchAsync(new()
        {
            Headless = false,
            SlowMo = 50
        });
    }

    public async Task DisposeAsync()
    {
        await _browser.CloseAsync();
        _playwright?.Dispose();
    }

    protected async Task<IPage> NewPageAsync()
    {
        var context = await _browser.NewContextAsync(new()
        {
            StorageState = null,
            IgnoreHTTPSErrors = true
        });

        await context.ClearCookiesAsync();
        await context.ClearPermissionsAsync();

        return await context.NewPageAsync();
    }

    protected async Task LoginAsync(IPage page, string email, string password)
    {
        await page.GotoAsync($"{TestSettings.BaseUrl}/");
        await page.ClickAsync("a.home-button.home-button-outline:has-text('Sign In')");

        await page.FillAsync("#email", email);
        await page.FillAsync("#password", password);
        await page.ClickAsync("button.login-button");

        await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    protected async Task<IPage> LoginAsCustomerAsync()
    {
        var page = await NewPageAsync();
        await LoginAsync(page, TestSettings.CustomerEmail, TestSettings.CustomerPassword);
        return page;
    }

    protected async Task<IPage> LoginAsVendorAsync()
    {
        var page = await NewPageAsync();
        await LoginAsync(page, TestSettings.VendorEmail, TestSettings.VendorPassword);
        return page;
    }

    protected async Task<IPage> LoginAsAdminAsync()
    {
        var page = await NewPageAsync();
        await LoginAsync(page, TestSettings.AdminEmail, TestSettings.AdminPassword);
        return page;
    }
}