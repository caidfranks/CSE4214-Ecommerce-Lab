using Microsoft.Playwright;
using Xunit;

using GameVault.Tests.Client;

namespace GameVault.Tests.Vendor;

public class VendorOrdersTests : VendorBase
{
  protected async Task<IPage> NavigateToVendorViewOrdersPage()
  {
    var page = await NavigateToVendorHome();

    await page.GetByRole(AriaRole.Link, new() { Name = "View Orders" }).First.ClickAsync();

    await page.WaitForURLAsync(url => url.Contains("/vendor/orders"));

    return page;
  }

  [Fact]
  public async Task Vendor_Can_Navigate_To_Vendor_Orders_Page()
  {
    var page = await NavigateToVendorViewOrdersPage();

    Assert.Contains("/vendor/orders", page.Url);
  }

  [Fact]
  public async Task Vendor_Can_Accept_Order()
  {
    var page = await NavigateToVendorViewOrdersPage();

    Assert.Contains("/vendor/orders", page.Url);

    // Wait for data to load
    await page.GetByText("Loading...").WaitForAsync(new() { State = WaitForSelectorState.Hidden });

    // Accept order
    await page.GetByRole(AriaRole.Row).Filter(new()
    {
      Has = page.GetByRole(AriaRole.Cell, new() { Name = "fIQklgoFiNn8Rqa60bOG" })
    }).GetByRole(AriaRole.Button, new()
    {
      Name = "Accept"
    }).ClickAsync();

    // Delay to let server process
    await page.WaitForTimeoutAsync(100);

    // Check orders awaiting shipment
    await page.GetByRole(AriaRole.Button, new() { Name = "AwaitingShipment" }).ClickAsync();

    // Wait for data to load
    await page.GetByText("Loading...").WaitForAsync(new() { State = WaitForSelectorState.Hidden });

    // Ensure now exists
    await page.GetByText("fIQklgoFiNn8Rqa60bOG").WaitForAsync();
  }


  [Fact]
  public async Task Vendor_Can_Ship_Order()
  {
    var page = await NavigateToVendorViewOrdersPage();

    Assert.Contains("/vendor/orders", page.Url);

    // Check orders awaiting shipment
    await page.GetByRole(AriaRole.Button, new() { Name = "AwaitingShipment" }).ClickAsync();

    // Wait for data to load
    await page.GetByText("Loading...").WaitForAsync(new() { State = WaitForSelectorState.Hidden });

    // Accept order
    await page.GetByRole(AriaRole.Row).Filter(new()
    {
      Has = page.GetByRole(AriaRole.Cell, new() { Name = "F6TybjleF0i1aLttWNm3" })
    }).GetByRole(AriaRole.Button, new()
    {
      Name = "Shipped"
    }).ClickAsync();

    // Delay to let server process
    await page.WaitForTimeoutAsync(100);

    // Check orders awaiting shipment
    await page.Locator(".btn-secondary:text(\"Shipped\")").ClickAsync();

    // Wait for data to load
    await page.GetByText("Loading...").WaitForAsync(new() { State = WaitForSelectorState.Hidden });

    // Ensure now exists
    await page.GetByText("F6TybjleF0i1aLttWNm3").WaitForAsync();
  }

  [Fact]
  public async Task Vendor_Can_Complete_Order()
  {
    var page = await NavigateToVendorViewOrdersPage();

    Assert.Contains("/vendor/orders", page.Url);

    // Check orders awaiting shipment
    await page.GetByRole(AriaRole.Button, new() { Name = "Shipped" }).ClickAsync();

    // Wait for data to load
    await page.GetByText("Loading...").WaitForAsync(new() { State = WaitForSelectorState.Hidden });

    // Accept order
    await page.GetByRole(AriaRole.Row).Filter(new()
    {
      Has = page.GetByRole(AriaRole.Cell, new() { Name = "dICRROkXc9aVs3uFyJAQ" })
    }).GetByRole(AriaRole.Button, new()
    {
      Name = "Arrived"
    }).ClickAsync();

    // Delay to let server process
    await page.WaitForTimeoutAsync(100);

    // Check orders awaiting shipment
    await page.GetByRole(AriaRole.Button, new() { Name = "Completed" }).ClickAsync();

    // Wait for data to load
    await page.GetByText("Loading...").WaitForAsync(new() { State = WaitForSelectorState.Hidden });

    // Ensure now exists
    await page.GetByText("dICRROkXc9aVs3uFyJAQ").WaitForAsync();
  }

  [Fact]
  public async Task Vendor_Can_Accept_Return()
  {
    var page = await NavigateToVendorViewOrdersPage();

    Assert.Contains("/vendor/orders", page.Url);

    // Check orders awaiting shipment
    await page.GetByRole(AriaRole.Button, new() { Name = "PendingReturn" }).ClickAsync();

    // Wait for data to load
    await page.GetByText("Loading...").WaitForAsync(new() { State = WaitForSelectorState.Hidden });

    // Accept order
    await page.GetByRole(AriaRole.Row).Filter(new()
    {
      Has = page.GetByRole(AriaRole.Cell, new() { Name = "j7T251nHVev7Xy9pRUGX" })
    }).GetByRole(AriaRole.Button, new()
    {
      Name = "Accept"
    }).ClickAsync();

    // Delay to let server process
    await page.WaitForTimeoutAsync(100);

    // Check orders awaiting shipment
    await page.GetByRole(AriaRole.Button, new() { Name = "AwaitingReturn" }).ClickAsync();

    // Wait for data to load
    await page.GetByText("Loading...").WaitForAsync(new() { State = WaitForSelectorState.Hidden });

    // Ensure now exists
    await page.GetByText("j7T251nHVev7Xy9pRUGX").WaitForAsync();
  }

