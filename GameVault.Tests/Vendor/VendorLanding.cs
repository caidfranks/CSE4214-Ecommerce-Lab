using Microsoft.Playwright;
using Xunit;

using GameVault.Tests.Client;

namespace GameVault.Tests.Vendor;

public class VendorLandingTests : VendorBase
{
  [Fact]
  public async Task Vendor_Can_Navigate_To_Vendor_Landing_Page()
  {
    var page = await NavigateToVendorHome();

    Assert.Contains("/vendor", page.Url);
  }

  [Fact]
  public async Task Vendor_Has_Links_On_Homepage()
  {
    var page = await NavigateToVendorHome();

    Assert.Contains("/vendor", page.Url);

    // Wait for navbar to load
    await page.GetByText("Logout").WaitForAsync();

    Assert.Equal(2, await page.GetByText("Listings").CountAsync());
    Assert.Equal(2, await page.GetByText("Create").CountAsync());
    Assert.Equal(2, await page.GetByText("Orders").CountAsync());

    // Wait for load
    await page.GetByText("Loading...").WaitForAsync(new() { State = WaitForSelectorState.Hidden });

    Assert.True(await page.GetByText("Account Balance").IsVisibleAsync());
  }
}