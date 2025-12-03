using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using System.Text.RegularExpressions;

namespace GameVault.Tests.Pages;

public class ProductBrowsingTests : PageTest
{
    private const string ClientBaseUrl = "http://localhost:5166";
    private const string GamesUrl = $"{ClientBaseUrl}/games";
    private const string LoginUrl = $"{ClientBaseUrl}/login";

    private const string TestCustomerEmail = "testcustomer@example.com";
    private const string TestCustomerPassword = "Password123!";

    [Fact]
    public async Task Setup()
    {
        await LoginAsCustomer();
    }

    private async Task LoginAsCustomer()
    {
        await Page.GotoAsync(LoginUrl);
        await Page.Locator("#email").FillAsync(TestCustomerEmail);
        await Page.Locator("#password").FillAsync(TestCustomerPassword);
        await Page.Locator("button[type='submit']").ClickAsync();

        await Page.WaitForURLAsync(new Regex("^((?!login).)*$"), new PageWaitForURLOptions { Timeout = 5000 }
);

    }

    [Fact]
    public async Task CanSelectSingleFilter()
    {
        await Page.GotoAsync(GamesUrl);
        await Page.WaitForSelectorAsync(".filter-options .form-check", new() { Timeout = 5000 });

        var firstCheckbox = Page.Locator(".filter-options .form-check-input").First;
        await firstCheckbox.CheckAsync();

        await Expect(firstCheckbox).ToBeCheckedAsync();
    }

    [Fact]

    public async Task CanRemoveIndividualFilter()
    {
        await Page.GotoAsync(GamesUrl);
        await Page.WaitForSelectorAsync(".filter-options", new() { Timeout = 5000 });

        var firstCheckbox = Page.Locator(".filter-options .form-check-input").First;
        await firstCheckbox.CheckAsync();

        await Page.Locator("button:has-text('Apply Filter')").ClickAsync();
        await Task.Delay(500);

        var closeButton = Page.Locator(".active-filters .btn-close").First;
        await closeButton.ClickAsync();
        await Task.Delay(500);

        await Expect(Page.Locator(".active-filters")).Not.ToBeVisibleAsync();
    }

    [Fact]
    public async Task SearchBarIsAccessibleFromBrowsePage()
    {
        await Page.GotoAsync(GamesUrl);

        var searchInput = Page.Locator("input.search-input");
        await Expect(searchInput).ToBeVisibleAsync();
        await Expect(searchInput).ToBeEnabledAsync();
    }

    [Fact]
    public async Task CanSearchFromBrowsePage()
    {
        await Page.GotoAsync(GamesUrl);

        var searchInput = Page.Locator("input.search-input");
        await searchInput.FillAsync("test game");

        var searchButton = Page.Locator("button.search-button");
        await searchButton.ClickAsync();

        await Page.WaitForURLAsync(new Regex(".*/search\\?q=.*"), new PageWaitForURLOptions { Timeout = 5000 }
);

    }
}
