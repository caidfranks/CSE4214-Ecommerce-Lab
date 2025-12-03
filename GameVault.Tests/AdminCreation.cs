using Microsoft.Playwright;
using Xunit;

namespace GameVault.Tests.Client;

public class AdminCreationTests : TestBase
{
    private async Task<IPage> NavigateToAdminCreatePage()
    {
        var page = await LoginAsAdminAsync();

        await page.WaitForURLAsync(url => url.Contains("/adminLanding"));

        await page.ClickAsync("a:has-text('Accounts')");
        await page.WaitForURLAsync(url => url.Contains("/manageAccounts"));

        await page.ClickAsync("button:has-text('Admin Accounts')");
        await page.WaitForURLAsync(url => url.Contains("/adminAccounts"));

        await page.ClickAsync("a:has-text('Admin Account')");
        await page.WaitForURLAsync(url => url.Contains("/register/admin"));
        await page.WaitForSelectorAsync("#email");

        return page;
    }

    [Fact]
    public async Task Admin_Can_Navigate_To_Create_Admin_Page()
    {
        var page = await NavigateToAdminCreatePage();

        Assert.Contains("/register/admin", page.Url);
        Assert.True(await page.IsVisibleAsync("input#email"));
        Assert.True(await page.IsVisibleAsync("input#password"));
        Assert.True(await page.IsVisibleAsync("button:has-text('Create Account')"));
    }

    [Fact]
    public async Task Admin_Create_Shows_Validation_When_Emails_Missing()
    {
        var page = await NavigateToAdminCreatePage();

        await page.EvaluateAsync("document.querySelector('form').dispatchEvent(new Event('submit', { bubbles: true }));");

        await page.WaitForTimeoutAsync(500);

        var emailClass = await page.GetAttributeAsync("#email", "class");
        var passwordClass = await page.GetAttributeAsync("#password", "class");

        Assert.Contains("is-invalid", emailClass);
        Assert.Contains("is-invalid", passwordClass);
    }

    [Fact]
    public async Task Admin_Can_Create_New_Admin_Successfully()
    {
        var page = await NavigateToAdminCreatePage();

        //Create random emails so auth will work
        var newAdminEmail = $"newadmin{Guid.NewGuid()}@gmail.com";

        await page.FillAsync("#email", newAdminEmail);
        await page.FillAsync("#password", "password");
        await page.ClickAsync("button:has-text('Create Account')");

        // await page.WaitForURLAsync($"{TestSettings.BaseUrl}/");
        await page.WaitForURLAsync(url => url.Contains("/adminLanding"), new() { Timeout = 10000 });
        Assert.Contains("/adminLanding", page.Url);
    }
}