using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Errors;
using BolluFlow.Domain.Enums;
using BolluFlow.Infrastructure.Auth;
using BolluFlow.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using System.Globalization;

namespace BolluFlow.Application.Features.Reports.CustomerOrderReport;

public class CustomerOrderReportHandler(ApplicationDbContext db, CurrentUser user) : IRequestHandler<CustomerOrderReportQuery, ResultOf<byte[]>>
{
    public async Task<ResultOf<byte[]>> Handle(
        CustomerOrderReportQuery request,
        CancellationToken cancellationToken)
    {
        var order = await db.Orders
            .Where(x => x.Id == request.OrderId && x.UserId == user.Id)
            .Select(x => new CustomerOrderReportResult
            {
                Id = x.Id,
                Number = x.Number,
                StoreName = x.User.Name,
                CustomerName = x.Customer!.Name,
                DeliveryDate = x.DeliveryDate,
                Subtotal = x.Items.Sum(i => i.Quantity * i.UnitPrice),
                Discount = x.Discount,
                DiscountType = x.DiscountType,
                DiscountValue = ((x.Discount == null || x.DiscountType == null)
                    ? 0
                    : x.DiscountType == DiscountType.Percentage
                        ? x.Items.Sum(i => i.Quantity * i.UnitPrice) * (x.Discount.Value / 100)
                        : x.Discount.Value),
                ShippingFee = x.ShippingFee,
                TotalValue = x.Items.Sum(i => i.Quantity * i.UnitPrice)
                    - ((x.Discount == null || x.DiscountType == null)
                        ? 0
                        : x.DiscountType == DiscountType.Percentage
                            ? x.Items.Sum(i => i.Quantity * i.UnitPrice) * (x.Discount.Value / 100)
                            : x.Discount.Value)
                    + x.ShippingFee,
                PaymentMethod = x.PaymentMethod,
                Note = x.Note,
                Items = x.Items.Select(i => new CustomerOrderReportResultItem
                {
                    ProductName = i.Product.Name,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                }).ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);

        return order is null
            ? ResultOf<byte[]>.BadRequest(CommonErrors.OrderNotFound)
            : ResultOf<byte[]>.Success(Generate(order));
    }

    private static byte[] Generate(CustomerOrderReportResult order)
    {
        var ptBR = new CultureInfo("pt-BR");

        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A6);
                page.Margin(4);

                page.DefaultTextStyle(t =>
                    t.FontFamily("Courier New")
                        .FontSize(10));

                page.Content()
                    .Border(0.8f)
                    .BorderColor(Colors.Grey.Darken2)
                    .Padding(8)
                    .Column(main =>
                    {
                        main.Spacing(0);

                        main.Item().PaddingBottom(6).Column(col =>
                        {
                            col.Spacing(2);

                            col.Item().AlignCenter()
                                .Text(order.StoreName)
                                .Bold()
                                .FontSize(12);

                            col.Item().AlignCenter()
                                .Text($"PEDIDO #{order.Number}")
                                .Bold()
                                .FontSize(14);

                            col.Item().AlignCenter()
                                .Text(order.CustomerName ?? "Cliente")
                                .Bold()
                                .FontSize(11);

                            if (order.DeliveryDate.HasValue)
                            {
                                col.Item().AlignCenter()
                                    .Text(order.DeliveryDate.Value.ToString("dd/MM/yyyy"))
                                    .FontSize(9)
                                    .FontColor(Colors.Grey.Darken1);
                            }
                        });

                        main.Item().PaddingBottom(6)
                            .LineHorizontal(0.5f)
                            .LineColor(Colors.Grey.Lighten1);

                        main.Item().PaddingBottom(6).Column(col =>
                        {
                            col.Spacing(2);

                            col.Item().Row(row =>
                            {
                                row.RelativeItem().Text("ITEM").Bold().FontSize(9);
                                row.ConstantItem(26).AlignRight().Text("QTD").Bold().FontSize(9);
                                row.ConstantItem(50).AlignRight().Text("PREÇO").Bold().FontSize(9);
                            });

                            foreach (var item in order.Items)
                            {
                                col.Item().Row(row =>
                                {
                                    row.RelativeItem().Text(item.ProductName).FontSize(10);
                                    row.ConstantItem(26).AlignRight().Text($"x{item.Quantity}").FontSize(9);
                                    row.ConstantItem(50).AlignRight().Text(item.UnitPrice.ToString("C", ptBR)).FontSize(9);
                                });

                                col.Item().AlignRight()
                                    .Text((item.Quantity * item.UnitPrice).ToString("C", ptBR))
                                    .FontSize(9)
                                    .FontColor(Colors.Grey.Darken1);
                            }
                        });

                        main.Item().PaddingBottom(6)
                            .LineHorizontal(0.5f)
                            .LineColor(Colors.Grey.Lighten1);

                        main.Item().PaddingBottom(6).Column(col =>
                        {
                            col.Spacing(2);

                            col.Item().Row(row =>
                            {
                                row.RelativeItem().Text("Subtotal").FontSize(9);
                                row.AutoItem().Text(order.Subtotal.ToString("C", ptBR)).FontSize(9);
                            });

                            if (order.DiscountValue > 0)
                            {
                                var discountLabel = order.DiscountType == DiscountType.Percentage
                                    ? $"Desconto ({order.Discount:0.##}%)"
                                    : "Desconto";

                                col.Item().Row(row =>
                                {
                                    row.RelativeItem().Text(discountLabel).FontSize(9).FontColor(Colors.Red.Medium);
                                    row.AutoItem().Text($"-{order.DiscountValue.ToString("C", ptBR)}")
                                        .FontSize(9).FontColor(Colors.Red.Medium);
                                });
                            }

                            if (order.ShippingFee > 0)
                            {
                                col.Item().Row(row =>
                                {
                                    row.RelativeItem().Text("Frete").FontSize(9);
                                    row.AutoItem().Text(order.ShippingFee.ToString("C", ptBR)).FontSize(9);
                                });
                            }

                            col.Item().LineHorizontal(0.5f).LineColor(Colors.Grey.Lighten1);

                            col.Item().Row(row =>
                            {
                                row.RelativeItem().Text("TOTAL").Bold().FontSize(11);
                                row.AutoItem().Text(order.TotalValue.ToString("C", ptBR)).Bold().FontSize(11);
                            });

                            if (order.PaymentMethod.HasValue)
                            {
                                col.Item().Text($"Pagamento: {ReportLabels.GetPaymentMethodLabel(order.PaymentMethod.Value)}")
                                    .FontSize(9)
                                    .FontColor(Colors.Grey.Darken1);
                            }
                        });

                        if (!string.IsNullOrWhiteSpace(order.Note))
                        {
                            main.Item().PaddingBottom(6)
                                .LineHorizontal(0.5f)
                                .LineColor(Colors.Grey.Lighten1);

                            main.Item().PaddingBottom(6).Column(col =>
                            {
                                col.Spacing(2);

                                col.Item()
                                    .Text($"OBS: {order.Note}")
                                    .FontSize(9)
                                    .FontColor(Colors.Grey.Darken1);
                            });
                        }

                        main.Item().PaddingTop(8).AlignCenter()
                            .Text("Obrigado pela preferência!")
                            .FontSize(9)
                            .FontColor(Colors.Grey.Darken1)
                            .Italic();
                    });
            });
        }).GeneratePdf();
    }
}
