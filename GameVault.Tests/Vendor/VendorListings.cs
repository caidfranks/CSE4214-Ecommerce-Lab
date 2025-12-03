using Microsoft.Playwright;
using Xunit;

using GameVault.Tests.Client;

namespace GameVault.Tests.Vendor;

public class VendorListingsTests : VendorBase
{
    [Fact]
    public async Task Vendor_Can_Navigate_To_Vendor_Listings_Page()
    {
        var page = await NavigateToVendorViewListingsPage();

        Assert.Contains("/vendor/listings", page.Url);
    }

    [Fact]
    public async Task Vendor_Can_Submit_Listing()
    {
        var page = await NavigateToVendorViewListingsPage();

        Assert.Contains("/vendor/listings", page.Url);

        // Wait for data to load
        await page.GetByText("Loading...").WaitForAsync(new() { State = WaitForSelectorState.Hidden });

        // Submit listing
        await page.GetByRole(AriaRole.Row).Filter(new()
        {
            Has = page.GetByRole(AriaRole.Cell, new() { Name = "FIFA 24" })
        }).GetByRole(AriaRole.Button, new()
        {
            Name = "Submit"
        }).ClickAsync();

        // Check pending orders
        await page.GetByRole(AriaRole.Button, new() { Name = "Pending" }).ClickAsync();

        // Wait for data to load
        await page.GetByText("Loading...").WaitForAsync(new() { State = WaitForSelectorState.Hidden });

        // Ensure now exists
        Assert.True(await page.GetByText("FIFA 24").IsVisibleAsync());
    }

    [Fact]
    public async Task Vendor_Can_Unsubmit_Listing()
    {
        var page = await NavigateToVendorViewListingsPage();

        Assert.Contains("/vendor/listings", page.Url);

        // Check pending orders
        await page.GetByRole(AriaRole.Button, new() { Name = "Pending" }).ClickAsync();

        // Wait for data to load
        await page.GetByText("Loading...").WaitForAsync(new() { State = WaitForSelectorState.Hidden });

        // Unsubmit listing
        await page.GetByRole(AriaRole.Row).Filter(new()
        {
            Has = page.GetByRole(AriaRole.Cell, new() { Name = "Fragments of Memory" })
        }).GetByRole(AriaRole.Button, new()
        {
            Name = "Cancel"
        }).ClickAsync();

        // Check inactive orders
        await page.GetByRole(AriaRole.Button, new() { Name = "Inactive" }).ClickAsync();

        // Wait for data to load
        await page.GetByText("Loading...").WaitForAsync(new() { State = WaitForSelectorState.Hidden });

        // Ensure now exists
        Assert.True(await page.GetByText("Fragments of Memory").IsVisibleAsync());
    }

    [Fact]
    public async Task Vendor_Can_Edit_Stock()
    {
        var page = await NavigateToVendorViewListingsPage();

        Assert.Contains("/vendor/listings", page.Url);

        // Check published orders
        await page.GetByRole(AriaRole.Button, new() { Name = "Published" }).ClickAsync();

        // Wait for data to load
        await page.GetByText("Loading...").WaitForAsync(new() { State = WaitForSelectorState.Hidden });

        // Make sure stock starts at 23
        await page.GetByRole(AriaRole.Row).Filter(new()
        {
            Has = page.GetByRole(AriaRole.Cell, new() { Name = "Mystline Chronicles" })
        }).GetByRole(AriaRole.Cell, new()
        {
            Name = "23",
            Exact = true
        }).IsVisibleAsync();

        // Start editing stock
        await page.GetByRole(AriaRole.Row).Filter(new()
        {
            Has = page.GetByRole(AriaRole.Cell, new() { Name = "Mystline Chronicles" })
        }).GetByRole(AriaRole.Button, new()
        {
            Name = "Change Stock"
        }).ClickAsync();

        // Add stock
        await page.GetByRole(AriaRole.Row).Filter(new()
        {
            Has = page.GetByRole(AriaRole.Cell, new() { Name = "Mystline Chronicles" })
        }).Locator("input").FillAsync("5");

        // Save stock change
        await page.GetByRole(AriaRole.Row).Filter(new()
        {
            Has = page.GetByRole(AriaRole.Cell, new() { Name = "Mystline Chronicles" })
        }).GetByRole(AriaRole.Button, new()
        {
            Name = "Save"
        }).ClickAsync();

        // Ensure now updated
        await page.GetByRole(AriaRole.Row).Filter(new()
        {
            Has = page.GetByRole(AriaRole.Cell, new() { Name = "Mystline Chronicles" })
        }).GetByRole(AriaRole.Cell, new()
        {
            Name = "28",
            Exact = true
        }).WaitForAsync();
    }

    [Fact]
    public async Task Vendor_Can_Unpublish_Listing()
    {
        var page = await NavigateToVendorViewListingsPage();

        Assert.Contains("/vendor/listings", page.Url);

        // Check published listings
        await page.GetByRole(AriaRole.Button, new() { Name = "Published" }).ClickAsync();

        // Wait for data to load
        await page.GetByText("Loading...").WaitForAsync(new() { State = WaitForSelectorState.Hidden });

        // Unpublish listing
        await page.GetByRole(AriaRole.Row).Filter(new()
        {
            Has = page.GetByRole(AriaRole.Cell, new() { Name = "Dreamshift" })
        }).GetByRole(AriaRole.Button, new()
        {
            Name = "Unpublish"
        }).ClickAsync();

        // Check inactive orders
        await page.GetByRole(AriaRole.Button, new() { Name = "Inactive" }).ClickAsync();

        // Wait for data to load
        await page.GetByText("Loading...").WaitForAsync(new() { State = WaitForSelectorState.Hidden });

        // Ensure now exists
        Assert.True(await page.GetByText("Dreamshift").IsVisibleAsync());
    }

    [Fact]
    public async Task Vendor_Can_Edit_Removed_Listing()
    {
        var page = await NavigateToVendorViewListingsPage();

        Assert.Contains("/vendor/listings", page.Url);

        // Check removed listings
        await page.GetByRole(AriaRole.Button, new() { Name = "Removed" }).ClickAsync();

        // Wait for data to load
        await page.GetByText("Loading...").WaitForAsync(new() { State = WaitForSelectorState.Hidden });

        // Select removed listing
        await page.GetByRole(AriaRole.Row).Filter(new()
        {
            Has = page.GetByRole(AriaRole.Cell, new() { Name = "Madden NFL 24 Special Edition" })
        }).GetByRole(AriaRole.Button, new()
        {
            Name = "Edit"
        }).ClickAsync();

        await page.WaitForURLAsync(url => url.Contains("/listings/edit"));

        await page.GetByPlaceholder("Unnamed Listing").FillAsync("Madden NFL 24");

        await page.GetByRole(AriaRole.Button, new() { Name = "Update Listing" }).ClickAsync();

        // Check for success message
        await page.Locator(".alert-success").WaitForAsync();

        // Visit View Listings Page to confirm updated
        await page.GetByRole(AriaRole.Button, new() { Name = "Cancel" }).ClickAsync();

        await page.WaitForURLAsync(url => url.Contains("/vendor/listings"));

        // Wait for data to load
        await page.GetByText("Loading...").WaitForAsync(new() { State = WaitForSelectorState.Hidden });

        Assert.True(await page.GetByRole(AriaRole.Cell, new() { Name = "Madden" }).TextContentAsync() == "Madden NFL 24");
    }

}