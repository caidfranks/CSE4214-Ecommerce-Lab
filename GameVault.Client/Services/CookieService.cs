using Microsoft.JSInterop;

namespace GameVault.Client.Services;

public class CookieService
{
    private readonly IJSRuntime _js;

    public CookieService(IJSRuntime js)
    {
        _js = js;
    }

    public async Task SetCookieAsync(string name, string value, int expirationDays = 30)
    {
        await _js.InvokeVoidAsync("cookieHelper.setCookie", name, value, expirationDays);
    }

    public async Task<string?> GetCookieAsync(string name)
    {
        return await _js.InvokeAsync<string?>("cookieHelper.getCookie", name);
    }

    public async Task DeleteCookieAsync(string name)
    {
        await _js.InvokeVoidAsync("cookieHelper.deleteCookie", name);
    }
}