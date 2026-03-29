using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;
using BolluFlow.Domain.Enums;
using BolluFlow.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Finance.ListFinancialEntries;

public class ListFinancialEntriesHandler(ApplicationDbContext db)
    : IRequestHandler<ListFinancialEntriesQuery, ResultOf<List<FinancialEntryModel>>>
{
    public async Task<ResultOf<List<FinancialEntryModel>>> Handle(ListFinancialEntriesQuery request, CancellationToken cancellationToken)
    {
        var query = db.FinancialEntries.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Tipo))
        {
            if (!Enum.TryParse<FinancialEntryType>(request.Tipo, true, out var type))
                return ResultOf<List<FinancialEntryModel>>.BadRequest(new("FINANCIAL_TYPE_INVALID", "Tipo inválido"));

            query = query.Where(x => x.Type == type);
        }

        var entries = await query
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new FinancialEntryModel
            {
                Id = x.Id,
                Tipo = x.Type.ToString().ToLower(),
                Valor = x.Amount,
                Descricao = x.Description,
                Origem = x.Origin.ToString().ToLower(),
                OrigemId = x.OriginId,
                CreatedAt = x.CreatedAt
            })
            .ToListAsync(cancellationToken);

        return ResultOf<List<FinancialEntryModel>>.Success(entries);
    }
}
