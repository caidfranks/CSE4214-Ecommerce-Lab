using Microsoft.Playwright;
using NUnit.Framework.Internal;
using Xunit;

namespace GameVault.Tests.Client;

public class OrderHistoryTests : TestBase
{
  private async Task<IPage> NavigateToOrderHistoryPage()
  {
    var page = await LoginAsCustomerAsync();

    await page.WaitForURLAsync(url => url.Contains("/games"));

    await page.GetByRole(AriaRole.Link, new() { Name = "Order History" }).ClickAsync();
    await page.WaitForURLAsync(url => url.Contains("/orders"));

    return page;
  }

  [Fact]
  public async Task Customer_Can_Navigate_To_Order_History_Page()
  {
    var page = await NavigateToOrderHistoryPage();

    Assert.Contains("/orders", page.Url);
  }

  [Fact]
  public async Task Customer_Can_Cancel_Order()
  {
    var page = await NavigateToOrderHistoryPage();

    Assert.Contains("/orders", page.Url);

    // Wait for data to load
    await page.GetByText("Loading...").WaitForAsync(new() { State = WaitForSelectorState.Hidden });

    // Open order details dialog
    await page.GetByRole(AriaRole.Row).Filter(new()
    {
      Has = page.GetByRole(AriaRole.Cell, new() { Name = "rEK6AjUw5kAvtQe4JHYn" })
    }).GetByRole(AriaRole.Button, new()
    {
      Name = "Details"
    }).ClickAsync();

    // Wait for data to load
    await page.GetByText("Loading...").WaitForAsync(new() { State = WaitForSelectorState.Hidden });

    // Cancel pending invoice
    await page.GetByRole(AriaRole.Row).Filter(new()
    {
      Has = page.GetByRole(AriaRole.Cell, new() { Name = "OAcNw1YWlTYfMYAN6u2r" })
    }).GetByRole(AriaRole.Button, new()
    {
      Name = "Cancel"
    }).ClickAsync();

    // Ensure cancelled
    await page.GetByRole(AriaRole.Row).Filter(new()
    {
      Has = page.GetByRole(AriaRole.Cell, new() { Name = "OAcNw1YWlTYfMYAN6u2r" })
    }).GetByRole(AriaRole.Cell, new()
    {
      Name = "Cancelled"
    }).WaitForAsync();
  }

  [Fact]
  public async Task Customer_Can_Start_And_Cancel_Return()
  {
    var page = await NavigateToOrderHistoryPage();

    Assert.Contains("/orders", page.Url);

    // Wait for data to load
    await page.GetByText("Loading...").WaitForAsync(new() { State = WaitForSelectorState.Hidden });

    // Open order details dialog
    await page.GetByRole(AriaRole.Row).Filter(new()
    {
      Has = page.GetByRole(AriaRole.Cell, new() { Name = "rEK6AjUw5kAvtQe4JHYn" })
    }).GetByRole(AriaRole.Button, new()
    {
      Name = "Details"
    }).ClickAsync();

    // Wait for data to load
    await page.GetByText("Loading...").WaitForAsync(new() { State = WaitForSelectorState.Hidden });

    // Open return prompt
    await page.GetByRole(AriaRole.Row).Filter(new()
    {
      Has = page.GetByRole(AriaRole.Cell, new() { Name = "r5dleV36rYfeUPjd888R" })
    }).GetByRole(AriaRole.Button, new()
    {
      Name = "Start Return"
    }).ClickAsync();

    // Enter reason
    await page.GetByPlaceholder("Enter reason").FillAsync("Bad product");

    await page.Locator("button.btn-approve:text(\"Start Return\")").ClickAsync();

    // Ensure pending
    await page.GetByRole(AriaRole.Row).Filter(new()
    {
      Has = page.GetByRole(AriaRole.Cell, new() { Name = "r5dleV36rYfeUPjd888R" })
    }).GetByRole(AriaRole.Cell, new()
    {
      Name = "PendingReturn"
    }).WaitForAsync();

    // Make sure reason showed up
    Assert.True(await page.GetByRole(AriaRole.Row).Filter(new()
    {
      Has = page.GetByRole(AriaRole.Cell, new() { Name = "r5dleV36rYfeUPjd888R" })
    }).GetByRole(AriaRole.Cell, new()
    {
      Name = "Bad product"
    }).IsVisibleAsync());

    // Cancel return
    await page.GetByRole(AriaRole.Row).Filter(new()
    {
      Has = page.GetByRole(AriaRole.Cell, new() { Name = "r5dleV36rYfeUPjd888R" })
    }).GetByRole(AriaRole.Button, new()
    {
      Name = "Cancel"
    }).ClickAsync();

    // Ensure cancelled
    await page.GetByRole(AriaRole.Row).Filter(new()
    {
      Has = page.GetByRole(AriaRole.Cell, new() { Name = "r5dleV36rYfeUPjd888R" })
    }).GetByRole(AriaRole.Cell, new()
    {
      Name = "Completed"
    }).WaitForAsync();
  }

  [Fact(Skip = "Fails: Return prompt does not require a reason")]
  public async Task Fail_No_Return_Reason()
  {
    var page = await NavigateToOrderHistoryPage();

    Assert.Contains("/orders", page.Url);

    // Wait for data to load
    await page.GetByText("Loading...").WaitForAsync(new() { State = WaitForSelectorState.Hidden });

    // Open order details dialog
    await page.GetByRole(AriaRole.Row).Filter(new()
    {
      Has = page.GetByRole(AriaRole.Cell, new() { Name = "rEK6AjUw5kAvtQe4JHYn" })
    }).GetByRole(AriaRole.Button, new()
    {
      Name = "Details"
    }).ClickAsync();

    // Wait for data to load
    await page.GetByText("Loading...").WaitForAsync(new() { State = WaitForSelectorState.Hidden });

    // Cancel pending invoice
    await page.GetByRole(AriaRole.Row).Filter(new()
    {
      Has = page.GetByRole(AriaRole.Cell, new() { Name = "r5dleV36rYfeUPjd888R" })
    }).GetByRole(AriaRole.Button, new()
    {
      Name = "Start Return"
    }).ClickAsync();

    // Enter blank reason
    await page.GetByPlaceholder("Enter reason").FillAsync("");

    await page.Locator("button.btn-approve:text(\"Start Return\")").ClickAsync();

    // TODO: Should Fail but succeeds right now
  }
}