using System.Text.Json.Serialization;

namespace BolluFlow.Domain.Enums;

public enum FinancialReferenceType
{
    [JsonStringEnumMemberName("none")]
    None,
    [JsonStringEnumMemberName("order")]
    Order,
    [JsonStringEnumMemberName("stock_purchase")]
    StockPurchase,
    [JsonStringEnumMemberName("fixed_account")]
    FixedAccount
}
