using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;
using BolluFlow.Domain.Enums;
using BolluFlow.Domain.Errors;
using BolluFlow.Infrastructure.Auth;
using BolluFlow.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Financial.ListSalesEntries;

public class ListSalesEntriesHandler(ApplicationDbContext db, CurrentUser user)
    : IRequestHandler<ListSalesEntriesQuery, ResultOf<PagedResult<SaleFinancialEntryModel>>>
{
    public async Task<ResultOf<PagedResult<SaleFinancialEntryModel>>> Handle(ListSalesEntriesQuery request, CancellationToken cancellationToken)
    {
        if (request.Page <= 0 || request.PageSize <= 0)
            return ResultOf<PagedResult<SaleFinancialEntryModel>>.BadRequest(FinancialErrors.InvalidPagination);

        await FinancialSalesEntrySync.EnsureSalesEntries(db, user.Id, cancellationToken);

        var query = db.FinancialEntries
            .Where(x => x.UserId == user.Id && x.IsAutomaticFromSale)
            .AsQueryable();

        if (request.StartDate is not null)
            query = query.Where(x => x.Date >= request.StartDate.Value);

        if (request.EndDate is not null)
            query = query.Where(x => x.Date <= request.EndDate.Value);

        var totalItems = await query.CountAsync(cancellationToken);

        var entries = await query
            .OrderByDescending(x => x.Date)
            .ThenByDescending(x => x.Id)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        var response = new PagedResult<SaleFinancialEntryModel>
        {
            Items = entries.Select(x => new SaleFinancialEntryModel
            {
                Id = $"sale-order-{x.SourceOrderId}",
                OrderId = x.SourceOrderId ?? 0,
                Description = x.Description,
                Amount = x.Amount,
                Date = x.Date,
                Status = x.Status
            }).ToList(),
            Page = request.Page,
            PageSize = request.PageSize,
            TotalItems = totalItems,
            TotalPages = (int)Math.Ceiling(totalItems / (double)request.PageSize)
        };

        return ResultOf<PagedResult<SaleFinancialEntryModel>>.Success(response);
    }
}
