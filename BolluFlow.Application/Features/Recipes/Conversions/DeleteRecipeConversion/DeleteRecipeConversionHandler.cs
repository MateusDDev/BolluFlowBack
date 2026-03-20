using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Errors;
using BolluFlow.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Recipes.Conversions.DeleteRecipeConversion;

public class DeleteRecipeConversionHandler(ApplicationDbContext db) : IRequestHandler<DeleteRecipeConversionCommand, Result>
{
    public async Task<Result> Handle(DeleteRecipeConversionCommand request, CancellationToken cancellationToken)
    {
        var conversion = await db.RecipeConversions.FirstOrDefaultAsync(x => x.RecipeId == request.RecipeId && x.Id == request.ConversionId, cancellationToken);
        if (conversion is null)
            return Result.NotFound(CommonErrors.RecipeNotFound);

        db.RecipeConversions.Remove(conversion);
        await db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
