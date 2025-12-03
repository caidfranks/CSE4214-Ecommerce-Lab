using Microsoft.Playwright;
using System.Security.Cryptography.X509Certificates;
using Xunit;

namespace GameVault.Tests.Client;

public class LoginTests : TestBase
{
    [Fact]
    public async Task Home_To_Login_Navigation_Works()
    {
        var page = await NewPageAsync();
        await page.GotoAsync($"{TestSettings.BaseUrl}/");

        await page.ClickAsync("a.home-button.home-button-outline:has-text('Sign In')");
        await page.WaitForURLAsync($"{TestSettings.BaseUrl}/login");

        Assert.True(await page.IsVisibleAsync(".login-container"));
    }

    [Fact]
    public async Task Login_Shows_Validation_When_Submitted_Empty()
    {
        var page = await NewPageAsync();
        await page.GotoAsync($"{TestSettings.BaseUrl}/");
        await page.ClickAsync("a.home-button.home-button-outline:has-text('Sign In')");

        await page.EvaluateAsync("document.querySelector('form').dispatchEvent(new Event('submit', { bubbles: true }));");

        await page.WaitForTimeoutAsync(500);

        var emailClass = await page.GetAttributeAsync("#email", "class");
        var passwordClass = await page.GetAttributeAsync("#password", "class");

        Assert.Contains("is-invalid", emailClass);
        Assert.Contains("is-invalid", passwordClass);
    }

    [Fact]
    public async Task Login_Shows_Error_For_Invalid_Email()
    {
        var page = await NewPageAsync();
        await page.GotoAsync($"{TestSettings.BaseUrl}/");
        await page.ClickAsync("a.home-button.home-button-outline:has-text('Sign In')");

        await page.FillAsync("#email", "invalidemail");
        await page.FillAsync("#password", "123456");

        await page.ClickAsync("button.login-button");
        await page.WaitForTimeoutAsync(30);

        Assert.Contains("/login", page.Url);
        Assert.True(await page.IsVisibleAsync("form.login-form"));
    }

    [Fact]
    public async Task Login_Shows_Error_On_Wrong_Credentials()
    {
        var page = await NewPageAsync();
        await page.GotoAsync($"{TestSettings.BaseUrl}/");

        await page.ClickAsync("a.home-button.home-button-outline:has-text('Sign In')");
        await page.FillAsync("#email", "wrong@test.com");
        await page.FillAsync("#password", "WrongPass123!");
        await page.ClickAsync("button.login-button");

        await page.WaitForSelectorAsync(".alert-danger");
        Assert.True(await page.IsVisibleAsync(".alert-danger"));
    }

    [Fact]
    public async Task Customer_Login_Redirects_To_Games()
    {
        var page = await NewPageAsync();
        await page.GotoAsync($"{TestSettings.BaseUrl}/");
        await page.ClickAsync("a.home-button.home-button-outline:has-text('Sign In')");

        await page.FillAsync("#email", TestSettings.CustomerEmail);
        await page.FillAsync("#password", TestSettings.CustomerPassword);
        await page.ClickAsync("button.login-button");

        await page.WaitForURLAsync(url => url.Contains($"{TestSettings.BaseUrl}/"));
        await page.WaitForURLAsync(url => url.Contains("/games"), new() { Timeout = 10000 });
        Assert.Contains("/games", page.Url);
    }

    [Fact]
    public async Task Vendor_Login_Redirects_To_VendorDashboard()
    {
        var page = await NewPageAsync();
        await page.GotoAsync($"{TestSettings.BaseUrl}/");
        await page.ClickAsync("a.home-button.home-button-outline:has-text('Sign In')");

        await page.FillAsync("#email", TestSettings.VendorEmail);
        await page.FillAsync("#password", TestSettings.VendorPassword);
        await page.ClickAsync("button.login-button");

        await page.WaitForURLAsync(url => url.Contains($"{TestSettings.BaseUrl}/"));
        await page.WaitForURLAsync(url => url.Contains("/vendor"), new() { Timeout = 10000 });
        Assert.Contains("/vendor", page.Url);
    }

    [Fact]
    public async Task Admin_Login_Redirects_To_AdminDashboard()
    {
        var page = await NewPageAsync();
        await page.GotoAsync($"{TestSettings.BaseUrl}/");
        await page.ClickAsync("a.home-button.home-button-outline:has-text('Sign In')");

        await page.FillAsync("#email", TestSettings.AdminEmail);
        await page.FillAsync("#password", TestSettings.AdminPassword);
        await page.ClickAsync("button.login-button");

        await page.WaitForURLAsync(url => url.Contains($"{TestSettings.BaseUrl}/"));
        await page.WaitForURLAsync(url => url.Contains("/adminLanding"), new() { Timeout = 10000 });
        Assert.Contains("/adminLanding", page.Url);
    }

    [Fact]
    public async Task Login_Toggle_Password_Visibility_Works()
    {
        var page = await NewPageAsync();
        await page.GotoAsync($"{TestSettings.BaseUrl}/");

        await page.ClickAsync("a.home-button.home-button-outline:has-text('Sign In')");

        Assert.Equal("password", await page.GetAttributeAsync("#password", "type"));

        await page.ClickAsync(".password-toggle");

        Assert.Equal("text", await page.GetAttributeAsync("#password", "type"));
    }
}
