using Microsoft.Playwright;
using Xunit;

using GameVault.Tests.Client;

namespace GameVault.Tests.Vendor;

public class CreateListingTests : VendorBase
{
  private async Task<IPage> NavigateToCreateListingPage()
  {
    var page = await NavigateToVendorHome();

    await page.GetByRole(AriaRole.Link, new() { Name = "Create Listing" }).First.ClickAsync();

    await page.WaitForURLAsync(url => url.Contains("/vendor/listings/create"));

    return page;
  }

  [Fact]
  public async Task Vendor_Can_Navigate_To_Create_Listing_Page()
  {
    var page = await NavigateToCreateListingPage();

    Assert.Contains("/vendor/listings/create", page.Url);
  }

  [Fact]
  public async Task Vendor_Can_Create_Listing()
  {
    var page = await NavigateToCreateListingPage();

    // Valid Name, Description, Price, Stock, and Category
    await page.GetByPlaceholder("Unnamed Listing").FillAsync("Test product");
    await page.GetByPlaceholder("Optional Description").FillAsync("Useful for lots of testing and such");
    await page.GetByPlaceholder("0.0").FillAsync("59.67");
    await page.GetByPlaceholder("0", new() { Exact = true }).FillAsync("4");

    await page.GetByRole(AriaRole.Combobox).SelectOptionAsync("FPS");

    await page.GetByRole(AriaRole.Button, new() { Name = "Create Listing" }).ClickAsync();

    // Visit page by visiting View Listings Page and clicking View next to listing
    // await page.GotoAsync("/vendor/listings");

    await page.WaitForURLAsync(url => url.Contains("/vendor/listings"));

    // Select Test Product
    await page.GetByRole(AriaRole.Row).Filter(new()
    {
      Has = page.GetByRole(AriaRole.Cell, new() { Name = "Test product", Exact = true })
    }).GetByRole(AriaRole.Button, new()
    {
      Name = "View"
    }).ClickAsync();

    await page.WaitForURLAsync(url => url.Contains("/product"));

    // Make sure data filled in
    Assert.True(await page.Locator(".product-title").TextContentAsync() == "Test product");
    Assert.True(await page.Locator(".product-price").TextContentAsync() == "$59.67");
    Assert.True(await page.Locator(".product-description").GetByRole(AriaRole.Paragraph).TextContentAsync() == "Useful for lots of testing and such");
    Assert.True(await page.Locator(".badge").TextContentAsync() == "FPS");
  }


  [Fact]
  public async Task Create_Listing_Blank_Desc()
  {
    var page = await NavigateToCreateListingPage();

    // Valid Name, Description, Price, Stock, and Category
    await page.GetByPlaceholder("Unnamed Listing").FillAsync("Test Product B");
    await page.GetByPlaceholder("Optional Description").FillAsync("");
    await page.GetByPlaceholder("0.0").FillAsync("12.34");
    await page.GetByPlaceholder("0", new() { Exact = true }).FillAsync("4");

    await page.GetByRole(AriaRole.Combobox).SelectOptionAsync("FPS");

    await page.GetByRole(AriaRole.Button, new() { Name = "Create Listing" }).ClickAsync();

    // Visit page by visiting View Listings Page and clicking View next to listing
    // await page.GotoAsync("/vendor/listings");

    await page.WaitForURLAsync(url => url.Contains("/vendor/listings"));

    // Select Test Product
    await page.GetByRole(AriaRole.Row).Filter(new()
    {
      Has = page.GetByRole(AriaRole.Cell, new() { Name = "Test Product B", Exact = true })
    }).GetByRole(AriaRole.Button, new()
    {
      Name = "View"
    }).ClickAsync();

    await page.WaitForURLAsync(url => url.Contains("/product"));

    // Make sure data filled in
    Assert.True(await page.Locator(".product-title").TextContentAsync() == "Test Product B");
    Assert.True(await page.Locator(".product-price").TextContentAsync() == "$12.34");
    Assert.True(await page.Locator(".product-description").GetByRole(AriaRole.Paragraph).TextContentAsync() == "");
    Assert.True(await page.Locator(".badge").TextContentAsync() == "FPS");
  }


