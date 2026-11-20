using BolluFlow.Domain.Abstractions;
using BolluFlow.Domain.Enums;
using BolluFlow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Services;

public sealed record StockPolicy(bool IsActive, bool AllowNegativeStock)
{
    public static async Task<StockPolicy> LoadAsync(ApplicationDbContext db, long userId, CancellationToken cancellationToken)
    {
        var settings = await db.Users
            .Where(x => x.Id == userId)
            .Select(x => new { x.EnabledModules, x.AllowNegativeStock })
            .FirstAsync(cancellationToken);

        return new StockPolicy(settings.EnabledModules.HasFlag(AppModule.Stock), settings.AllowNegativeStock);
    }

    public bool HasEnough(IStockItem item, decimal quantity)
    {
        return !IsActive || item.HasEnoughStock(quantity, AllowNegativeStock);
    }

    public bool Consume(IStockItem item, decimal quantity)
    {
        if (!IsActive || !item.TrackStock)
            return false;

        item.ConsumeStock(quantity, AllowNegativeStock);
        return true;
    }
}
