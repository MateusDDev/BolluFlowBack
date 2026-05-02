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
        var menu = await db.Users
            .AsNoTracking()
            .Where(x => x.IsActive && x.Slug == request.Slug)
            .Select(x => new MenuStoreModel
            {
                Name = x.Name,
                Slug = x.Slug!,
                Whatsapp = x.Whatsapp
            })
            .FirstOrDefaultAsync(cancellationToken);

        return menu is null
            ? ResultOf<MenuStoreModel>.NotFound(new DomainError("MENU_NOT_FOUND", "Cardápio não encontrado"))
            : ResultOf<MenuStoreModel>.Success(menu);
    }
}
