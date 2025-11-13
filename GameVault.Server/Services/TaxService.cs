using Microsoft.VisualBasic;

namespace GameVault.Server.Services;

public class TaxService
{
    private static readonly Dictionary<string, decimal> StateTaxRates = new()
    {
        { "AL", 0.04m },
        { "AK", 0.00m },
        { "AZ", 0.056m },
        { "AR", 0.065m },
        { "CA", 0.0725m },
        { "CO", 0.029m },
        { "CT", 0.0635m },
        { "DE", 0.00m },
        { "FL", 0.06m },
        { "GA", 0.04m },
        { "HI", 0.04m },
        { "ID", 0.06m },
        { "IL", 0.0625m },
        { "IN", 0.07m },
        { "IA", 0.06m },
        { "KS", 0.065m },
        { "KY", 0.06m },
        { "LA", 0.0445m },
        { "ME", 0.055m },
        { "MD", 0.06m },
        { "MA", 0.0625m },
        { "MI", 0.06m },
        { "MN", 0.06875m },
        { "MS", 0.07m },
        { "MO", 0.04225m },
        { "MT", 0.00m },
        { "NE", 0.055m },
        { "NV", 0.0685m },
        { "NH", 0.00m },
        { "NJ", 0.06625m },
        { "NM", 0.05125m },
        { "NY", 0.04m },
        { "NC", 0.0475m },
        { "ND", 0.05m },
        { "OH", 0.0575m },
        { "OK", 0.045m },
        { "OR", 0.00m },
        { "PA", 0.06m },
        { "RI", 0.07m },
        { "SC", 0.06m },
        { "SD", 0.045m },
        { "TN", 0.07m },
        { "TX", 0.0625m },
        { "UT", 0.0485m },
        { "VT", 0.06m },
        { "VA", 0.053m },
        { "WA", 0.065m },
        { "WV", 0.06m },
        { "WI", 0.05m },
        { "WY", 0.04m }
    };


    public int CalculateTax(int SubtotalInCents, string State)
    {
        if (string.IsNullOrWhiteSpace(State))
        {
            return 0;
        }

        State = State.ToUpper().Trim();

        var taxRate = StateTaxRates[State];
        var taxAmount = SubtotalInCents * taxRate;

        return (int)Math.Round(taxAmount);
    }

    public decimal GetTaxRate(string State)
    {
        if (string.IsNullOrWhiteSpace(State))
        {
            return 0m;
        }

        State = State.ToUpper().Trim();

        return StateTaxRates.GetValueOrDefault(State, 0m);
    }
};