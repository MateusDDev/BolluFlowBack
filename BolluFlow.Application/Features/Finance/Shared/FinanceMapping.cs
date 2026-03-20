using System.Text.Json.Nodes;
using BolluFlow.Domain.Entities;

namespace BolluFlow.Application.Features.Finance.Shared;

public static class FinanceMapping
{
    public static FinancialTransactionDto ToDto(this FinancialTransaction transaction)
        => new()
        {
            Id = transaction.Id,
            Type = transaction.Type.ToString().ToLowerInvariant(),
            SourceType = ToKebabCase(transaction.SourceType.ToString()),
            Description = transaction.Description,
            Amount = transaction.Amount,
            EventDate = transaction.EventDate.ToString("yyyy-MM-dd"),
            Month = transaction.Month,
            SourceEntityType = ToKebabCase(transaction.SourceEntityType.ToString()),
            SourceEntityId = transaction.SourceEntityId,
            IsAutomatic = transaction.IsAutomatic,
            Notes = transaction.Notes,
            Metadata = string.IsNullOrWhiteSpace(transaction.Metadata) ? null : JsonNode.Parse(transaction.Metadata)
        };

    public static FixedCostDto ToDto(this FixedCost fixedCost)
        => new()
        {
            Id = fixedCost.Id,
            Name = fixedCost.Name,
            Amount = fixedCost.Amount,
            StartMonth = fixedCost.StartMonth,
            EndMonth = fixedCost.EndMonth,
            Notes = fixedCost.Notes,
            CreatedAt = fixedCost.CreatedAt,
            UpdatedAt = fixedCost.UpdatedAt
        };

    public static FinancialTransactionDto ToFixedCostTransactionDto(this FixedCost fixedCost, string month)
        => new()
        {
            Id = DeterministicGuid($"fixed-cost:{fixedCost.Id}:{month}"),
            Type = "expense",
            SourceType = "fixed_cost",
            Description = fixedCost.Name,
            Amount = fixedCost.Amount,
            EventDate = $"{month}-01",
            Month = month,
            SourceEntityType = "fixed_cost",
            SourceEntityId = fixedCost.Id,
            IsAutomatic = true,
            Notes = fixedCost.Notes,
            Metadata = JsonNode.Parse("{\"recurring\":true}")
        };

    private static Guid DeterministicGuid(string value)
    {
        var bytes = System.Security.Cryptography.MD5.HashData(System.Text.Encoding.UTF8.GetBytes(value));
        return new Guid(bytes);
    }

    private static string ToKebabCase(string value)
        => string.Concat(value.Select((ch, index) => index > 0 && char.IsUpper(ch) ? $"_{char.ToLowerInvariant(ch)}" : char.ToLowerInvariant(ch).ToString()));
}
