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
}
