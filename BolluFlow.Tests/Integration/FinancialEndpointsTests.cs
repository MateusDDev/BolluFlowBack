using System.Net;
using System.Net.Http.Json;
using BolluFlow.Application.Models;
using BolluFlow.Domain.Entities;
using BolluFlow.Domain.Enums;
using BolluFlow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BolluFlow.Tests.Integration;

public class FinancialEndpointsTests(DashboardWebApplicationFactory factory) : IClassFixture<DashboardWebApplicationFactory>
{
    [Fact]
    public async Task ShouldReturnProjectedBalanceWithAllPendingOrdersRegardlessOfMonthFilter()
    {
        await ResetDatabaseAsync();
        await SeedFinancialDataAsync();

        var client = factory.CreateClient();

        var response = await client.GetAsync("/financial/balance?month=3&year=2026");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var balance = await response.Content.ReadFromJsonAsync<FinancialBalanceModel>();
        Assert.NotNull(balance);

        Assert.Equal(100m, balance!.MonthIncome);
        Assert.Equal(50m, balance.MonthExpenses);
        Assert.Equal(900m, balance.TotalIncome);
        Assert.Equal(400m, balance.TotalExpenses);
        Assert.Equal(500m, balance.Balance);
        Assert.Equal(560m, balance.PendingOrdersTotal);
        Assert.Equal(1060m, balance.ProjectedBalance);
    }

    private async Task ResetDatabaseAsync()
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        db.FinancialEntries.RemoveRange(db.FinancialEntries);
        db.OrderItems.RemoveRange(db.OrderItems);
        db.Orders.RemoveRange(db.Orders);
        db.Customers.RemoveRange(db.Customers);
        db.DeliveryAddresses.RemoveRange(db.DeliveryAddresses);

        await db.SaveChangesAsync();
    }

    private async Task SeedFinancialDataAsync()
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var customer1 = Customer.Create(1, "Cliente 1", "1111", DeliveryAddress.Create("Rua A", "10", "Centro", "Cidade", "ST", "11111-111", null));
        var customer2 = Customer.Create(2, "Cliente 2", "2222", DeliveryAddress.Create("Rua B", "20", "Centro", "Cidade", "ST", "22222-222", null));
        db.Customers.AddRange(customer1, customer2);
        await db.SaveChangesAsync();

        var pendingFixedDiscountOrder = Order.Create(customer1.Id, 1, discount: 20, discountType: DiscountType.Fixed, shippingFee: 10);
        pendingFixedDiscountOrder.AddItem(1, 2, 100, 40); // total = 190

        var inProductionPercentDiscountOrder = Order.Create(customer1.Id, 2, discount: 10, discountType: DiscountType.Percentage, shippingFee: 0);
        inProductionPercentDiscountOrder.AddItem(1, 1, 300, 120); // total = 270
        inProductionPercentDiscountOrder.Status = OrderStatus.InProduction;

        var readyOrder = Order.Create(customer1.Id, 3, shippingFee: 0);
        readyOrder.AddItem(1, 1, 100, 30); // total = 100
        readyOrder.Status = OrderStatus.Ready;

        var deliveredOrder = Order.Create(customer1.Id, 4, shippingFee: 0);
        deliveredOrder.AddItem(1, 1, 999, 300); // ignored
        deliveredOrder.Status = OrderStatus.Delivered;

        var canceledOrder = Order.Create(customer1.Id, 5, shippingFee: 0);
        canceledOrder.AddItem(1, 1, 999, 300); // ignored
        canceledOrder.Status = OrderStatus.Canceled;

        var anotherUserPendingOrder = Order.Create(customer2.Id, 6, shippingFee: 0);
        anotherUserPendingOrder.AddItem(1, 1, 1000, 500); // ignored (different user)

        db.Orders.AddRange(
            pendingFixedDiscountOrder,
            inProductionPercentDiscountOrder,
            readyOrder,
            deliveredOrder,
            canceledOrder,
            anotherUserPendingOrder);

        db.FinancialEntries.AddRange(
            FinancialEntry.Create(FinancialEntryType.Income, 100, "March income", FinancialOriginType.Manual, null, "entry-1"),
            FinancialEntry.Create(FinancialEntryType.Expense, 50, "March expense", FinancialOriginType.Manual, null, "entry-2"),
            FinancialEntry.Create(FinancialEntryType.Income, 800, "Past income", FinancialOriginType.Manual, null, "entry-3"),
            FinancialEntry.Create(FinancialEntryType.Expense, 350, "Past expense", FinancialOriginType.Manual, null, "entry-4"));

        await db.SaveChangesAsync();

        await db.Database.ExecuteSqlRawAsync("""
            UPDATE tb_financial_entries
            SET created_at = CASE event_key
                WHEN 'entry-1' THEN '2026-03-10T00:00:00Z'
                WHEN 'entry-2' THEN '2026-03-11T00:00:00Z'
                WHEN 'entry-3' THEN '2026-02-05T00:00:00Z'
                WHEN 'entry-4' THEN '2026-02-06T00:00:00Z'
                ELSE created_at
            END
            """);
    }
}
