using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;
using BolluFlow.Domain.Abstractions;
using BolluFlow.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Menu.GetMenu;

public class GetMenuHandler(ApplicationDbContext db) : IRequestHandler<GetMenuQuery, ResultOf<MenuStoreModel>>
{
    public async Task<ResultOf<MenuStoreModel>> Handle(GetMenuQuery request, CancellationToken cancellationToken)
    {
        var owner = await db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.IsActive && x.Slug == request.Slug, cancellationToken);

        var menu = owner is null
            ? null
            : new MenuStoreModel
            {
                Name = owner.Name,
                Slug = owner.Slug!,
                Whatsapp = owner.Whatsapp,
                DeliveryTimeMinutes = owner.DeliveryTimeMinutes,
                PixKey = owner.PixKey,
                StoreAddress = StoreAddressModel.From(owner.StoreAddress)
            };

        return menu is null
            ? ResultOf<MenuStoreModel>.NotFound(new DomainError("MENU_NOT_FOUND", "Cardápio não encontrado"))
            : ResultOf<MenuStoreModel>.Success(menu);
    }
}