  [Fact]
  public async Task Vendor_Can_Decline_Accepted_Order()
  {
    var page = await NavigateToVendorViewOrdersPage();

    Assert.Contains("/vendor/orders", page.Url);

    // Check orders awaiting shipment
    await page.GetByRole(AriaRole.Button, new() { Name = "AwaitingShipment" }).ClickAsync();

    // Wait for data to load
    await page.GetByText("Loading...").WaitForAsync(new() { State = WaitForSelectorState.Hidden });

    // Accept order
    await page.GetByRole(AriaRole.Row).Filter(new()
    {
      Has = page.GetByRole(AriaRole.Cell, new() { Name = "LzQGjIn1ztBoZYsi3ypc" })
    }).GetByRole(AriaRole.Button, new()
    {
      Name = "Decline"
    }).ClickAsync();

    // Enter reason
    await page.GetByPlaceholder("Enter reason").FillAsync("Import restrictions");

    await page.Locator("button.btn-approve:text(\"Decline\")").ClickAsync();

    // Delay to let server process
    await page.WaitForTimeoutAsync(100);

    // Check orders awaiting shipment
    await page.GetByRole(AriaRole.Button, new() { Name = "Declined" }).ClickAsync();

    // Wait for data to load
    await page.GetByText("Loading...").WaitForAsync(new() { State = WaitForSelectorState.Hidden });

    // Ensure now exists
    await page.GetByText("LzQGjIn1ztBoZYsi3ypc").WaitForAsync();

    // Make sure reason showed up
    Assert.True(await page.GetByRole(AriaRole.Row).Filter(new()
    {
      Has = page.GetByRole(AriaRole.Cell, new() { Name = "LzQGjIn1ztBoZYsi3ypc" })
    }).GetByRole(AriaRole.Cell, new()
    {
      Name = "Import restrictions"
    }).IsVisibleAsync());
  }

  [Fact]
  public async Task Vendor_Can_Decline_Return()
  {
    var page = await NavigateToVendorViewOrdersPage();

    Assert.Contains("/vendor/orders", page.Url);

    // Check orders awaiting shipment
    await page.GetByRole(AriaRole.Button, new() { Name = "AwaitingReturn" }).ClickAsync();

    // Wait for data to load
    await page.GetByText("Loading...").WaitForAsync(new() { State = WaitForSelectorState.Hidden });

    // Accept order
    await page.GetByRole(AriaRole.Row).Filter(new()
    {
      Has = page.GetByRole(AriaRole.Cell, new() { Name = "rXM7vaAbI1RbFv6YNhHF" })
    }).GetByRole(AriaRole.Button, new()
    {
      Name = "Failed"
    }).ClickAsync();

    // Enter reason
    await page.GetByPlaceholder("Enter reason").FillAsync("Never received product");

    await page.Locator("button.btn-approve:text(\"Fail\")").ClickAsync();

    // Delay to let server process
    await page.WaitForTimeoutAsync(100);

    // Check orders awaiting shipment
    await page.GetByRole(AriaRole.Button, new() { Name = "PendingReturn" }).ClickAsync();

    // Wait for data to load
    await page.GetByText("Loading...").WaitForAsync(new() { State = WaitForSelectorState.Hidden });

    // Ensure now exists
    await page.GetByText("rXM7vaAbI1RbFv6YNhHF").WaitForAsync();

    // Make sure reason showed up
    Assert.True(await page.GetByRole(AriaRole.Row).Filter(new()
    {
      Has = page.GetByRole(AriaRole.Cell, new() { Name = "rXM7vaAbI1RbFv6YNhHF" })
    }).GetByRole(AriaRole.Cell, new()
    {
      Name = "Never received product"
    }).IsVisibleAsync());
  }

  [Fact]
  public async Task Vendor_Can_Finish_Return()
  {
    var page = await NavigateToVendorViewOrdersPage();

    Assert.Contains("/vendor/orders", page.Url);

    // Check orders awaiting shipment
    await page.GetByRole(AriaRole.Button, new() { Name = "AwaitingReturn" }).ClickAsync();

    // Wait for data to load
    await page.GetByText("Loading...").WaitForAsync(new() { State = WaitForSelectorState.Hidden });

    // Accept order
    await page.GetByRole(AriaRole.Row).Filter(new()
    {
      Has = page.GetByRole(AriaRole.Cell, new() { Name = "fYyyupk0so2Ul44POii3" })
    }).GetByRole(AriaRole.Button, new()
    {
      Name = "Received"
    }).ClickAsync();

    // Delay to let server process
    await page.WaitForTimeoutAsync(100);

    // Check orders awaiting shipment
    await page.GetByRole(AriaRole.Button, new() { Name = "Completed" }).ClickAsync();

    // Wait for data to load
    await page.GetByText("Loading...").WaitForAsync(new() { State = WaitForSelectorState.Hidden });

    // Ensure now exists
    await page.GetByText("fYyyupk0so2Ul44POii3").WaitForAsync();

    // Make sure reason showed up
    Assert.True(await page.GetByRole(AriaRole.Row).Filter(new()
    {
      Has = page.GetByRole(AriaRole.Cell, new() { Name = "fYyyupk0so2Ul44POii3" })
    }).GetByRole(AriaRole.Cell, new()
    {
      Name = "Return Completed!"
    }).IsVisibleAsync());
  }
}