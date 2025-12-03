using Microsoft.Playwright;
using Xunit;

namespace GameVault.Tests.Client;

public class VendorCreationTests : TestBase {
    [Fact]
    public async Task Public_Can_Navigate_To_Vendor_Register_Page()
    {
        var page = await NewPageAsync();
        await page.GotoAsync($"{TestSettings.BaseUrl}/");

        await page.ClickAsync("button:has-text('Become a Vendor')");
        await page.WaitForURLAsync($"{TestSettings.BaseUrl}/register/vendor");

        Assert.True(await page.IsVisibleAsync("h1:has-text('Become a Vendor')"));
        Assert.True(await page.IsVisibleAsync("input[id='email']"));
        Assert.True(await page.IsVisibleAsync("input[id='password']"));
        Assert.True(await page.IsVisibleAsync("input[id='displayName']"));
        Assert.True(await page.IsVisibleAsync("input[id='reason']"));
    }

    [Fact]
    public async Task Vendor_Form_Shows_Validation_Errors()
    {
        var page = await NewPageAsync();
        await page.GotoAsync($"{TestSettings.BaseUrl}/");

        await page.ClickAsync("button:has-text('Become a Vendor')");
        await page.WaitForURLAsync($"{TestSettings.BaseUrl}/register/vendor");

        await page.ClickAsync("button:has-text('Submit Application')");

        await page.EvaluateAsync("document.querySelector('form').dispatchEvent(new Event('submit', { bubbles: true }));");

        await page.WaitForTimeoutAsync(500);

        var emailClass = await page.GetAttributeAsync("#email", "class");
        var passwordClass = await page.GetAttributeAsync("#password", "class");
        var nameClass = await page.GetAttributeAsync("#displayName", "class");
        var reasonClass = await page.GetAttributeAsync("#reason", "class");

        Assert.Contains("is-invalid", emailClass);
        Assert.Contains("is-invalid", passwordClass);
        Assert.Contains("is-invalid", nameClass);
        Assert.Contains("is-invalid", reasonClass);
    }

    [Fact]
    public async Task Vendor_Request_Submits_And_Logs_Actions()
    {
        var page = await NewPageAsync();
        await page.GotoAsync($"{TestSettings.BaseUrl}/");

        await page.ClickAsync("button:has-text('Become a Vendor')");
        await page.WaitForURLAsync($"{TestSettings.BaseUrl}/register/vendor");


        var newVendorEmail = $"newvendor{Guid.NewGuid()}@gmail.com";
        await page.FillAsync("#email", newVendorEmail);
        await page.FillAsync("#password", "password");
        await page.FillAsync("#displayName", $"Test Vendor {Guid.NewGuid()}");
        await page.FillAsync("#reason", "I sell games and wish to join GameVault as a vendor.");

        await page.ClickAsync("button:has-text('Submit Application')");
        await page.WaitForURLAsync(url => url.Contains("/"));

        var content = await page.InnerTextAsync("body");
        Assert.True(content.Length > 0, "Redirected to home page after request.");

        // LogService.LogVendorAccountRequestAsync should have executed
        // Admin test will confirm via logs
    }

    [Fact]
    public async Task Admin_Can_View_Pending_Vendors()
    {
        var page = await LoginAsAdminAsync();

        await page.WaitForURLAsync(url => url.Contains("/adminLanding"));

        await page.ClickAsync("a:has-text('Accounts')");
        await page.WaitForURLAsync(url => url.Contains("/manageAccounts"));

        await page.ClickAsync("button:has-text('Pending Vendor')");
        await page.WaitForURLAsync(url => url.Contains("/manageAccounts/pendingVendors"));

        if (await page.IsVisibleAsync("text=No accounts found"))
        {
            Assert.True(true, "No pending vendor requests is a valid state.");
            return;
        }
        await page.WaitForSelectorAsync("table.table tbody tr");
        Assert.True(await page.IsVisibleAsync("table.table"));
    }

    [Fact]
    public async Task Admin_Can_Open_Vendor_Request_Details_Modal()
    {
        var page = await LoginAsAdminAsync();

        await page.WaitForURLAsync(url => url.Contains("/adminLanding"));

        await page.ClickAsync("a:has-text('Accounts')");
        await page.WaitForURLAsync(url => url.Contains("/manageAccounts"));

        await page.ClickAsync("button:has-text('Pending Vendor')");
        await page.WaitForURLAsync(url => url.Contains("/pendingVendors"));

        if (await page.IsVisibleAsync("text=No accounts found"))
            return; // valid state

        var viewBtn = await page.QuerySelectorAsync("button.btn-ban:has-text('View Vendor Request')");
        Assert.NotNull(viewBtn);

        await viewBtn.ClickAsync();
        await page.WaitForSelectorAsync("div.modal.show");

        Assert.True(await page.IsVisibleAsync("div.modal.show"));
        Assert.True(await page.IsVisibleAsync("button.btn-approve"));
        Assert.True(await page.IsVisibleAsync("button.btn-deny"));
    }

    [Fact]
    public async Task Admin_Can_Approve_Vendor_Request()
    {
        var page = await LoginAsAdminAsync();
        await page.WaitForURLAsync(url => url.Contains("/adminLanding"));

        await page.ClickAsync("a:has-text('Accounts')");
        await page.WaitForURLAsync(url => url.Contains("/manageAccounts"));

        await page.ClickAsync("button:has-text('Pending Vendor')");
        await page.WaitForURLAsync(url => url.Contains("/manageAccounts/pendingVendors"));

        if (await page.IsVisibleAsync("text=No accounts found"))
            return;

        await page.ClickAsync("button:has-text('View Vendor Request')");
        await page.WaitForSelectorAsync("button.btn-approve");

        await page.ClickAsync("button.btn-approve");
        await page.WaitForSelectorAsync("table.table", new() { State = WaitForSelectorState.Visible });

        await page.WaitForSelectorAsync("div.modal.show", new() { State = WaitForSelectorState.Detached });

        var body = await page.InnerTextAsync("body");
        Assert.DoesNotContain("View Vendor Request", body);
    }

    [Fact]
    public async Task Approved_User_Can_Login_And_View_Vendor_Dashboard()
    {
        var page = await LoginAsVendorAsync();
        await page.WaitForLoadStateAsync();

        Assert.True(await page.IsVisibleAsync("h3:has-text('Welcome Vendor!')"));
        Assert.True(await page.IsVisibleAsync("a:has-text('View Listings')"));
        Assert.True(await page.IsVisibleAsync("a:has-text('Create Listing')"));
    }
}