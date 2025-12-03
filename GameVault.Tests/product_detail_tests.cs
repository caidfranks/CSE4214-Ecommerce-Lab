using Microsoft.Playwright;
using Xunit;
using System.Text.RegularExpressions;
using GameVault.Tests.Client;

namespace GameVault.Tests.Pages;

public class ProductDetailTests : TestBase
{
    private const string ClientBaseUrl = "http://localhost:5166";
    private const string LoginUrl = $"{ClientBaseUrl}/login";
    private const string GamesUrl = $"{ClientBaseUrl}/games";

    private const string TestCustomerEmail = "testcustomer@example.com";
    private const string TestCustomerPassword = "Password123!";

    private async Task LoginAsCustomer()
    {
        await Page.GotoAsync(LoginUrl);
        await Page.Locator("#email").FillAsync(TestCustomerEmail);
        await Page.Locator("#password").FillAsync(TestCustomerPassword);
        await Page.Locator("button[type='submit']").ClickAsync();

        await Page.WaitForURLAsync(new Regex("^((?!login).)*$"));
    }

    [Fact]
    public async Task ProductDetailDisplaysCorrectInfo()
    {
        await LoginAsCustomer();
        await Page.GotoAsync(GamesUrl);

        await Page.Locator(".product-card").First.ClickAsync();
        await Page.WaitForSelectorAsync(".product-detail");

        Assert.True(await Page.Locator(".product-title").IsVisibleAsync());
        Assert.True(await Page.Locator(".product-price").IsVisibleAsync());
    }

    [Fact]
    public async Task CanAddToCart()
    {
        await LoginAsCustomer();
        await Page.GotoAsync(GamesUrl);

        await Page.Locator(".product-card").First.ClickAsync();
        await Page.Locator(".add-to-cart").ClickAsync();

        await Page.WaitForSelectorAsync(".alert-success");
        Assert.True(await Page.Locator(".alert-success").IsVisibleAsync());
    }
}
