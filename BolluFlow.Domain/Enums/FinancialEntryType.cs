using System.Text.Json.Serialization;

namespace BolluFlow.Domain.Enums;

public enum FinancialEntryType
{
    [JsonStringEnumMemberName("income")]
    Income,
    [JsonStringEnumMemberName("expense")]
    Expense
}
