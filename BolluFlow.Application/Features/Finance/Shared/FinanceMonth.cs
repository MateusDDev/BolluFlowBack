using System.Globalization;
using BolluFlow.Domain.Errors;

namespace BolluFlow.Application.Features.Finance.Shared;

public static class FinanceMonth
{
    public static bool TryParseMonth(string month, out DateOnly value)
        => DateOnly.TryParseExact($"{month}-01", "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out value);

    public static bool TryParseDate(string value, out DateOnly date)
        => DateOnly.TryParseExact(value, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out date);

    public static string Format(DateOnly value) => $"{value:yyyy-MM}";

    public static IEnumerable<DateOnly> EnumerateMonths(DateOnly startInclusive, DateOnly endInclusive)
    {
        var cursor = new DateOnly(startInclusive.Year, startInclusive.Month, 1);
        var end = new DateOnly(endInclusive.Year, endInclusive.Month, 1);
        while (cursor <= end)
        {
            yield return cursor;
            cursor = cursor.AddMonths(1);
        }
    }
}
