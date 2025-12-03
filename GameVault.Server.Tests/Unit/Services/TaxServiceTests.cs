using GameVault.Server.Services;
using Xunit;

namespace GameVault.Server.Tests.Unit.Services;

public class TaxServiceTests
{
    private readonly TaxService _taxService;

    public TaxServiceTests()
    {
        _taxService = new TaxService();
    }

    [Fact]
    public void CalculateTax_WithValidState_ReturnsCorrectTax()
    {
        int subtotalInCents = 10000;
        string state = "TX";

        int tax = _taxService.CalculateTax(subtotalInCents, state);

        Assert.Equal(625, tax);
    }

    [Fact]
    public void CalculateTax_WithZeroTaxState_ReturnsZero()
    {
        int subtotalInCents = 10000;
        string state = "OR";

        int tax = _taxService.CalculateTax(subtotalInCents, state);

        Assert.Equal(0, tax);
    }

    [Fact]
    public void CalculateTax_WithNullState_ReturnsZero()
    {
        int subtotalInCents = 10000;
        string? state = null;

        int tax = _taxService.CalculateTax(subtotalInCents, state!);

        Assert.Equal(0, tax);
    }

    [Fact]
    public void CalculateTax_WithZeroSubtotal_ReturnsZero()
    {
        int subtotalInCents = 0;
        string state = "TX";

        int tax = _taxService.CalculateTax(subtotalInCents, state);

        Assert.Equal(0, tax);
    }

    [Fact]
    public void GetTaxRate_WithInvalidState_ReturnsZero()
    {
        string state = "XX";

        decimal rate = _taxService.GetTaxRate(state);

        Assert.Equal(0m, rate);
    }
}
