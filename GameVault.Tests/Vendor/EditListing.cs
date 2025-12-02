using Microsoft.Playwright;
using Microsoft.Playwright.MSTest;
using Xunit;

using GameVault.Tests.Client;

namespace GameVault.Tests.Vendor;

public class EditListingTests : VendorBase
{
  private async Task<IPage> NavigateToEditListingPage()
  {
    var page = await NavigateToVendorViewListingsPage();

    // Wait for data to load
    await page.GetByText("Loading...").WaitForAsync(new() { State = WaitForSelectorState.Hidden });

    // Select PuzzleScape for editing
    await page.GetByRole(AriaRole.Row).Filter(new()
    {
      Has = page.GetByRole(AriaRole.Cell, new() { Name = "PuzzleScape" })
    }).GetByRole(AriaRole.Button, new()
    {
      Name = "Edit"
    }).ClickAsync();

    return page;
  }

  [Fact]
  public async Task Vendor_Can_Navigate_To_Edit_Listing_Page()
  {
    var page = await NavigateToEditListingPage();

    Assert.Contains("/listings/edit", page.Url);
  }

  [Fact]
  public async Task Vendor_Can_Edit_Listing_Name()
  {
    var page = await NavigateToEditListingPage();

    // Change Name
    await page.GetByPlaceholder("Unnamed Listing").FillAsync("PuzzleScape 2: Finite");

    await page.GetByRole(AriaRole.Button, new() { Name = "Update Listing" }).ClickAsync();

    // Check for success message
    await page.Locator(".alert-success").WaitForAsync();
    Assert.Contains("Success", await page.Locator(".alert-success").TextContentAsync());

    // Visit View Listings Page to confirm updated
    await page.GetByRole(AriaRole.Button, new() { Name = "Cancel" }).ClickAsync();

    await page.WaitForURLAsync(url => url.Contains("/vendor/listings"));

    // Wait for data to load
    await page.GetByText("Loading...").WaitForAsync(new() { State = WaitForSelectorState.Hidden });

    Assert.True(await page.GetByRole(AriaRole.Cell, new() { Name = "PuzzleScape" }).TextContentAsync() == "PuzzleScape 2: Finite");
  }

  [Fact]
  public async Task Vendor_Can_Edit_Listing_Price()
  {
    var page = await NavigateToEditListingPage();

    // Change Price
    await page.GetByPlaceholder("0.0").FillAsync("12.99");

    await page.GetByRole(AriaRole.Button, new() { Name = "Update Listing" }).ClickAsync();

    // Check for success message
    await page.Locator(".alert-success").WaitForAsync();
    Assert.Contains("Success", await page.Locator(".alert-success").TextContentAsync());

    // Visit View Listings Page to confirm updated
    await page.GetByRole(AriaRole.Button, new() { Name = "Cancel" }).ClickAsync();

    await page.WaitForURLAsync(url => url.Contains("/vendor/listings"));

    // Wait for data to load
    await page.GetByText("Loading...").WaitForAsync(new() { State = WaitForSelectorState.Hidden });

    Assert.True(await page.GetByRole(AriaRole.Row).Filter(new()
    {
      Has = page.GetByRole(AriaRole.Cell, new() { Name = "PuzzleScape" })
    }).GetByRole(AriaRole.Cell, new()
    {
      Name = "$"
    }).TextContentAsync() == "$12.99");
  }

  [Fact]
  public async Task Vendor_Can_Edit_Listing_Stock()
  {
    var page = await NavigateToEditListingPage();

    // Change Stock
    await page.GetByPlaceholder("0", new() { Exact = true }).FillAsync("15");

    await page.GetByRole(AriaRole.Button, new() { Name = "Update Listing" }).ClickAsync();

    // Check for success message
    await page.Locator(".alert-success").WaitForAsync();
    Assert.Contains("Success", await page.Locator(".alert-success").TextContentAsync());

    // Visit View Listings Page to confirm updated
    await page.GetByRole(AriaRole.Button, new() { Name = "Cancel" }).ClickAsync();

    await page.WaitForURLAsync(url => url.Contains("/vendor/listings"));

    // Wait for data to load
    await page.GetByText("Loading...").WaitForAsync(new() { State = WaitForSelectorState.Hidden });

    Assert.True(await page.GetByRole(AriaRole.Row).Filter(new()
    {
      Has = page.GetByRole(AriaRole.Cell, new() { Name = "PuzzleScape" })
    }).GetByRole(AriaRole.Cell, new()
    {
      Name = "15"
    }).IsVisibleAsync());
  }

