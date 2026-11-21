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

    public static readonly DomainError ChangeForRequiresCash =
        new("ORDER_CHANGE_FOR_REQUIRES_CASH", "Troco só vale para pagamento em dinheiro");

    public static readonly DomainError ChangeForBelowTotal =
        new("ORDER_CHANGE_FOR_BELOW_TOTAL", "O valor para troco deve ser maior ou igual ao total do pedido");

    public static readonly DomainError InvalidShippingFee =
        new("ORDER_INVALID_SHIPPING_FEE", "Frete deve ser maior ou igual a zero");

    public static readonly DomainError InvalidPercentageDiscount =
        new("ORDER_INVALID_PERCENTAGE_DISCOUNT", "Desconto percentual deve estar entre 0 e 100");

    public static readonly DomainError NegativeTotalNotAllowed =
        new("ORDER_NEGATIVE_TOTAL", "O total do pedido não pode ser negativo");

    public static readonly DomainError DirectSaleWithoutItems =
        new("ORDER_DIRECT_SALE_WITHOUT_ITEMS", "Adicione ao menos um produto para registrar a venda");

    public static readonly DomainError OrderWithoutItems =
        new("ORDER_WITHOUT_ITEMS", "Pedido sem itens não pode ser iniciado");

    public static readonly DomainError InvalidCancelTransition =
        new("ORDER_NOT_POSSIBLE_TO_CANCEL", "Não é possível cancelar pedidos já entregues ou já cancelados");

    public static readonly DomainError InvalidReopenTransition =
        new("ORDER_NOT_CANCELED", "Apenas pedidos cancelados podem ser reabertos");

    public static readonly DomainError DirectSaleHasNoWorkflow =
        new("ORDER_DIRECT_SALE_HAS_NO_WORKFLOW", "Venda direta não passa por produção nem entrega. Para desfazer, exclua a venda");

    public static readonly DomainError OnlyDirectSaleCanBeDeleted =
        new("ORDER_ONLY_DIRECT_SALE_CAN_BE_DELETED", "Só vendas diretas podem ser excluídas. Encomendas devem ser canceladas");
}
