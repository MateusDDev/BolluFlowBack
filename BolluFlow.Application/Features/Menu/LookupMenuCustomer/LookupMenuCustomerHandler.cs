using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;
using BolluFlow.Domain.Abstractions;
using BolluFlow.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Menu.LookupMenuCustomer;

public class LookupMenuCustomerHandler(ApplicationDbContext db) : IRequestHandler<LookupMenuCustomerQuery, ResultOf<MenuCustomerLookupModel>>
{
    public async Task<ResultOf<MenuCustomerLookupModel>> Handle(LookupMenuCustomerQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Whatsapp))
            return ResultOf<MenuCustomerLookupModel>.BadRequest(new DomainError("MENU_CUSTOMER_WHATSAPP_REQUIRED", "whatsapp é obrigatório"));

        var normalizedWhatsapp = request.Whatsapp.Trim();

        if (normalizedWhatsapp.Length < 8 || normalizedWhatsapp.Any(x => !char.IsDigit(x)))
            return ResultOf<MenuCustomerLookupModel>.BadRequest(new DomainError("MENU_CUSTOMER_WHATSAPP_INVALID", "whatsapp deve conter ao menos 8 dígitos numéricos"));

        var ownerId = await db.Users
            .AsNoTracking()
            .Where(x => x.IsActive && x.Slug == request.Slug)
            .Select(x => (long?)x.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (!ownerId.HasValue)
            return ResultOf<MenuCustomerLookupModel>.NotFound(new DomainError("MENU_NOT_FOUND", "Cardápio não encontrado"));

        var exists = await db.Customers
            .AsNoTracking()
            .AnyAsync(x => x.UserId == ownerId.Value && x.Whatsapp == normalizedWhatsapp, cancellationToken);

        return ResultOf<MenuCustomerLookupModel>.Success(new MenuCustomerLookupModel
        {
            Exists = exists
        });
    }
}
