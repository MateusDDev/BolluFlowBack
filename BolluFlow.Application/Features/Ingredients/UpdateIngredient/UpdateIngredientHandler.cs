using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Errors;
using BolluFlow.Application.Models;
using BolluFlow.Domain.Abstractions;
using BolluFlow.Infrastructure.Persistence;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Ingredients.UpdateIngredient;

public class UpdateIngredientHandler(ApplicationDbContext db) : IRequestHandler<UpdateIngredientCommand, ResultOf<IngredientModel>>
{
    public async Task<ResultOf<IngredientModel>> Handle(UpdateIngredientCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var command = request.Body;

            var ingredient = await db.Ingredients
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (ingredient is null)
                return ResultOf<IngredientModel>.NotFound(CommonErrors.IngredientNotFound);

            ingredient.Update(
                command.Name, 
                command.Unit, 
                command.UnitCost, 
                command.StockQuantity, 
                command.MinimumStockQuantity);

            await db.SaveChangesAsync(cancellationToken);

            return ResultOf<IngredientModel>.Success(ingredient.Adapt<IngredientModel>());
        }
        catch (DomainException e)
        {
            return ResultOf<IngredientModel>.BadRequest(e.Error);
        }
    }
}