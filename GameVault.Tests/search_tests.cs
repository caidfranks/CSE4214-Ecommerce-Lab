using Microsoft.Playwright;
using Xunit;
using System.Text.RegularExpressions;
using GameVault.Tests.Client;

namespace GameVault.Tests.Pages;

public class SearchTests : TestBase
{
    private const string ClientBaseUrl = "http://localhost:5166";
    private const string SearchUrl = $"{ClientBaseUrl}/search?q=test";
    private const string LoginUrl = $"{ClientBaseUrl}/login";

    private const string TestCustomerEmail = "testcustomer@example.com";
    private const string TestCustomerPassword = "Password123!";

    private async Task Login()
    {
        await Page.GotoAsync(LoginUrl);
        await Page.Locator("#email").FillAsync(TestCustomerEmail);
        await Page.Locator("#password").FillAsync(TestCustomerPassword);
        await Page.Locator("button[type='submit']").ClickAsync();
        await Page.WaitForURLAsync(new Regex("^((?!login).)*$"));
    }

    [Fact]
    public async Task SearchPageLoads()
    {
        await Login();
        await Page.GotoAsync(SearchUrl);

        Assert.True(await Page.Locator(".search-results").IsVisibleAsync());
    }

    [Fact]
    public async Task ResultsAppearForQuery()
    {
        await Login();
        await Page.GotoAsync(SearchUrl);

        var results = Page.Locator(".product-card");
        Assert.True(await results.CountAsync() > 0);
    }
}
