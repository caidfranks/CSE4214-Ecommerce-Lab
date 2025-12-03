using Microsoft.Playwright;
using Xunit;

namespace GameVault.Tests.Client;

public class ListingTests : TestBase
{
    private async Task<IPage> NavigateToListingAuditingPage()
    {
        var page = await LoginAsAdminAsync();

        await page.WaitForURLAsync(url => url.Contains("/adminLanding"));

        await page.ClickAsync("a:has-text('Listings')");
        await page.WaitForURLAsync(url => url.Contains("/viewListings"));
        await page.IsVisibleAsync("h3:has-text('@PageTitle')");

        return page;

    }

    [Fact]
    public async Task Admin_Can_View_All_Listing_Status_Tabs()
    {
        var page = await NavigateToListingAuditingPage();

        Assert.Contains("/viewListings", page.Url);
        Assert.True(await page.IsVisibleAsync("a:has-text('Pending')"));
        Assert.True(await page.IsVisibleAsync("a:has-text('Published')"));
        Assert.True(await page.IsVisibleAsync("a:has-text('Removed')"));
        Assert.True(await page.IsVisibleAsync("a:has-text('Unpublished')"));
    }

    [Fact]
    public async Task Admin_Pending_Listings_Appear_With_Basic_Fields()
    {
        var page = await NavigateToListingAuditingPage();

        await page.ClickAsync("a:has-text('Pending')");
        await page.WaitForURLAsync(url => url.Contains("/viewListings/pendingListings"));
        
        if (await page.IsVisibleAsync("text=Loading..."))
        {
            await page.WaitForSelectorAsync(".table, text=No accounts found with status \"pending\"");
        }

        if (await page.IsVisibleAsync("text=No accounts found"))
        {
            Assert.True(true, "No pending listings is a valid state.");
            return;
        }

        Assert.True(await page.IsVisibleAsync("th:has-text('Name')"));
        Assert.True(await page.IsVisibleAsync("th:has-text('User')"));
        Assert.True(await page.IsVisibleAsync("th:has-text('Status')"));

        await page.WaitForSelectorAsync(".table tbody tr");
        var firstRow = await page.QuerySelectorAsync(".table tbody tr");
        Assert.NotNull(firstRow);

        var cells = await firstRow.QuerySelectorAllAsync("td");
        Assert.True(cells.Count >= 3);

        Assert.True(await cells[0].IsVisibleAsync(), "Product Name should be visible");
        Assert.True(await cells[1].IsVisibleAsync(), "Owner ID should be visible");

        var statusText = await cells[2].InnerTextAsync();
        Assert.Contains("Pending", statusText);
    }

    [Fact]
    public async Task Admin_Can_View_Listing_Details_From_Pending_List()
    {
        var page = await NavigateToListingAuditingPage();

        await page.ClickAsync("a:has-text('Pending')");
        await page.WaitForURLAsync(url => url.Contains("/viewListings/pendingListings"));

        if (await page.IsVisibleAsync("text=Loading..."))
        {
            await page.WaitForSelectorAsync(".table, text=No accounts found");
        }

        if (await page.IsVisibleAsync("text=No accounts found with status \"pending\""))
        {
            Assert.True(true, "No pending listings is a valid state.");
            return;
        }

        await page.WaitForSelectorAsync(".table tbody tr");
        var firstRow = await page.QuerySelectorAsync(".table tbody tr");
        Assert.NotNull(firstRow);

        await page.ClickAsync("td a");
        await page.WaitForSelectorAsync("div[class='modal-content']");

        Assert.True(await page.IsVisibleAsync("text=Details"));
        Assert.True(await page.IsVisibleAsync("button:has-text('Approve')") || await page.IsVisibleAsync("button:has-text('Deny')"), "One of the action buttons must be available");

    }

    [Fact]
    public async Task Admin_Can_Approve_Pending_Listing_From_Modal()
    {
        var page = await NavigateToListingAuditingPage();

        await page.ClickAsync("a:has-text('Pending')");
        await page.WaitForURLAsync(url => url.Contains("/viewListings/pendingListings"));

        await page.WaitForSelectorAsync(".table tbody tr");
        var firstRow = await page.QuerySelectorAsync(".table tbody tr");

        await page.ClickAsync("td a");
        await page.WaitForSelectorAsync("div[class='modal-content']");
        await page.ClickAsync("button:has-text('Approve')");

        if (await page.IsVisibleAsync("text=Loading..."))
        {
            await page.WaitForSelectorAsync(".table, text=No accounts found");
        }

        if (await page.IsVisibleAsync("text=No accounts found with status \"pending\""))
        {
            Assert.True(true, "No pending listings is a valid state.");
            return;
        }
    }

    [Fact]
    public async Task Admin_Can_Deny_Pending_Listing_From_Modal()
    {
        var page = await NavigateToListingAuditingPage();

        await page.ClickAsync("a:has-text('Pending')");
        await page.WaitForURLAsync(url => url.Contains("/viewListings/pendingListings"));

        await page.WaitForSelectorAsync(".table tbody tr");
        var firstRow = await page.QuerySelectorAsync(".table tbody tr");

        await page.ClickAsync("td a");
        await page.WaitForSelectorAsync("div[class='modal-content']");
        await page.ClickAsync("button:has-text('Deny')");

        if (await page.IsVisibleAsync("text=Loading..."))
        {
            await page.WaitForSelectorAsync(".table, text=No accounts found");
        }

        if (await page.IsVisibleAsync("text=No accounts found with status \"pending\""))
        {
            Assert.True(true, "No pending listings is a valid state.");
            return;
        }
    }


}

