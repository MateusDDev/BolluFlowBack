using BolluFlow.Domain.Abstractions;

namespace BolluFlow.Domain.Errors;

public class OrderErrors
{
    public static readonly DomainError RecipeInactive =
        new("RECIPE_INACTIVE", "A receita precisa estar ativa para ser adicionada ao pedido");
    
    public static readonly DomainError OrderIsNotOpen =
        new("ORDER_IS_NOT_OPEN", "O pedido precisa estar aberto para adicionar, editar ou remover itens");
    
    public static readonly DomainError ItemNotFound =
        new("ITEM_NOT_FOUND", "Item não encontrado");
}