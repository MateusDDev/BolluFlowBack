using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;
using BolluFlow.Domain.Entities;
using BolluFlow.Domain.Enums;
using BolluFlow.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Finance.CreateFinancialEntry;

public class CreateFinancialEntryHandler(ApplicationDbContext db)
    : IRequestHandler<CreateFinancialEntryCommand, ResultOf<FinancialEntryModel>>
{
    public async Task<ResultOf<FinancialEntryModel>> Handle(CreateFinancialEntryCommand request, CancellationToken cancellationToken)
    {
        if (request.Valor <= 0)
            return ResultOf<FinancialEntryModel>.BadRequest(new("FINANCIAL_VALUE_INVALID", "Valor deve ser maior que zero"));

        if (!Enum.TryParse<FinancialEntryType>(request.Tipo, true, out var type))
            return ResultOf<FinancialEntryModel>.BadRequest(new("FINANCIAL_TYPE_INVALID", "Tipo inválido"));

        if (!Enum.TryParse<FinancialOriginType>(request.Origem, true, out var origin))
            return ResultOf<FinancialEntryModel>.BadRequest(new("FINANCIAL_ORIGIN_INVALID", "Origem inválida"));

        var normalizedTipo = type.ToString().ToLowerInvariant();
        var normalizedOrigem = origin.ToString().ToLowerInvariant();
        var eventKey = $"manual:{normalizedTipo}:{normalizedOrigem}:{request.OrigemId}:{request.Valor:0.00}:{request.Descricao.Trim()}";

        var existing = await db.FinancialEntries
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.EventKey == eventKey, cancellationToken);

        if (existing is not null)
            return ResultOf<FinancialEntryModel>.Success(ToModel(existing));

        var entry = FinancialEntry.Create(
            type,
            decimal.Round(request.Valor, 2, MidpointRounding.AwayFromZero),
            request.Descricao,
            origin,
            request.OrigemId,
            "manual",
            eventKey);

        db.FinancialEntries.Add(entry);
        await db.SaveChangesAsync(cancellationToken);

        return ResultOf<FinancialEntryModel>.Success(ToModel(entry));
    }

    private static FinancialEntryModel ToModel(FinancialEntry item) => new()
    {
        Id = item.Id,
        Tipo = item.Type.ToString().ToLowerInvariant(),
        Valor = item.Amount,
        Descricao = item.Description,
        Origem = item.Origin.ToString().ToLowerInvariant(),
        OrigemId = item.OriginId,
        CreatedAt = item.CreatedAt
    };
}
