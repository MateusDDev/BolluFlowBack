using BolluFlow.Domain.Abstractions;

namespace BolluFlow.Domain.Errors;

public static class ProductComponentErrors
{
    public static readonly DomainError InvalidQuantity = new("INVALID_COMPONENT_QUANTITY", "quantityUsed deve ser maior que zero");
}
