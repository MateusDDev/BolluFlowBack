using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Errors;
using BolluFlow.Application.Models;
using BolluFlow.Domain.Abstractions;
using BolluFlow.Domain.Entities;
using BolluFlow.Domain.Enums;
using BolluFlow.Infrastructure.Auth;
using BolluFlow.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Products.Productions.RecordProduction;

public class RecordProductProductionHandler(ApplicationDbContext db, CurrentUser user)
    : IRequestHandler<RecordProductProductionCommand, ResultOf<ProductProductionModel>>
{
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

            var requiredByComponent = product.Components
                .Select(component => new
                {
                    Component = component,
                    Required = component.QuantityUsed * request.Body.Quantity
                })
                .ToList();

            var skipStockCheck = await db.Users
                .Where(x => x.Id == product.UserId)
                .Select(x => x.SkipStockCheck)
                .FirstOrDefaultAsync(cancellationToken);

            if (!skipStockCheck)
            {
                var insufficientComponents = requiredByComponent
                    .Select(x => new
                    {
                        x.Component,
                        x.Required,
                        Availability = GetAvailability(x.Component)
                    })
                    .Where(x => x.Availability is null || x.Availability.Value.Available < x.Required)
                    .Select(x => new
                    {
                        componentType = x.Component.ComponentType.ToString().ToLowerInvariant(),
                        componentId = x.Availability?.Id ?? x.Component.RecipeId ?? x.Component.IngredientId ?? x.Component.SupplyId ?? 0,
                        componentName = x.Availability?.Name ?? x.Component.Name,
                        required = x.Required,
                        available = x.Availability?.Available ?? 0
                    })
                    .ToList();

                if (insufficientComponents.Count > 0)
                {
                    return ResultOf<ProductProductionModel>.Unprocessable(
                        new DomainError("INSUFFICIENT_STOCK", "Estoque insuficiente para produzir este produto"),
                        new { insufficientComponents });
                }
            }

            var production = ProductProduction.Create(request.ProductId, request.Body.Quantity, request.Body.Notes?.Trim());
            db.ProductProductions.Add(production);
            await db.SaveChangesAsync(cancellationToken);

            var debits = new List<ProductProductionDebitModel>();

            if (!skipStockCheck)
            {
                foreach (var componentData in requiredByComponent)
                {
                    var component = componentData.Component;
                    var quantityToDebit = componentData.Required;

                    if (component.ComponentType == ProductComponentType.Ingredient && component.Ingredient is not null)
                    {
                        component.Ingredient.ConsumeStock(quantityToDebit);
                        debits.Add(new ProductProductionDebitModel
                        {
                            ComponentType = "ingredient",
                            ComponentId = component.Ingredient.Id,
                            ComponentName = component.Ingredient.Name,
                            QuantityDebited = quantityToDebit
                        });
                        continue;
                    }

                    if (component.ComponentType == ProductComponentType.Supply && component.Supply is not null)
                    {
                        component.Supply.ConsumeStock(quantityToDebit);
                        debits.Add(new ProductProductionDebitModel
                        {
                            ComponentType = "supply",
                            ComponentId = component.Supply.Id,
                            ComponentName = component.Supply.Name,
                            QuantityDebited = quantityToDebit
                        });
                        continue;
                    }

                    if (component.ComponentType == ProductComponentType.Recipe && component.Recipe is not null)
                    {
                        component.Recipe.ConsumeStock(quantityToDebit);
                        debits.Add(new ProductProductionDebitModel
                        {
                            ComponentType = "recipe",
                            ComponentId = component.Recipe.Id,
                            ComponentName = component.Recipe.Name,
                            QuantityDebited = quantityToDebit
                        });
                        continue;
                    }

                    throw new DomainException(new DomainError("PRODUCT_COMPONENT_INVALID", "Componente do produto inválido para produção"));
                }
            }

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

    private static (long Id, string Name, decimal Available)? GetAvailability(ProductComponent component)
    {
        return component.ComponentType switch
        {
            ProductComponentType.Ingredient when component.Ingredient is not null =>
                (component.Ingredient.Id, component.Ingredient.Name, component.Ingredient.StockQuantity),
            ProductComponentType.Supply when component.Supply is not null =>
                (component.Supply.Id, component.Supply.Name, component.Supply.StockQuantity),
            ProductComponentType.Recipe when component.Recipe is not null =>
                (component.Recipe.Id, component.Recipe.Name, component.Recipe.StockQuantity),
            _ => null
        };
    }
}
