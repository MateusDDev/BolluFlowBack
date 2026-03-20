using System.Text.Json.Serialization;

namespace BolluFlow.Domain.Enums;

public enum DeletionMode
{
    [JsonStringEnumMemberName("until-month")]
    UntilMonth,
    [JsonStringEnumMemberName("from-month")]
    FromMonth,
    [JsonStringEnumMemberName("all")]
    All
}
