using Microsoft.Playwright;
using Xunit;

using GameVault.Tests.Client;

namespace GameVault.Tests.Vendor;

public class ViewListingsTests : VendorBase
{
    [Fact]
    public async Task Vendor_Can_Navigate_To_Vendor_Listings_Page()
    {
        var page = await NavigateToVendorViewListingsPage();

        Assert.Contains("/vendor/listings", page.Url);
    }
}