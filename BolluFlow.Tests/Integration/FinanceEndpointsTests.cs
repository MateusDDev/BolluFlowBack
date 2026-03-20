using System.Net;
using System.Net.Http.Json;
using BolluFlow.Domain.Entities;
using BolluFlow.Domain.Enums;
using BolluFlow.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace BolluFlow.Tests.Integration;

public class FinanceEndpointsTests(DashboardWebApplicationFactory factory) : IClassFixture<DashboardWebApplicationFactory>
{
    [Fact]
    public async Task ShouldCreateManualTransactionAndReturnMonthOverview()
    {
        await ResetDatabaseAsync();
        var client = factory.CreateClient();

        var createResponse = await client.PostAsJsonAsync("/api/finance/transactions", new
        {
            type = "expense",
            description = "Manual expense - packaging refund",
            amount = 45.50m,
            eventDate = "2026-03-20",
            notes = "Supplier adjustment"
        });

        Assert.Equal(HttpStatusCode.OK, createResponse.StatusCode);

        var transactions = await client.GetFromJsonAsync<List<Dictionary<string, object>>>("/api/finance/transactions?month=2026-03");
        var overview = await client.GetFromJsonAsync<Dictionary<string, object>>("/api/finance/overview?month=2026-03");

        Assert.NotNull(transactions);
        Assert.Single(transactions!);
        Assert.NotNull(overview);
        Assert.Equal("2026-03", overview!["month"]?.ToString());
    }

    [Fact]
    public async Task ShouldCreatePurchaseAndAutomaticExpenseTransaction()
    {
        await ResetDatabaseAsync();
        await SeedSupplyAsync();

        var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/inventory/purchases", new
        {
            itemType = "supply",
            itemId = 1,
            itemName = "Box",
            conversionName = "20 unit pack",
            quantity = 2m,
            quantityInBaseUnit = 40m,
            totalCost = 80m,
            purchasedAt = "2026-03-08",
            notes = "Monthly replenishment"
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var transactions = await client.GetFromJsonAsync<List<Dictionary<string, object>>>("/api/finance/transactions?month=2026-03");
        Assert.NotNull(transactions);
        Assert.Contains(transactions!, x => x["sourceType"]?.ToString() == "supply_purchase");
    }

    private async Task ResetDatabaseAsync()
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        db.FinancialTransactions.RemoveRange(db.FinancialTransactions);
        db.Purchases.RemoveRange(db.Purchases);
        db.FixedCosts.RemoveRange(db.FixedCosts);
        db.OrderItems.RemoveRange(db.OrderItems);
        db.Orders.RemoveRange(db.Orders);
        db.Products.RemoveRange(db.Products);
        db.Ingredients.RemoveRange(db.Ingredients);
        db.Supplies.RemoveRange(db.Supplies);
        db.Customers.RemoveRange(db.Customers);
        db.DeliveryAddresses.RemoveRange(db.DeliveryAddresses);

        await db.SaveChangesAsync();
    }

    private async Task SeedSupplyAsync()
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        db.Supplies.Add(Supply.Create(1, "Box", IngredientBaseUnit.Unit, 2, 10, 5));
        await db.SaveChangesAsync();
    }
}
