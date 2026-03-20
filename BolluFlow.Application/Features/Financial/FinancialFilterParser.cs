using BolluFlow.Domain.Enums;

namespace BolluFlow.Application.Features.Financial;

public static class FinancialFilterParser
{
    public static bool TryParseType(string? value, out FinancialEntryType? type)
    {
        type = value?.Trim().ToLowerInvariant() switch
        {
            null or "" => null,
            "income" => FinancialEntryType.Income,
            "expense" => FinancialEntryType.Expense,
            _ => null
        };

        return value is null or "" || type is not null;
    }

    public static bool TryParseStatus(string? value, out FinancialEntryStatus? status)
    {
        status = value?.Trim().ToLowerInvariant() switch
        {
            null or "" => null,
            "paid" => FinancialEntryStatus.Paid,
            "pending" => FinancialEntryStatus.Pending,
            "excluded" => FinancialEntryStatus.Excluded,
            _ => null
        };

        return value is null or "" || status is not null;
    }

    public static bool TryParseSource(string? value, out FinancialEntrySource? source)
    {
        source = value?.Trim().ToLowerInvariant() switch
        {
            null or "" => null,
            "manual" => FinancialEntrySource.Manual,
            "auto-sale" => FinancialEntrySource.AutoSale,
            "auto-stock" => FinancialEntrySource.AutoStock,
            "auto-fixed" => FinancialEntrySource.AutoFixed,
            _ => null
        };

        return value is null or "" || source is not null;
    }
}
