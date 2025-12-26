using BolluFlow.Domain.Abstractions;

namespace BolluFlow.Domain.Errors;

public class OrderItemErrors
{
    public static readonly DomainError InvalidQuantity =
        new("INVALID_QUANTITY", "A quantidade deve ser maior que zero");
}