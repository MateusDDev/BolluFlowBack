using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;
using BolluFlow.Domain.Errors;
using BolluFlow.Infrastructure.Auth;
using BolluFlow.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Financial.ListFixedAccounts;

public class ListFixedAccountsHandler(ApplicationDbContext db, CurrentUser user)
    : IRequestHandler<ListFixedAccountsQuery, ResultOf<PagedResult<FixedAccountModel>>>
{
    public async Task<ResultOf<PagedResult<FixedAccountModel>>> Handle(ListFixedAccountsQuery request, CancellationToken cancellationToken)
    {
        if (request.Page <= 0 || request.PageSize <= 0)
            return ResultOf<PagedResult<FixedAccountModel>>.BadRequest(FinancialErrors.InvalidPagination);

        var query = db.FixedAccounts.Where(x => x.UserId == user.Id).AsQueryable();
        var totalItems = await query.CountAsync(cancellationToken);

        var accounts = await query
            .OrderBy(x => x.Description)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new FixedAccountModel
            {
                Id = x.Id,
                Description = x.Description,
                Category = x.Category,
                DefaultAmount = x.DefaultAmount,
                DueDay = x.DueDay,
                IsActive = x.IsActive
            })
            .ToListAsync(cancellationToken);

        return ResultOf<PagedResult<FixedAccountModel>>.Success(new PagedResult<FixedAccountModel>
        {
            Items = accounts,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalItems = totalItems,
            TotalPages = (int)Math.Ceiling(totalItems / (double)request.PageSize)
        });
    }
}
