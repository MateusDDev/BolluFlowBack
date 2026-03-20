using System.Text.Json.Serialization;

namespace BolluFlow.Domain.Enums;

public enum FinancialEntryStatus
{
    [JsonStringEnumMemberName("paid")]
    Paid,
    [JsonStringEnumMemberName("pending")]
    Pending,
    [JsonStringEnumMemberName("excluded")]
    Excluded
}