  [Fact]
  public async Task Create_Listing_No_Cat()
  {
    var page = await NavigateToCreateListingPage();

    // Valid Name, Description, Price, Stock, and Category
    await page.GetByPlaceholder("Unnamed Listing").FillAsync("Test Product C");
    await page.GetByPlaceholder("Optional Description").FillAsync("");
    await page.GetByPlaceholder("0.0").FillAsync("45.67");
    await page.GetByPlaceholder("0", new() { Exact = true }).FillAsync("8");

    // await page.GetByRole(AriaRole.Combobox).SelectOptionAsync("FPS");

    await page.GetByRole(AriaRole.Button, new() { Name = "Create Listing" }).ClickAsync();

    // Visit page by visiting View Listings Page and clicking View next to listing
    // await page.GotoAsync("/vendor/listings");

    await page.WaitForURLAsync(url => url.Contains("/vendor/listings"));

    // Select Test Product
    await page.GetByRole(AriaRole.Row).Filter(new()
    {
      Has = page.GetByRole(AriaRole.Cell, new() { Name = "Test Product C", Exact = true })
    }).GetByRole(AriaRole.Button, new()
    {
      Name = "View"
    }).ClickAsync();

    await page.WaitForURLAsync(url => url.Contains("/product"));

    // Make sure data filled in
    Assert.True(await page.Locator(".product-title").TextContentAsync() == "Test Product C");
    Assert.True(await page.Locator(".product-price").TextContentAsync() == "$45.67");
    Assert.True(await page.Locator(".product-description").GetByRole(AriaRole.Paragraph).TextContentAsync() == "");
    Assert.True(await page.Locator(".badge").TextContentAsync() == "Other");
  }

  [Fact]
  public async Task Fail_Create_Listing_No_Name()
  {
    var page = await NavigateToCreateListingPage();

    // Valid Name, Description, Price, Stock, and Category
    await page.GetByPlaceholder("Unnamed Listing").FillAsync("");
    await page.GetByPlaceholder("Optional Description").FillAsync("");
    await page.GetByPlaceholder("0.0").FillAsync("45.67");
    await page.GetByPlaceholder("0", new() { Exact = true }).FillAsync("8");

    // await page.GetByRole(AriaRole.Combobox).SelectOptionAsync("FPS");

    await page.GetByRole(AriaRole.Button, new() { Name = "Create Listing" }).ClickAsync();

    // Ensure error message appeared
    await page.Locator(".alert-danger").WaitForAsync();
    Assert.Contains("name", await page.Locator(".alert-danger").TextContentAsync());
  }


  [Fact]
  public async Task Fail_Create_Listing_Free()
  {
    var page = await NavigateToCreateListingPage();

    // Valid Name, Description, Price, Stock, and Category
    await page.GetByPlaceholder("Unnamed Listing").FillAsync("Free Product");
    await page.GetByPlaceholder("Optional Description").FillAsync("");
    await page.GetByPlaceholder("0.0").FillAsync("0");
    await page.GetByPlaceholder("0", new() { Exact = true }).FillAsync("8");

    // await page.GetByRole(AriaRole.Combobox).SelectOptionAsync("FPS");

    await page.GetByRole(AriaRole.Button, new() { Name = "Create Listing" }).ClickAsync();

    // Ensure error message appeared
    await page.Locator(".alert-danger").WaitForAsync();
    Assert.Contains("price", await page.Locator(".alert-danger").TextContentAsync());
  }


  [Fact]
  public async Task Fail_Create_Listing_Neg_Stock()
  {
    var page = await NavigateToCreateListingPage();

    // Valid Name, Description, Price, Stock, and Category
    await page.GetByPlaceholder("Unnamed Listing").FillAsync("Infinite Money");
    await page.GetByPlaceholder("Optional Description").FillAsync("");
    await page.GetByPlaceholder("0.0").FillAsync("45.67");
    await page.GetByPlaceholder("0", new() { Exact = true }).FillAsync("-8");

    // await page.GetByRole(AriaRole.Combobox).SelectOptionAsync("FPS");

    await page.GetByRole(AriaRole.Button, new() { Name = "Create Listing" }).ClickAsync();

    // Ensure error message appeared
    await page.Locator(".alert-danger").WaitForAsync();
    Assert.Contains("stock", await page.Locator(".alert-danger").TextContentAsync());
  }
  [Fact]
  public async Task Create_Listing_Cancel()
  {
    var page = await NavigateToCreateListingPage();

    await page.GetByRole(AriaRole.Button, new() { Name = "Cancel" }).ClickAsync();

    await page.WaitForURLAsync(url => url.Contains("/vendor/listings"));
  }
}