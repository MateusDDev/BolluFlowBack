using BolluFlow.Application.Abstractions;
using BolluFlow.Domain.Abstractions;
using BolluFlow.Domain.Entities;
using BolluFlow.Infrastructure.Auth;
using BolluFlow.Infrastructure.Persistence;
using MediatR;

namespace BolluFlow.Application.Features.Recipes.Create;

public class CreateRecipeHandler(ApplicationDbContext db, CurrentUser user) : IRequestHandler<CreateRecipeCommand, ResultOf<CreateRecipeResult>>
{
    public async Task<ResultOf<CreateRecipeResult>> Handle(CreateRecipeCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var recipe = Recipe.Create(user.Id, request.Name, request.Category, request.PreparationTime, request.FinalPrice ?? 0, request.BaseUnit, request.StockQuantity ?? 0);
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
