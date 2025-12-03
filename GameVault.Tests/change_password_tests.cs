using Microsoft.Playwright;
using Xunit;
using System.Text.RegularExpressions;
using GameVault.Tests.Client;

namespace GameVault.Tests.Pages;

public class ChangePasswordTests : TestBase
{
    private const string ClientBaseUrl = "http://localhost:5166";
    private const string LoginUrl = $"{ClientBaseUrl}/login";
    private const string AccountSettingsUrl = $"{ClientBaseUrl}/account/settings";

    private const string TestEmail = "testuser@example.com";
    private const string TestPassword = "Password123!";
    private const string NewTestPassword = "NewPassword123!";

    private async Task Login()
    {
        await Page.GotoAsync(LoginUrl);
        await Page.Locator("#email").FillAsync(TestEmail);
        await Page.Locator("#password").FillAsync(TestPassword);
        await Page.Locator("button[type='submit']").ClickAsync();
        await Page.WaitForURLAsync(new Regex("^((?!login).)*$"));
    }

    [Fact]
    public async Task FormDisplaysAllFields()
    {
        await Login();
        await Page.GotoAsync(AccountSettingsUrl);

        Assert.True(await Page.Locator("#currentPassword").IsVisibleAsync());
        Assert.True(await Page.Locator("#newPassword").IsVisibleAsync());
        Assert.True(await Page.Locator("#confirmPassword").IsVisibleAsync());
    }

    [Fact]
    public async Task ShowsErrorWhenPasswordsDoNotMatch()
    {
        await Login();
        await Page.GotoAsync(AccountSettingsUrl);

        await Page.Locator("#currentPassword").FillAsync(TestPassword);
        await Page.Locator("#newPassword").FillAsync(NewTestPassword);
        await Page.Locator("#confirmPassword").FillAsync("Mismatch123!");
        await Page.Locator("button[type='submit']").ClickAsync();

        Assert.True(await Page.Locator(".invalid-feedback, .alert-danger").IsVisibleAsync());
    }

    [Fact]
    public async Task SuccessfullyChangesPassword()
    {
        await Login();
        await Page.GotoAsync(AccountSettingsUrl);

        await Page.Locator("#currentPassword").FillAsync(TestPassword);
        await Page.Locator("#newPassword").FillAsync(NewTestPassword);
        await Page.Locator("#confirmPassword").FillAsync(NewTestPassword);
        await Page.Locator("button[type='submit']").ClickAsync();

        Assert.True(await Page.Locator(".alert-success").IsVisibleAsync());
    }

    [Fact]
    public async Task ShowsErrorForIncorrectCurrentPassword()
    {
        await Login();
        await Page.GotoAsync(AccountSettingsUrl);

        await Page.Locator("#currentPassword").FillAsync("WrongPassword123!");
        await Page.Locator("#newPassword").FillAsync(NewTestPassword);
        await Page.Locator("#confirmPassword").FillAsync(NewTestPassword);
        await Page.Locator("button[type='submit']").ClickAsync();

        Assert.True(await Page.Locator(".alert-danger").IsVisibleAsync());
    }

    [Fact]
    public async Task RequiresAuthentication()
    {
        await Page.Context.ClearCookiesAsync();
        await Page.GotoAsync(AccountSettingsUrl);

        Assert.Contains("login", Page.Url);
    }

    [Fact]
    public async Task TogglePasswordVisibilityWorks()
    {
        await Login();
        await Page.GotoAsync(AccountSettingsUrl);

        var input = Page.Locator("#currentPassword");
        var toggle = Page.Locator(".password-toggle").First;

        Assert.Equal("password", await input.GetAttributeAsync("type"));
        await toggle.ClickAsync();
        Assert.Equal("text", await input.GetAttributeAsync("type"));
    }

    [Fact]
    public async Task SendsChangePasswordRequestToApi()
    {
        await Login();

        var apiRequestTask = Page.WaitForRequestAsync(r =>
            r.Url.Contains("/api/auth/change-password") &&
            r.Method == "POST"
        );

        await Page.GotoAsync(AccountSettingsUrl);
        await Page.Locator("#currentPassword").FillAsync(TestPassword);
        await Page.Locator("#newPassword").FillAsync(NewTestPassword);
        await Page.Locator("#confirmPassword").FillAsync(NewTestPassword);
        await Page.Locator("button[type='submit']").ClickAsync();

        var request = await apiRequestTask;
        Assert.Contains("/change-password", request.Url);
    }
}
