using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;
using BolluFlow.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Ingredients.UnitConversion.ListConversions;

public class ListConversionsHandler(ApplicationDbContext db) : IRequestHandler<ListConversionsQuery, ResultOf<List<IngredientConversionModel>>>
{
    public async Task<ResultOf<List<IngredientConversionModel>>> Handle(ListConversionsQuery request, CancellationToken cancellationToken)
    {
        var ingredientConversions = await db.IngredientConversions
            .Where(x => x.IngredientId == request.IngredientId)
            .ToListAsync(cancellationToken);
        
        var result = ingredientConversions.Select(IngredientConversionModel.From).ToList();

        return ResultOf<List<IngredientConversionModel>>.Success(result);
    }
}