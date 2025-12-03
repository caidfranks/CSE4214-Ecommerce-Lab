using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

public static class TestSettings
{
    private static readonly JsonElement _settings;

    static TestSettings()
    {
        var basePath = AppContext.BaseDirectory;
        var jsonPath = Path.Combine(basePath, "TestSettings.json");
        var json = File.ReadAllText(jsonPath);
        _settings = JsonDocument.Parse(json).RootElement;
    }

    public static string BaseUrl => _settings.GetProperty("BaseUrl").GetString()!;

    public static string CustomerEmail => _settings.GetProperty("Users").GetProperty("Customer").GetProperty("Email").GetString()!;
    public static string CustomerPassword => _settings.GetProperty("Users").GetProperty("Customer").GetProperty("Password").GetString()!;

    public static string VendorEmail => _settings.GetProperty("Users").GetProperty("Vendor").GetProperty("Email").GetString()!;
    public static string VendorPassword => _settings.GetProperty("Users").GetProperty("Vendor").GetProperty("Password").GetString()!;

    public static string AdminEmail => _settings.GetProperty("Users").GetProperty("Admin").GetProperty("Email").GetString()!;
    public static string AdminPassword => _settings.GetProperty("Users").GetProperty("Admin").GetProperty("Password").GetString()!;

    public static int Timeout => _settings.GetProperty("Timeouts").GetProperty("PageLoad").GetInt32();
}
