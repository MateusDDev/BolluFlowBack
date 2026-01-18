using BolluFlow.Domain.Abstractions;

namespace BolluFlow.Application.Errors;

public class CommonErrors
{
    public static readonly DomainError CustomerNotFound =
        new("CUSTOMER_NOT_FOUND", "Cliente não encontrado");

    public static readonly DomainError RecipeNotFound =
        new("RECIPE_NOT_FOUND", "Receita não encontrada");
    
    public static readonly DomainError UserNotFound =
        new("USER_NOT_FOUND", "Usuário não encontrado");

    public static readonly DomainError OrderNotFound =
        new("ORDER_NOT_FOUND", "Pedido não encontrado");
    
    public static readonly DomainError IngredientNotFound =
        new("INGREDIENT_NOT_FOUND", "Ingrediente não encontrado");
    
    public static readonly DomainError IngredientConversionNotFound =
        new("INGREDIENT_NOT_FOUND", "Conversão para o ingrediente não encontrada");
    
    public static readonly DomainError RecipeIngredientNotFound =
        new("RECIPE_INGREDIENT_NOT_FOUND", "Ingrediente não encontrado");
}