using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Errors;
using BolluFlow.Domain.Abstractions;
using BolluFlow.Domain.Entities;
using BolluFlow.Domain.Enums;
using BolluFlow.Infrastructure.Auth;
using BolluFlow.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Products.Components.CreateProductComponent;

public class CreateProductComponentHandler(ApplicationDbContext db, CurrentUser user)
    : IRequestHandler<CreateProductComponentCommand, Result>
{
    public async Task<Result> Handle(CreateProductComponentCommand request, CancellationToken cancellationToken)
    {
        await using var tx = await db.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var product = await db.Products.FirstOrDefaultAsync(x => x.Id == request.ProductId && x.UserId == user.Id, cancellationToken);
            if (product is null)
                return Result.NotFound(CommonErrors.ProductNotFound);

            var validation = Validate(request.Body);
            if (validation is not null)
                return Result.Unprocessable(validation);

            var componentType = request.Body.ComponentType!.Value;
            var sourceId = componentType switch
            {
                ProductComponentType.Recipe => request.Body.RecipeId!.Value,
                ProductComponentType.Ingredient => request.Body.IngredientId!.Value,
                _ => request.Body.SupplyId!.Value
            };

            var duplicated = await db.ProductComponents.AnyAsync(x =>
                x.ProductId == request.ProductId &&
                x.ComponentType == componentType &&
                ((componentType == ProductComponentType.Recipe && x.RecipeId == sourceId) ||
                 (componentType == ProductComponentType.Ingredient && x.IngredientId == sourceId) ||
                 (componentType == ProductComponentType.Supply && x.SupplyId == sourceId)), cancellationToken);

            if (duplicated)
                return Result.Conflict(new DomainError("COMPONENT_ALREADY_EXISTS", "Componente já adicionado ao produto"));

            var source = await LoadSource(componentType, sourceId, cancellationToken);
            if (source is null)
                return Result.NotFound(new DomainError("COMPONENT_SOURCE_NOT_FOUND", "Origem do componente não encontrada"));

            db.ProductComponents.Add(ProductComponent.Create(request.ProductId, componentType, sourceId, source.Value.Name,
                source.Value.BaseUnit, source.Value.UnitPrice, source.Value.StockQuantity, request.Body.QuantityUsed));

            await db.SaveChangesAsync(cancellationToken);
            await tx.CommitAsync(cancellationToken);
            return Result.Success();
        }
        catch (DomainException e)
        {
            await tx.RollbackAsync(cancellationToken);
            return Result.Unprocessable(e.Error);
        }
    }

    private static DomainError? Validate(CreateProductComponentCommandBody body)
    {
        if (body.ComponentType is null)
            return new DomainError("COMPONENT_TYPE_REQUIRED", "componentType é obrigatório");

        return body.ComponentType.Value switch
        {
            ProductComponentType.Recipe when body.RecipeId is null || body.IngredientId is not null || body.SupplyId is not null =>
                new DomainError("INVALID_COMPONENT_PAYLOAD", "Se Recipe, exige recipeId e proíbe ingredientId/supplyId"),
            ProductComponentType.Ingredient when body.IngredientId is null || body.RecipeId is not null || body.SupplyId is not null =>
                new DomainError("INVALID_COMPONENT_PAYLOAD", "Se Ingredient, exige ingredientId e proíbe recipeId/supplyId"),
            ProductComponentType.Supply when body.SupplyId is null || body.RecipeId is not null || body.IngredientId is not null =>
                new DomainError("INVALID_COMPONENT_PAYLOAD", "Se Supply, exige supplyId e proíbe recipeId/ingredientId"),
            _ when body.QuantityUsed <= 0 => new DomainError("INVALID_COMPONENT_QUANTITY", "quantityUsed deve ser maior que zero"),
            _ => null
        };
    }

    private async Task<(string Name, string BaseUnit, decimal UnitPrice, decimal StockQuantity)?> LoadSource(ProductComponentType type, long sourceId, CancellationToken ct)
    {
        if (type == ProductComponentType.Recipe)
        {
            var recipe = await db.Recipes.FirstOrDefaultAsync(x => x.Id == sourceId && x.UserId == user.Id, ct);
            return recipe is null ? null : (recipe.Name, "unit", recipe.FinalPrice, 0);
        }

        if (type == ProductComponentType.Ingredient)
        {
            var ingredient = await db.Ingredients.FirstOrDefaultAsync(x => x.Id == sourceId && x.UserId == user.Id, ct);
            return ingredient is null ? null : (ingredient.Name, ingredient.BaseUnit.ToString(), ingredient.UnitCost, ingredient.StockQuantity);
        }

        var supply = await db.Supplies.FirstOrDefaultAsync(x => x.Id == sourceId && x.UserId == user.Id, ct);
        return supply is null ? null : (supply.Name, supply.BaseUnit.ToString(), supply.UnitCost, supply.StockQuantity);
    }
}
