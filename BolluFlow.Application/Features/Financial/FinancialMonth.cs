namespace BolluFlow.Application.Features.Financial;

public readonly record struct FinancialMonth(DateOnly Start, DateOnly End, string Value)
{
    public static bool TryParse(string month, out FinancialMonth financialMonth)
    {
        financialMonth = default;

        if (!DateOnly.TryParse($"{month}-01", out var date))
            return false;

        var start = new DateOnly(date.Year, date.Month, 1);
        var end = start.AddMonths(1).AddDays(-1);
        financialMonth = new FinancialMonth(start, end, start.ToString("yyyy-MM"));
        return true;
    }
}
