using System.Net;
using System.Net.Http.Json;
using BolluFlow.Application.Models;
using BolluFlow.Domain.Entities;
using BolluFlow.Domain.Enums;
using BolluFlow.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace BolluFlow.Tests.Integration;

public class FinancialEndpointsTests(DashboardWebApplicationFactory factory) : IClassFixture<DashboardWebApplicationFactory>
{
    [Fact]
    public async Task ShouldReturnMonthlyOverviewAndAutomaticEntries()
    {
        await ResetDatabaseAsync();
        await SeedFinancialDataAsync();

        var client = factory.CreateClient();

        var generateResponse = await client.PostAsync("/financial/fixed-accounts/generate-month?month=2026-03", null);
        var overview = await client.GetFromJsonAsync<FinancialOverviewModel>("/financial/overview?month=2026-03");
        var autoSales = await client.GetFromJsonAsync<PagedResult<AutoFinancialEntryModel>>("/financial/entries/auto-sales?month=2026-03&page=1&pageSize=50");
        var autoStockPurchases = await client.GetFromJsonAsync<PagedResult<AutoFinancialEntryModel>>("/financial/entries/auto-stock-purchases?month=2026-03&page=1&pageSize=50");
        var entries = await client.GetFromJsonAsync<PagedResult<FinancialEntryModel>>("/financial/entries?month=2026-03&page=1&pageSize=50");

        Assert.Equal(HttpStatusCode.OK, generateResponse.StatusCode);
        Assert.NotNull(overview);
        Assert.Equal("2026-03", overview!.Month);
        Assert.Equal(50m, overview.CurrentBalance);
        Assert.Equal(20m, overview.ForecastBalance);
        Assert.Equal(200m, overview.TotalIncomesPaid);
        Assert.Equal(150m, overview.TotalExpensesPaid);
        Assert.Equal(100m, overview.PendingToReceive);
        Assert.Equal(130m, overview.PendingToPay);
        Assert.Equal(90m, overview.FixedCostsInMonth);

        Assert.NotNull(autoSales);
        Assert.Single(autoSales!.Items);
        Assert.Equal(FinancialEntrySource.AutoSale, autoSales.Items[0].Source);
        Assert.True(autoSales.Items[0].ReferenceId > 0);

        Assert.NotNull(autoStockPurchases);
        Assert.Single(autoStockPurchases!.Items);
        Assert.Equal(FinancialEntrySource.AutoStockPurchase, autoStockPurchases.Items[0].Source);
        Assert.True(autoStockPurchases.Items[0].ReferenceId > 0);

        Assert.NotNull(entries);
        Assert.Equal(5, entries!.Items.Count);
    }

    [Fact]
    public async Task ShouldOnlyAllowDeletingManualEntries()
    {
        await ResetDatabaseAsync();
        await SeedFinancialDataAsync();

        var client = factory.CreateClient();

        var autoSales = await client.GetFromJsonAsync<PagedResult<AutoFinancialEntryModel>>("/financial/entries/auto-sales?month=2026-03&page=1&pageSize=50");
        var allEntries = await client.GetFromJsonAsync<PagedResult<FinancialEntryModel>>("/financial/entries?month=2026-03&page=1&pageSize=50");
        Assert.NotNull(autoSales);
        Assert.NotNull(allEntries);

        var manualCreateResponse = await client.PostAsJsonAsync("/financial/entries", new
        {
            description = "Recebimento manual",
            type = "Income",
            category = "Outros",
            amount = 75.0m,
            date = "2026-03-15",
            status = "Pending",
            isFixed = false,
            notes = "Teste"
        });

        var manualEntry = await manualCreateResponse.Content.ReadFromJsonAsync<FinancialEntryModel>();
        Assert.Equal(HttpStatusCode.OK, manualCreateResponse.StatusCode);
        Assert.NotNull(manualEntry);

        var automaticEntryId = allEntries!.Items.Single(x => x.Source == FinancialEntrySource.AutoSale).Id;

        var deleteAutomaticResponse = await client.DeleteAsync($"/financial/entries/{automaticEntryId}");
        var deleteManualResponse = await client.DeleteAsync($"/financial/entries/{manualEntry!.Id}");

        Assert.Equal(HttpStatusCode.BadRequest, deleteAutomaticResponse.StatusCode);
        Assert.Equal(HttpStatusCode.NoContent, deleteManualResponse.StatusCode);
    }

    private async Task ResetDatabaseAsync()
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        db.FinancialEntries.RemoveRange(db.FinancialEntries);
        db.InventoryPurchaseMovements.RemoveRange(db.InventoryPurchaseMovements);
        db.FixedAccounts.RemoveRange(db.FixedAccounts);
        db.OrderItems.RemoveRange(db.OrderItems);
        db.Orders.RemoveRange(db.Orders);
        db.Products.RemoveRange(db.Products);
        db.Ingredients.RemoveRange(db.Ingredients);
        db.Supplies.RemoveRange(db.Supplies);
        db.Customers.RemoveRange(db.Customers);
        db.DeliveryAddresses.RemoveRange(db.DeliveryAddresses);

        await db.SaveChangesAsync();
    }

    private async Task SeedFinancialDataAsync()
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var address = DeliveryAddress.Create("Rua A", "10", "Centro", "Cidade", "ST", "12345-000", null);
        var customer = Customer.Create(1, "Maria", "1111", address);
        var product = Product.Create(1, "Bolo de pote", 200m, 10m, 1m);

        db.Customers.Add(customer);
        db.Products.Add(product);
        await db.SaveChangesAsync();

        var deliveredOrder = Order.Create(customer.Id, 2050, deliveryDate: new DateTime(2026, 03, 03, 0, 0, 0, DateTimeKind.Utc));
        deliveredOrder.AddItem(product.Id, 1, 200m, 80m);
        deliveredOrder.Status = OrderStatus.Delivered;

        db.Orders.Add(deliveredOrder);
        db.InventoryPurchaseMovements.Add(InventoryPurchaseMovement.Create(
            1,
            InventoryPurchaseItemType.Ingredient,
            77,
            "Açúcar",
            10m,
            15m,
            new DateOnly(2026, 03, 10),
            "Ingredientes",
            "Compra de ingrediente: açúcar"));
        db.FixedAccounts.Add(FixedAccount.Create(1, "Conta de luz", "Utilidades", 90m, 10, true));
        db.FinancialEntries.Add(FinancialEntry.CreateManual(
            1,
            "Recebimento de encomenda",
            FinancialEntryType.Income,
            "Outros",
            100m,
            new DateOnly(2026, 03, 20),
            FinancialEntryStatus.Pending,
            false,
            null));
        db.FinancialEntries.Add(FinancialEntry.CreateManual(
            1,
            "Despesa diversa",
            FinancialEntryType.Expense,
            "Operacional",
            40m,
            new DateOnly(2026, 03, 22),
            FinancialEntryStatus.Pending,
            false,
            null));

        await db.SaveChangesAsync();
    }
}
