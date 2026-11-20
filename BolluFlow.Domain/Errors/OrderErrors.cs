using BolluFlow.Domain.Abstractions;

namespace BolluFlow.Domain.Errors;

public class OrderErrors
{
    public static readonly DomainError OrderIsNotOpen =
        new("ORDER_IS_NOT_OPEN", "O pedido precisa estar aberto para adicionar, editar ou remover itens");

    public static readonly DomainError ItemNotFound =
        new("ITEM_NOT_FOUND", "Item não encontrado");

    public static readonly DomainError InvalidStartTransition =
        new("ORDER_INVALID_START_TRANSITION", "A ordem só pode ser iniciada quando estiver pendente");

    public static readonly DomainError InvalidFinishTransition =
        new("ORDER_INVALID_FINISH_TRANSITION", "A ordem só pode ser finalizada quando estiver em produção");

    public static readonly DomainError InvalidDeliverTransition =
        new("ORDER_INVALID_DELIVER_TRANSITION", "A ordem só pode ser entregue quando estiver pronta");

    public static readonly DomainError InvalidDiscount =
        new("ORDER_INVALID_DISCOUNT", "Desconto deve ser maior ou igual a zero");

    public static readonly DomainError DiscountTypeRequired =
        new("ORDER_DISCOUNT_TYPE_REQUIRED", "Tipo de desconto é obrigatório quando desconto é informado");

    public static readonly DomainError InvalidShippingFee =
        new("ORDER_INVALID_SHIPPING_FEE", "Frete deve ser maior ou igual a zero");

    public static readonly DomainError InvalidPercentageDiscount =
        new("ORDER_INVALID_PERCENTAGE_DISCOUNT", "Desconto percentual deve estar entre 0 e 100");

    public static readonly DomainError NegativeTotalNotAllowed =
        new("ORDER_NEGATIVE_TOTAL", "O total do pedido não pode ser negativo");

    public static readonly DomainError QuickSaleWithoutItems =
        new("ORDER_QUICK_SALE_WITHOUT_ITEMS", "Adicione ao menos um produto para registrar a venda");
}
