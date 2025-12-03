using Microsoft.Playwright;
using Xunit;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

using GameVault.Tests.Client;

namespace GameVault.Tests.Pages;

public class CartTests : TestBase
{
    private const string ClientBaseUrl = "http://localhost:5166";
    private const string CartUrl = $"{ClientBaseUrl}/cart";
    private const string GamesUrl = $"{ClientBaseUrl}/games";
    private const string LoginUrl = $"{ClientBaseUrl}/login";

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

    private async Task AddProductToCart()
    {
        await Page.GotoAsync(GamesUrl);
        var inStockProduct = Page.Locator(".product-card").Filter(new() { HasNot = Page.Locator(".sold-out-overlay") }).First;
        await inStockProduct.ClickAsync();
        var addToCartButton = Page.Locator("button:has-text('Add to Cart')");
        if (await addToCartButton.CountAsync() > 0)
        {
            await addToCartButton.ClickAsync();
            await Task.Delay(1000);
        }
    }

    [Fact]
    public async Task PageLoadsSuccessfully()
    {
        await Page.GotoAsync(CartUrl);
        Assert.True(await Page.Locator("h1:has-text('Shopping Cart')").IsVisibleAsync());
    }

    [Fact]
    public async Task RequiresAuthenticationToAccess()
    {
        await Page.Context.ClearCookiesAsync();
        await Page.GotoAsync(CartUrl);
        Assert.True(await Page.Locator("text=Please").IsVisibleAsync());
        Assert.True(await Page.Locator("a[href='/login']").IsVisibleAsync());
    }

    [Fact]
    public async Task EmptyCartDisplaysMessage()
    {
        await Page.GotoAsync(CartUrl);
        var emptyCart = Page.Locator(".empty-cart");
        if (await emptyCart.CountAsync() > 0)
        {
            Assert.True(await emptyCart.IsVisibleAsync());
            Assert.True(await emptyCart.Locator("p:has-text('cart is empty')").IsVisibleAsync());
            Assert.True(await emptyCart.Locator("button:has-text('Continue Shopping')").IsVisibleAsync());
        }
    }

    [Fact]
    public async Task CanAddProductToCart()
    {
        await AddProductToCart();
        await Page.GotoAsync(CartUrl);
        var emptyCart = Page.Locator(".empty-cart");
        Assert.True(await emptyCart.CountAsync() == 0);
    }

    [Fact]
    public async Task CanSelectQuantityBeforeAddingToCart()
    {
        await Page.GotoAsync(GamesUrl);
        var inStockProduct = Page.Locator(".product-card").Filter(new() { HasNot = Page.Locator(".sold-out-overlay") }).First;
        await inStockProduct.ClickAsync();

        var quantitySelect = Page.Locator("select.form-select");
        if (await quantitySelect.CountAsync() > 0)
        {
            await quantitySelect.SelectOptionAsync("2");
            Assert.Equal("2", await quantitySelect.InputValueAsync());
        }
    }
}
