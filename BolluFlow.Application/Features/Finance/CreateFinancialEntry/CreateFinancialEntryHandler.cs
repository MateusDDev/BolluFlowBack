using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;
using BolluFlow.Domain.Entities;
using BolluFlow.Domain.Enums;
using BolluFlow.Infrastructure.Persistence;
using MediatR;

namespace BolluFlow.Application.Features.Finance.CreateFinancialEntry;

public class CreateFinancialEntryHandler(ApplicationDbContext db)
    : IRequestHandler<CreateFinancialEntryCommand, ResultOf<FinancialEntryModel>>
{
    public async Task<ResultOf<FinancialEntryModel>> Handle(CreateFinancialEntryCommand request, CancellationToken cancellationToken)
    {
        if (request.Amount <= 0)
            return ResultOf<FinancialEntryModel>.BadRequest(new("FINANCIAL_AMOUNT_INVALID", "amount must be greater than zero"));

        if (string.IsNullOrWhiteSpace(request.Description))
            return ResultOf<FinancialEntryModel>.BadRequest(new("FINANCIAL_DESCRIPTION_REQUIRED", "description is required"));

        if (!Enum.TryParse<FinancialEntryType>(request.Type, true, out var type))
            return ResultOf<FinancialEntryModel>.BadRequest(new("FINANCIAL_TYPE_INVALID", "type must be income or expense"));

        var entry = FinancialEntry.Create(
            type,
            decimal.Round(request.Amount, 2, MidpointRounding.AwayFromZero),
            request.Description.Trim(),
            FinancialOriginType.Manual,
            null,
            null);

        db.FinancialEntries.Add(entry);
        await db.SaveChangesAsync(cancellationToken);

        return ResultOf<FinancialEntryModel>.Success(ToModel(entry));
    }

    private static FinancialEntryModel ToModel(FinancialEntry item) => new()
    {
        Id = item.Id,
        Type = ToSnakeCase(item.Type.ToString()),
        Amount = item.Amount,
        Description = item.Description,
        Origin = ToSnakeCase(item.Origin.ToString()),
        OriginId = item.OriginId,
        CreatedAt = item.CreatedAt
    };

    private static string ToSnakeCase(string value)
    {
        return string.Concat(value.Select((c, i) => i > 0 && char.IsUpper(c) ? "_" + char.ToLowerInvariant(c) : char.ToLowerInvariant(c).ToString()));
    }
}
