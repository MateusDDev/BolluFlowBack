using System.Text.Json.Serialization;

namespace BolluFlow.Domain.Enums;

public enum FinancialEntrySource
{
    [JsonStringEnumMemberName("manual")]
    Manual,
    [JsonStringEnumMemberName("auto-sale")]
    AutoSale,
    [JsonStringEnumMemberName("auto-stock")]
    AutoStock,
    [JsonStringEnumMemberName("auto-fixed")]
    AutoFixed
}