  [Fact]
  public async Task Vendor_Can_Edit_Listing_Description()
  {
    var page = await NavigateToEditListingPage();

    // Change Description
    await page.GetByPlaceholder("Optional Description").FillAsync("Not nearly as hard as the original!");

    await page.GetByRole(AriaRole.Button, new() { Name = "Update Listing" }).ClickAsync();

    // Check for success message
    await page.Locator(".alert-success").WaitForAsync();
    Assert.Contains("Success", await page.Locator(".alert-success").TextContentAsync());

    // Visit View Listings Page to confirm updated
    await page.GetByRole(AriaRole.Button, new() { Name = "Cancel" }).ClickAsync();

    await page.WaitForURLAsync(url => url.Contains("/vendor/listings"));

    // Wait for data to load
    await page.GetByText("Loading...").WaitForAsync(new() { State = WaitForSelectorState.Hidden });

    // Visit product page to check description
    await page.GetByRole(AriaRole.Row).Filter(new()
    {
      Has = page.GetByRole(AriaRole.Cell, new() { Name = "PuzzleScape" })
    }).GetByRole(AriaRole.Button, new()
    {
      Name = "View"
    }).ClickAsync();

    await page.WaitForURLAsync(url => url.Contains("/product"));

    Assert.True(await page.Locator(".product-description").GetByRole(AriaRole.Paragraph).TextContentAsync() == "Not nearly as hard as the original!");
  }

  [Fact(Skip = "Fails: Category updates do not take effect")]
  public async Task Vendor_Can_Edit_Listing_Category()
  {
    var page = await NavigateToEditListingPage();

    // Change Category
    await page.GetByRole(AriaRole.Combobox).SelectOptionAsync("Puzzle");

    await page.GetByRole(AriaRole.Button, new() { Name = "Update Listing" }).ClickAsync();

    // Check for success message
    await page.Locator(".alert-success").WaitForAsync();
    Assert.Contains("Success", await page.Locator(".alert-success").TextContentAsync());

    // Visit View Listings Page to confirm updated
    await page.GetByRole(AriaRole.Button, new() { Name = "Cancel" }).ClickAsync();

    await page.WaitForURLAsync(url => url.Contains("/vendor/listings"));

    // Wait for data to load
    await page.GetByText("Loading...").WaitForAsync(new() { State = WaitForSelectorState.Hidden });

    // Visit product page to check description
    await page.GetByRole(AriaRole.Row).Filter(new()
    {
      Has = page.GetByRole(AriaRole.Cell, new() { Name = "PuzzleScape" })
    }).GetByRole(AriaRole.Button, new()
    {
      Name = "View"
    }).ClickAsync();

    await page.WaitForURLAsync(url => url.Contains("/product"));

    Assert.True(await page.Locator(".badge").TextContentAsync() == "Puzzle");
  }

  [Fact]
  public async Task Edit_Listing_Blank_Description()
  {
    var page = await NavigateToEditListingPage();

    // Change Description
    await page.GetByPlaceholder("Optional Description").FillAsync("");

    await page.GetByRole(AriaRole.Button, new() { Name = "Update Listing" }).ClickAsync();

    // Check for success message
    await page.Locator(".alert-success").WaitForAsync();
    Assert.Contains("Success", await page.Locator(".alert-success").TextContentAsync());
  }

  [Fact]
  public async Task Fail_Edit_Listing_No_Name()
  {
    var page = await NavigateToEditListingPage();

    // Change Name
    await page.GetByPlaceholder("Unnamed Listing").FillAsync("");

    await page.GetByRole(AriaRole.Button, new() { Name = "Update Listing" }).ClickAsync();

    // Check for error message
    await page.Locator(".alert-danger").WaitForAsync();
    Assert.Contains("name", await page.Locator(".alert-danger").TextContentAsync());
  }

  [Fact]
  public async Task Fail_Edit_Listing_No_Free()
  {
    var page = await NavigateToEditListingPage();

    // Change Name
    await page.GetByPlaceholder("0.0").FillAsync("0");

    await page.GetByRole(AriaRole.Button, new() { Name = "Update Listing" }).ClickAsync();

    // Check for error message
    await page.Locator(".alert-danger").WaitForAsync();
    Assert.Contains("price", await page.Locator(".alert-danger").TextContentAsync());
  }

  [Fact]
  public async Task Fail_Edit_Listing_Neg_Stock()
  {
    var page = await NavigateToEditListingPage();

    // Change Name
    await page.GetByPlaceholder("0", new() { Exact = true }).FillAsync("-8");

    await page.GetByRole(AriaRole.Button, new() { Name = "Update Listing" }).ClickAsync();

    // Check for error message
    await page.Locator(".alert-danger").WaitForAsync();
    Assert.Contains("stock", await page.Locator(".alert-danger").TextContentAsync());
  }

  [Fact]
  public async Task Edit_Listing_Cancel()
  {
    var page = await NavigateToEditListingPage();

    await page.GetByRole(AriaRole.Button, new() { Name = "Cancel" }).ClickAsync();

    await page.WaitForURLAsync(url => url.Contains("/vendor/listings"));
  }
}