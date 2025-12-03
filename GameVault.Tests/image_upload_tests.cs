using Microsoft.Playwright;
using Xunit;
using System.IO;
using System.Text.RegularExpressions;
using GameVault.Tests.Client;

namespace GameVault.Tests.Components;

public class ImageUploadTests : TestBase
{
    private const string ClientBaseUrl = "http://localhost:5166";
    private const string LoginUrl = $"{ClientBaseUrl}/login";
    private const string CreateListingUrl = $"{ClientBaseUrl}/vendor/listings/create";

    private const string VendorEmail = "testvendor@example.com";
    private const string VendorPassword = "Password123!";

    private string _tmpPath;

    private async Task Login()
    {
        await Page.GotoAsync(LoginUrl);
        await Page.Locator("#email").FillAsync(VendorEmail);
        await Page.Locator("#password").FillAsync(VendorPassword);
        await Page.Locator("button[type='submit']").ClickAsync();
        await Page.WaitForURLAsync(new Regex("^((?!login).)*$"));
    }

    private async Task Navigate() => await Page.GotoAsync(CreateListingUrl);

    [Fact]
    public async Task ComponentRenders()
    {
        await Login();
        await Navigate();
        Assert.True(await Page.Locator(".image-upload-container").IsVisibleAsync());
    }

    [Fact]
    public async Task FileInputAcceptsImages()
    {
        await Login();
        await Navigate();

        var input = Page.Locator("input[type='file']");
        var accept = await input.GetAttributeAsync("accept");

        Assert.Contains("image", accept);
    }

    [Fact]
    public async Task RejectsLargeFiles()
    {
        await Login();
        await Navigate();

        var input = Page.Locator("input[type='file']");
        await input.SetInputFilesAsync(Path.Combine(_tmpPath, "large.jpg"));

        Assert.True(await Page.Locator(".alert-danger").IsVisibleAsync());
    }

    [Fact]
    public async Task RejectsInvalidFileTypes()
    {
        await Login();
        await Navigate();

        var input = Page.Locator("input[type='file']");
        await input.SetInputFilesAsync(Path.Combine(_tmpPath, "invalid.txt"));

        Assert.True(await Page.Locator(".alert-danger").IsVisibleAsync());
    }

    [Fact]
    public async Task AcceptsValidImage()
    {
        await Login();
        await Navigate();

        var input = Page.Locator("input[type='file']");
        await input.SetInputFilesAsync(Path.Combine(_tmpPath, "valid.jpg"));

        await Task.Delay(500);
        Assert.True(await Page.Locator("img").CountAsync() > 0);
    }
}
