using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;
using BolluFlow.Domain.Abstractions;
using BolluFlow.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Menu.ListMenuProducts;

public class ListMenuProductsHandler(ApplicationDbContext db) : IRequestHandler<ListMenuProductsQuery, ResultOf<List<MenuProductModel>>>
{
    public async Task<ResultOf<List<MenuProductModel>>> Handle(ListMenuProductsQuery request, CancellationToken cancellationToken)
    {
        var owner = await db.Users
            .AsNoTracking()
            .Where(x => x.IsActive && x.Slug == request.Slug)
            .Select(x => new
            {
                x.Id
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (owner is null)
            return ResultOf<List<MenuProductModel>>.NotFound(new DomainError("MENU_NOT_FOUND", "Cardápio não encontrado"));

        var products = await db.Products
            .AsNoTracking()
            .Where(x => x.UserId == owner.Id && x.IsActive && x.FinalPrice > 0)
            .OrderBy(x => x.Name)
            .Select(x => new MenuProductModel
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                FinalPrice = x.FinalPrice,
                Category = x.Category,
                IsAvailable = x.StockQuantity > 0,
                ImageUrl = x.ImageUrl
            })
            .ToListAsync(cancellationToken);

        return ResultOf<List<MenuProductModel>>.Success(products);
    }
}
