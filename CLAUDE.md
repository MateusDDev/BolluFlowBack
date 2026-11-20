# BolluFlow API

Backend do BolluFlow: gestão para pequenos negócios de confeitaria/alimentação (pedidos, receitas, ficha técnica de produtos, estoque, financeiro e cardápio público). O front fica em `../BolluFlowFront` (Next.js).

## Fase atual do produto

Tudo que existe hoje é o plano gratuito e o foco é **polir o que já existe**. Funcionalidade nova não é implementada direto: entra primeiro em [docs/roadmap.md](docs/roadmap.md) como plano. O roadmap também lista o backlog de polimento.

## Stack

- .NET 10, ASP.NET Core Controllers, MediatR 12 (CQRS: um Command/Query + Handler por caso de uso)
- EF Core 10 + Npgsql (PostgreSQL), `UseSnakeCaseNamingConvention()`
- BCrypt, QuestPDF (relatórios PDF)
- **Sem biblioteca de mapeamento** (Mapster foi removido): cada model de resposta tem um `static From(entidade)` em `Application/Models`. Não reintroduza Mapster/AutoMapper.
- JWT lido do cookie `access_token` (ou header `Authorization: Bearer`)
- Sem projeto de testes.

## Comandos

```bash
dotnet build BolluFlow.sln
dotnet run --project BolluFlow.Api            # porta 5143 (env PORT), Swagger em /swagger no Development
dotnet ef migrations add <Nome> --project BolluFlow.Infrastructure --startup-project BolluFlow.Api
dotnet ef database update --project BolluFlow.Infrastructure --startup-project BolluFlow.Api
```

Migrations NÃO rodam no startup (commit 0432208) — aplique manualmente.
Deploy: push na `main` → GitHub Actions builda e publica a imagem Docker (`.github/workflows/docker-publish.yml`).

## Estrutura

```
BolluFlow.Domain/          Entidades ricas (setters privados, factory Create, validação lançando DomainException),
                           Enums, Errors (DomainError por agregado), Services/RecipePricingService
BolluFlow.Infrastructure/  ApplicationDbContext, Configurations (IEntityTypeConfiguration), Migrations,
                           Auth/CurrentUser (UserId do claim NameIdentifier), Helpers/JwtHelper
BolluFlow.Application/     Features/<Módulo>/<CasoDeUso>/{Command|Query, Handler, Result?}
                           Models (DTOs de resposta), Services (ProductComponentCostService, SlugService),
                           Abstractions (Result, ResultOf<T>, OptionalField)
BolluFlow.Api/             Controllers (finos: montam o command e chamam mediator.Send),
                           Extensions/ResultToAction (Result → ActionResult), Program.cs
```

Dependências: Api → Application → Infrastructure → Domain. **Os handlers usam `ApplicationDbContext` direto** (sem repositórios); não há interface entre Application e Infrastructure.

## Commits

- Padrão Conventional Commits, mensagem curta **em inglês**: `feat: add quick sale button`, `fix: ...`, `refactor: ...`, `docs: ...`, `chore: ...`.
- Um commit por mudança lógica, feito depois de testada (mesmo que só localmente, ponta a ponta).

## Convenções

- **Código em inglês**: classes, métodos, propriedades, variáveis, enums, rotas, nomes de migration e colunas (ex.: `TrackStock`, não `ControlaEstoque`). Só textos para o usuário (mensagens de `DomainError`, descrições de lançamentos, labels de relatório) ficam em pt-BR.
- **Não escreva comentários no código** (nem `//`, nem `/* */`, nem XML doc `///`). Nomes claros substituem comentários; contexto vai na mensagem de commit/PR.
- Handler retorna `Result` / `ResultOf<T>` (`Success`, `Created`, `NotFound`, `BadRequest`, `Unprocessable`, `Conflict`); nunca lança para o controller. `DomainException` é capturada no handler e vira `BadRequest`/`Unprocessable`.
- Corpo de erro: `{ message, code, details }`. Códigos em UPPER_SNAKE (`INSUFFICIENT_STOCK`, `ORDER_NOT_CANCELED`...). Mensagens em pt-BR.
- Operações que tocam vários agregados abrem transação explícita (`db.Database.BeginTransactionAsync`).
- Idempotência por `EventKey` em `StockMovement` e `FinancialEntry` (ex.: `order_delivered:{orderId}`, `stock_purchase:{movementId}`, `recipe_production:{productionId}:ingredient:{id}`).
- Enums serializados como string (`JsonStringEnumConverter`).
- Multi-tenant por `UserId` (o "usuário" é a empresa). `Order` não tem `UserId` — o dono vem de `Order.Customer.UserId`. Vários handlers ainda buscam por Id sem filtrar pelo usuário atual (ex.: StartOrder, CancelOrder, UpdateRecipe); ao mexer neles, adicione o filtro.

## Módulos e como se ligam

| Módulo | Entidades | Toca em |
|---|---|---|
| Auth / Profile | User (Slug, Whatsapp, WhatsappMessageTemplate, IndirectCostRate) | — |
| Customers | Customer, DeliveryAddress | — |
| Ingredients | Ingredient (+ IngredientConversion) — custo + `StockQuantity` | Financeiro (compra inicial) |
| Supplies (insumos/embalagens) | Supply (+ SupplyConversion) | Financeiro |
| Recipes | Recipe, RecipeIngredient, RecipeConversion, RecipeProduction | Ingredients (custo e baixa de estoque na produção) |
| Products | Product, ProductComponent (Recipe/Ingredient/Supply), ProductProduction | Recipes/Ingredients/Supplies (custo e baixa na produção) |
| Orders | Order, OrderItem (snapshot de UnitPrice/UnitCost) | Products (preço, custo, estoque), Customers, Financeiro |
| Stock | StockMovement (ItemType + ItemId polimórfico), UserStockUnitPreference | Ingredients/Supplies, Financeiro |
| Finance | FinancialEntry (Origin: Order, IngredientPurchase, SupplyPurchase, manual) | Estoque (excluir lançamento pode reverter estoque) |
| Menu (público, `/cardapio/{slug}`) | — | Users, Products, Customers, Orders |
| Dashboard / Reports | — (só leitura) | Orders, estoque de tudo |

Fluxo de pedido: `Pending → (start) InProduction → (finish) Ready → (deliver) Delivered`; `cancel` (opcional `revertStock`) e `reopen` (Canceled → Pending).
- `StartOrder` valida e **debita** `Product.StockQuantity`.
- `DeliverOrder` **debita de novo** o estoque do produto e cria a receita (`FinancialEntry` Income).
- Produzir Produto debita componentes e soma estoque do produto; produzir Receita debita ingredientes e soma estoque da receita.
