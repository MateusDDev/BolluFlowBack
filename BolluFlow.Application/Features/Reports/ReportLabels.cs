using BolluFlow.Domain.Enums;

namespace BolluFlow.Application.Features.Reports;

public static class ReportLabels
{
    public static string GetPaymentMethodLabel(PaymentMethod paymentMethod)
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
