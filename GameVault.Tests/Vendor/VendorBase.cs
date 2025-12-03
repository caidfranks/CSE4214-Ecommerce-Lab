using Microsoft.Playwright;
using Xunit;

using GameVault.Tests.Client;

namespace GameVault.Tests.Vendor;

public class VendorBase : TestBase
{
  protected async Task<IPage> NavigateToVendorHome()
  {
    var page = await LoginAsVendorAsync();

    await page.WaitForURLAsync(url => url.Contains("/vendor"));

    return page;
  }
  protected async Task<IPage> NavigateToVendorViewListingsPage()
  {
    var page = await NavigateToVendorHome();

    await page.GetByRole(AriaRole.Link, new() { Name = "View Listings" }).First.ClickAsync();

    await page.WaitForURLAsync(url => url.Contains("/vendor/listings"));

    return page;
  }
}