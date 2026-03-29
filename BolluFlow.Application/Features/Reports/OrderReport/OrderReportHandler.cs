using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Configuration;
using BolluFlow.Application.Errors;
using BolluFlow.Domain.Enums;
using BolluFlow.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using System.Globalization;

namespace BolluFlow.Application.Features.Reports.OrderReport;

public class OrderReportHandler(ApplicationDbContext db, IOptions<FrontOptions> frontOptions) : IRequestHandler<OrderReportQuery, ResultOf<byte[]>>
{
    private readonly string _frontUrl = frontOptions.Value.Url;
    
    public async Task<ResultOf<byte[]>> Handle(
        OrderReportQuery request,
        CancellationToken cancellationToken)
    {
        var order = await db.Orders
            .Select(x => new OrderReportResult
            {
                Id = x.Id,
                Number = x.Number,
                DeliveryDate = x.DeliveryDate,
                CustomerName = x.Customer.Name,
                CustomerWhatsapp = x.Customer.Whatsapp,
                CustomerStreet = x.Customer.DeliveryAddress.Street,
                CustomerNumber = x.Customer.DeliveryAddress.Number,
                CustomerNeighborhood = x.Customer.DeliveryAddress.Neighborhood,
                TotalValue = x.Items.Sum(i => i.Quantity * i.UnitPrice),
                PaymentMethod = x.PaymentMethod,
                Items = x.Items.Select(i => new OrderReportResultItem
                {
                    Quantity = i.Quantity,
                    ProductName = i.Product.Name
                }).ToList(),
                Note = x.Note
            })
            .FirstOrDefaultAsync(x => x.Id == request.OrderId, cancellationToken);

        return order is null 
            ? ResultOf<byte[]>.BadRequest(CommonErrors.OrderNotFound) 
            : ResultOf<byte[]>.Success(Generate(order));
    }
    
    private byte[] Generate(OrderReportResult order)
    {
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
     
                        // ── CABEÇALHO ──────────────────────────────
                        main.Item().PaddingBottom(6).Column(col =>
                        {
                            col.Spacing(2);
     
                            col.Item().AlignCenter()
                                .Text($"COMANDA #{order.Number}")
                                .Bold()
                                .FontSize(14);
     
                            col.Item().AlignCenter()
                                .Text(order.CustomerName)
                                .Bold()
                                .FontSize(11);
     
                            col.Item().AlignCenter()
                                .Text(order.CustomerWhatsapp)
                                .FontSize(9)
                                .FontColor(Colors.Grey.Darken1);
     
                            if (order.DeliveryDate.HasValue)
                            {
                                col.Item().AlignCenter()
                                    .Text($"Entrega: {order.DeliveryDate.Value.ToString("dd/MM/yyyy")}")
                                    .FontSize(9)
                                    .FontColor(Colors.Grey.Darken1);
                            }
                        });
     
                        // ── SEPARADOR ──────────────────────────────
                        main.Item().PaddingBottom(6)
                            .LineHorizontal(0.5f)
                            .LineColor(Colors.Grey.Lighten1);
     
                        // ── ENDEREÇO ───────────────────────────────
                        main.Item().PaddingBottom(6).Column(address =>
                        {
                            address.Spacing(1);
     
                            address.Item()
                                .Text($"{order.CustomerStreet}, {order.CustomerNumber}")
                                .FontSize(9);
     
                            address.Item()
                                .Text(order.CustomerNeighborhood)
                                .FontSize(9)
                                .FontColor(Colors.Grey.Darken1);
                        });
     
                        // ── SEPARADOR ──────────────────────────────
                        main.Item().PaddingBottom(6)
                            .LineHorizontal(0.5f)
                            .LineColor(Colors.Grey.Lighten1);
     
                        // ── ITENS ──────────────────────────────────
                        main.Item().PaddingBottom(6).Column(col =>
                        {
                            col.Spacing(3);
     
                            foreach (var item in order.Items)
                            {
                                col.Item().Row(row =>
                                {
                                    row.RelativeItem()
                                        .Text(item.ProductName)
                                        .FontSize(10);
     
                                    row.ConstantItem(28)
                                        .AlignRight()
                                        .Text($"x{item.Quantity}")
                                        .Bold()
                                        .FontSize(10);
                                });
                            }
                        });
     
                        // ── SEPARADOR ──────────────────────────────
                        main.Item().PaddingBottom(6)
                            .LineHorizontal(0.5f)
                            .LineColor(Colors.Grey.Lighten1);
     
                        // ── TOTAL E PAGAMENTO ──────────────────────
                        main.Item().PaddingBottom(6).Column(col =>
                        {
                            col.Spacing(2);
     
                            col.Item().Row(row =>
                            {
                                row.RelativeItem()
                                    .Text("TOTAL")
                                    .Bold()
                                    .FontSize(10);
     
                                row.AutoItem()
                                    .Text(order.TotalValue.ToString("C", new CultureInfo("pt-BR")))
                                    .Bold()
                                    .FontSize(10);
                            });
     
                            if (order.PaymentMethod.HasValue)
                            {
                                col.Item()
                                    .Text($"Pagamento: {GetPaymentMethodLabel(order.PaymentMethod.Value)}")
                                    .FontSize(9)
                                    .FontColor(Colors.Grey.Darken1);
                            }
                        });
     
                        // ── OBSERVAÇÕES (se houver) ─────────────────
                        if (!string.IsNullOrWhiteSpace(order.Note))
                        {
                            main.Item().PaddingBottom(6)
                                .LineHorizontal(0.5f)
                                .LineColor(Colors.Grey.Lighten1);
     
                            main.Item().PaddingBottom(6).Column(col =>
                            {
                                col.Spacing(2);
     
                                col.Item()
                                    .Text("OBS:")
                                    .Bold()
                                    .FontSize(9);
     
                                col.Item()
                                    .Text(order.Note)
                                    .FontSize(9)
                                    .FontColor(Colors.Grey.Darken1);
                            });
                        }
     
                        // ── QR CODE ────────────────────────────────
                        main.Item().AlignCenter().Column(qr =>
                        {
                            qr.Item()
                                .PaddingTop(4)
                                .LineHorizontal(0.5f)
                                .LineColor(Colors.Grey.Lighten1);
     
                            var url = $"{_frontUrl}/orders/{order.Id}";
     
                            qr.Item()
                                .PaddingTop(6)
                                .Height(65)
                                .AlignCenter()
                                .BarcodeQr(url);
     
                            qr.Item()
                                .AlignCenter()
                                .Text("Abrir pedido")
                                .FontSize(8)
                                .FontColor(Colors.Grey.Darken2);
                        });
                    });
            });
        }).GeneratePdf();
    }

    
    private static string GetPaymentMethodLabel(PaymentMethod paymentMethod)
    {
        return paymentMethod switch
        {
            PaymentMethod.Cash => "Dinheiro",
            PaymentMethod.Pix => "Pix",
            PaymentMethod.CreditCard => "Cartão de crédito",
            PaymentMethod.DebitCard => "Cartão de débito",
            _ => paymentMethod.ToString()
        };
    }
}
