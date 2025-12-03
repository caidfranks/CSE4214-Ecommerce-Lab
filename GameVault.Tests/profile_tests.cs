using Microsoft.Playwright;
using Xunit;
using System.Text.RegularExpressions;
using GameVault.Tests.Client;

namespace GameVault.Tests.Pages;

public class ProfileTests : TestBase
{
    private const string ClientBaseUrl = "http://localhost:5166";
    private const string LoginUrl = $"{ClientBaseUrl}/login";
    private const string ProfileUrl = $"{ClientBaseUrl}/profile";

    private const string TestEmail = "testuser@example.com";
    private const string TestPassword = "Password123!";

    private async Task Login()
    {
        await Page.GotoAsync(LoginUrl);
        await Page.Locator("#email").FillAsync(TestEmail);
        await Page.Locator("#password").FillAsync(TestPassword);
        await Page.Locator("button[type='submit']").ClickAsync();
        await Page.WaitForURLAsync(new Regex("^((?!login).)*$"));
    }

    [Fact]
    public async Task CanLoadProfilePage()
    {
        await Login();
        await Page.GotoAsync(ProfileUrl);

        Assert.True(await Page.Locator(".profile-page").IsVisibleAsync());
    }

    [Fact]
    public async Task CanUpdateDisplayName()
    {
        await Login();
        await Page.GotoAsync(ProfileUrl);

        await Page.Locator("#displayName").FillAsync("New Name");
        await Page.Locator("button[type='submit']").ClickAsync();

        await Page.WaitForSelectorAsync(".alert-success");
        Assert.True(await Page.Locator(".alert-success").IsVisibleAsync());
    }
}
