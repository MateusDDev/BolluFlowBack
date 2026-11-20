using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Errors;
using BolluFlow.Application.Models;
using BolluFlow.Application.Services;
using BolluFlow.Domain.Abstractions;
using BolluFlow.Domain.Entities;
using BolluFlow.Infrastructure.Auth;
using BolluFlow.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Products.Productions.RecordProduction;

public class RecordProductProductionHandler(ApplicationDbContext db, CurrentUser user)
    : IRequestHandler<RecordProductProductionCommand, ResultOf<ProductProductionModel>>
{
    private static readonly DomainError InvalidComponent =
        new("PRODUCT_COMPONENT_INVALID", "Componente do produto inválido para produção");

    public async Task<ResultOf<ProductProductionModel>> Handle(RecordProductProductionCommand request, CancellationToken cancellationToken)
    {
        if (request.Body.Quantity <= 0)
            return ResultOf<ProductProductionModel>.BadRequest(new DomainError("PRODUCT_PRODUCTION_QUANTITY_INVALID", "quantity deve ser maior que zero"));

        await using var tx = await db.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var product = await db.Products
                .Include(x => x.Components)
                .ThenInclude(x => x.Ingredient)
                .Include(x => x.Components)
                .ThenInclude(x => x.Supply)
                .Include(x => x.Components)
                .ThenInclude(x => x.Recipe)
                .FirstOrDefaultAsync(x => x.Id == request.ProductId && x.UserId == user.Id, cancellationToken);

            if (product is null)
                return ResultOf<ProductProductionModel>.NotFound(CommonErrors.ProductNotFound);

            var policy = await StockPolicy.LoadAsync(db, user.Id, cancellationToken);

            var requirements = product.Components
                .Select(component => new
                {
                    Component = component,
                    Item = component.StockItem,
                    Required = component.QuantityUsed * request.Body.Quantity
                })
                .ToList();

            var insufficientComponents = requirements
                .Where(x => x.Item is null || !policy.HasEnough(x.Item, x.Required))
                .Select(x => new
                {
                    componentType = x.Component.ComponentType.ToString().ToLowerInvariant(),
                    componentId = x.Item?.Id ?? 0,
                    componentName = x.Item?.Name ?? x.Component.Name,
                    required = x.Required,
                    available = x.Item?.StockQuantity ?? 0
                })
                .ToList();

            if (insufficientComponents.Count > 0)
            {
                return ResultOf<ProductProductionModel>.Unprocessable(
                    new DomainError("INSUFFICIENT_STOCK", "Estoque insuficiente para produzir este produto"),
                    new { insufficientComponents });
            }

            var production = ProductProduction.Create(request.ProductId, request.Body.Quantity, request.Body.Notes?.Trim());
            db.ProductProductions.Add(production);
            await db.SaveChangesAsync(cancellationToken);

            var debits = new List<ProductProductionDebitModel>();

            foreach (var requirement in requirements)
            {
                var item = requirement.Item ?? throw new DomainException(InvalidComponent);

                policy.Consume(item, requirement.Required);

                if (policy.IsActive)
                {
                    debits.Add(new ProductProductionDebitModel
                    {
                        ComponentType = requirement.Component.ComponentType.ToString().ToLowerInvariant(),
                        ComponentId = item.Id,
                        ComponentName = item.Name,
                        QuantityDebited = requirement.Required
                    });
                }
            }

            if (policy.IsActive)
                product.AddStock(request.Body.Quantity);

            await db.SaveChangesAsync(cancellationToken);
            await tx.CommitAsync(cancellationToken);

            return ResultOf<ProductProductionModel>.Success(new ProductProductionModel
            {
                Id = production.Id,
                ProductId = product.Id,
                Quantity = production.Quantity,
                Notes = production.Notes,
                Debits = debits,
                CreatedAt = production.CreatedAt
            });
        }
        catch (DomainException e)
        {
            await tx.RollbackAsync(cancellationToken);
            return ResultOf<ProductProductionModel>.Unprocessable(e.Error);
        }
    }
}
