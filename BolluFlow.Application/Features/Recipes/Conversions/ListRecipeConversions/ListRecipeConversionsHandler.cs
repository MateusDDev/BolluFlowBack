using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;
using BolluFlow.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Recipes.Conversions.ListRecipeConversions;

public class ListRecipeConversionsHandler(ApplicationDbContext db) : IRequestHandler<ListRecipeConversionsQuery, ResultOf<List<RecipeConversionModel>>>
{
    public async Task<ResultOf<List<RecipeConversionModel>>> Handle(ListRecipeConversionsQuery request, CancellationToken cancellationToken)
    {
        var conversions = await db.RecipeConversions.Where(x => x.RecipeId == request.RecipeId)
            .Select(x => new RecipeConversionModel { Id = x.Id, Name = x.Name, QuantityInBaseUnit = x.QuantityInBaseUnit })
            .ToListAsync(cancellationToken);

        return ResultOf<List<RecipeConversionModel>>.Success(conversions);
    }
}
