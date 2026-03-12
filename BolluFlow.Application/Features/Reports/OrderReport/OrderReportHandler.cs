using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Configuration;
using BolluFlow.Application.Errors;
using BolluFlow.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using QuestPDF.Fluent;
using QuestPDF.Helpers;

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
                    .Padding(6)
                    .Column(main =>
                    {
                        main.Spacing(6);
                        
                        main.Item().Column(col =>
                        {
                            col.Item().AlignCenter()
                                .Text($"COMANDA #{order.Number}")
                                .Bold()
                                .FontSize(14);

                            col.Item().AlignCenter()
                                .Text(order.CustomerName)
                                .FontSize(10);

                            col.Item().AlignCenter()
                                .Text(order.DeliveryDate.HasValue
                                    ? order.DeliveryDate.Value.ToString("dd/MM/yyyy")
                                    : "Data de entrega não definida")
                                .FontSize(9)
                                .FontColor(Colors.Grey.Darken1);

                            col.Item()
                                .PaddingTop(2)
                                .LineHorizontal(0.5f)
                                .LineColor(Colors.Grey.Lighten1);
                        });
                        
                        main.Item().Column(col =>
                        {
                            col.Spacing(4);

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

                            if (!string.IsNullOrWhiteSpace(order.Note))
                            {
                                col.Item()
                                    .PaddingTop(4)
                                    .LineHorizontal(0.5f)
                                    .LineColor(Colors.Grey.Lighten1);

                                col.Item()
                                    .Text("OBS:")
                                    .Bold()
                                    .FontSize(9);

                                col.Item()
                                    .Text(order.Note)
                                    .FontSize(9);
                            }
                        });
                        
                        main.Item().AlignCenter().Column(qr =>
                        {
                            qr.Item()
                                .PaddingTop(4)
                                .AlignCenter()
                                .Text("———")
                                .FontSize(8);

                            var url = $"{_frontUrl}/orders/{order.Id}";

                            qr.Item()
                                .Height(70)
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
}
