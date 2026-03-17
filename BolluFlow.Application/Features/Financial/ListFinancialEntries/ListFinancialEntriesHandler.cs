using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;
using BolluFlow.Domain.Errors;
using BolluFlow.Infrastructure.Auth;
using BolluFlow.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Financial.ListFinancialEntries;

public class ListFinancialEntriesHandler(ApplicationDbContext db, CurrentUser user)
    : IRequestHandler<ListFinancialEntriesQuery, ResultOf<PagedResult<FinancialEntryModel>>>
{
    public async Task<ResultOf<PagedResult<FinancialEntryModel>>> Handle(ListFinancialEntriesQuery request, CancellationToken cancellationToken)
    {
        if (request.Page <= 0 || request.PageSize <= 0)
            return ResultOf<PagedResult<FinancialEntryModel>>.BadRequest(FinancialErrors.InvalidPagination);

        await FinancialSalesEntrySync.EnsureSalesEntries(db, user.Id, cancellationToken);

        var query = db.FinancialEntries.Where(x => x.UserId == user.Id).AsQueryable();

        if (request.StartDate is not null)
            query = query.Where(x => x.Date >= request.StartDate.Value);

        if (request.EndDate is not null)
            query = query.Where(x => x.Date <= request.EndDate.Value);

        if (request.Type is not null)
            query = query.Where(x => x.Type == request.Type.Value);

        if (request.Status is not null)
            query = query.Where(x => x.Status == request.Status.Value);

        if (request.IsFixed is not null)
            query = query.Where(x => x.IsFixed == request.IsFixed.Value);

        var totalItems = await query.CountAsync(cancellationToken);

        var entries = await query
            .OrderByDescending(x => x.Date)
            .ThenByDescending(x => x.Id)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        var response = new PagedResult<FinancialEntryModel>
        {
            Items = entries.Select(ToModel).ToList(),
            Page = request.Page,
            PageSize = request.PageSize,
            TotalItems = totalItems,
            TotalPages = (int)Math.Ceiling(totalItems / (double)request.PageSize)
        };

        return ResultOf<PagedResult<FinancialEntryModel>>.Success(response);
    }

    private static FinancialEntryModel ToModel(FinancialEntry entry) => new()
    {
        Id = entry.Id,
        Description = entry.Description,
        Type = entry.Type,
        Category = entry.Category,
        Amount = entry.Amount,
        Date = entry.Date,
        Status = entry.Status,
        IsFixed = entry.IsFixed,
        Notes = entry.Notes,
        IsAutomaticFromSale = entry.IsAutomaticFromSale,
        SourceOrderId = entry.SourceOrderId
    };
}
