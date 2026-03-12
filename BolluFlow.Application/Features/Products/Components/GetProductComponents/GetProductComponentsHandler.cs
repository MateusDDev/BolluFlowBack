using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;
using BolluFlow.Infrastructure.Auth;
using BolluFlow.Infrastructure.Persistence;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Products.Components.GetProductComponents;

public class GetProductComponentsHandler(ApplicationDbContext db, CurrentUser user) : IRequestHandler<GetProductComponentsQuery, ResultOf<List<ProductComponentModel>>>
{
    public async Task<ResultOf<List<ProductComponentModel>>> Handle(GetProductComponentsQuery request, CancellationToken cancellationToken)
    {
        var items = await db.ProductComponents
            .Where(x => x.ProductId == request.ProductId && x.Product.UserId == user.Id)
            .ToListAsync(cancellationToken);

        return ResultOf<List<ProductComponentModel>>.Success(items.Adapt<List<ProductComponentModel>>());
    }
}
