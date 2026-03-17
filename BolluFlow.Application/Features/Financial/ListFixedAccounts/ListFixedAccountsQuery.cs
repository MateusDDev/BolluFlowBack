using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;

namespace BolluFlow.Application.Features.Financial.ListFixedAccounts;

public record ListFixedAccountsQuery(int Page = 1, int PageSize = 20) : IQuery<ResultOf<PagedResult<FixedAccountModel>>>;
