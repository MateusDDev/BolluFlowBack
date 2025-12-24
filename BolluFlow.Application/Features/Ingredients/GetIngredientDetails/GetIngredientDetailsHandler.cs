using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Errors;
using BolluFlow.Application.Models;
using BolluFlow.Infrastructure.Persistence;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Ingredients.GetIngredientDetails;

public class GetIngredientDetailsHandler(ApplicationDbContext db) : IRequestHandler<GetIngredientDetailsQuery, ResultOf<IngredientModel>>
{
    public async Task<ResultOf<IngredientModel>> Handle(GetIngredientDetailsQuery request, CancellationToken cancellationToken)
    {
        var ingredient = await db.Ingredients
            .FirstOrDefaultAsync(x => x.Id == request.IngredientId, cancellationToken);

        if (ingredient is null)
            return ResultOf<IngredientModel>.NotFound(CommonErrors.IngredientNotFound);

        return ResultOf<IngredientModel>.Success(ingredient.Adapt<IngredientModel>());
    }
}