using BolluFlow.Application.Abstractions;
using BolluFlow.Domain.Abstractions;
using BolluFlow.Domain.Entities;
using BolluFlow.Domain.Enums;
using BolluFlow.Infrastructure.Auth;
using BolluFlow.Infrastructure.Persistence;
using MediatR;

namespace BolluFlow.Application.Features.Recipes.Create;

public class CreateRecipeHandler(ApplicationDbContext db, CurrentUser user) : IRequestHandler<CreateRecipeCommand, ResultOf<CreateRecipeResult>>
{
    public async Task<ResultOf<CreateRecipeResult>> Handle(
        CreateRecipeCommand request,
        CancellationToken cancellationToken)
    {
        if (!Enum.IsDefined(typeof(BaseUnit), request.BaseUnit))
            return ResultOf<CreateRecipeResult>.BadRequest(new DomainError("RECIPE_BASE_UNIT_INVALID", "baseUnit inválida"));

        try
        {
            var recipe = Recipe.Create(
                userId: user.Id,
                name: request.Name,
                category: request.Category,
                baseUnit: request.BaseUnit,
                preparationTime: request.PreparationTime,
                finalPrice: request.FinalPrice ?? 0);

            db.Recipes.Add(recipe);

            await db.SaveChangesAsync(cancellationToken);

            return ResultOf<CreateRecipeResult>.Success(new CreateRecipeResult(recipe.Id));
        }
        catch (DomainException e)
        {
            return ResultOf<CreateRecipeResult>.BadRequest(e.Error);
        }
    }
}
