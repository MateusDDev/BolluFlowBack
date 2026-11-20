using BolluFlow.Application.Abstractions;
using BolluFlow.Infrastructure.Auth;
using BolluFlow.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Onboarding.GetOnboardingProgress;

public class GetOnboardingProgressHandler(ApplicationDbContext db, CurrentUser currentUser)
    : IRequestHandler<GetOnboardingProgressQuery, ResultOf<GetOnboardingProgressResult>>
{
    public async Task<ResultOf<GetOnboardingProgressResult>> Handle(GetOnboardingProgressQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUser.Id;

        var hasProduct = await db.Products.AnyAsync(x => x.UserId == userId, cancellationToken);
        var hasPricedProduct = await db.Products.AnyAsync(x => x.UserId == userId && x.FinalPrice > 0, cancellationToken);
        var hasCustomer = await db.Customers.AnyAsync(x => x.UserId == userId, cancellationToken);
        var hasOrder = await db.Orders.AnyAsync(x => x.UserId == userId, cancellationToken);
        var hasMenuLink = await db.Users.AnyAsync(x => x.Id == userId && x.Slug != null, cancellationToken);
        var hasRecipe = await db.Recipes.AnyAsync(x => x.UserId == userId, cancellationToken);
        var hasProductComponent = await db.ProductComponents.AnyAsync(x => x.Product.UserId == userId, cancellationToken);
        var hasTrackedStock =
            await db.Ingredients.AnyAsync(x => x.UserId == userId && x.TrackStock, cancellationToken) ||
            await db.Supplies.AnyAsync(x => x.UserId == userId && x.TrackStock, cancellationToken) ||
            await db.Products.AnyAsync(x => x.UserId == userId && x.TrackStock, cancellationToken);
        var hasFinancialEntry = await db.FinancialEntries.AnyAsync(x => x.UserId == userId, cancellationToken);

        return ResultOf<GetOnboardingProgressResult>.Success(new GetOnboardingProgressResult(
            hasProduct,
            hasPricedProduct,
            hasCustomer,
            hasOrder,
            hasMenuLink,
            hasRecipe,
            hasProductComponent,
            hasTrackedStock,
            hasFinancialEntry));
    }
}
