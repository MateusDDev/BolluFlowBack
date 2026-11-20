# Roadmap do BolluFlow

**Regra:** tudo que existe hoje faz parte do plano gratuito. Enquanto isso, o foco é **polir o que já existe**. Funcionalidade nova entra primeiro aqui, como plano, e só é implementada quando for decidida.

Status: `ideia` → `decidido` → `em andamento` → `feito`.

## 1. Polimento do que já existe

### Pedidos
| Item | Status |
|---|---|
| Criar pedido em uma tela só: cliente, itens, entrega e pagamento juntos (hoje são dois passos e um modal por item) | feito |
| **Venda rápida**: a mesma tela com "Entregar agora" cria o pedido já entregue (baixa de estoque e receita), sem status. Não é uma entidade separada: é um pedido, para relatórios e financeiro terem uma fonte só | feito |
| Cliente opcional no pedido (venda de balcão) e data de entrega opcional | feito |
| Abrir o pedido recém-criado direto na tela de detalhes | feito |
| Cadastrar cliente sem sair do pedido, só com nome e WhatsApp | feito |
| Endereço do cliente opcional (para retirada no local) | feito |
| Atalhos de data de entrega ("Hoje", "Amanhã") e forma de pagamento em botões em vez de select | feito |
| Avançar status direto pela lista de pedidos | ideia |

### Dashboard
| Item | Status |
|---|---|
| Topo "Hoje": entregas de hoje/amanhã, pedidos prontos e alertas de estoque, só quando houver algo pendente | decidido |
| Métricas de vendas com seletor de período: faturamento e variação vs. período anterior, nº de vendas, ticket médio, lucro bruto e margem (usando o custo gravado no item do pedido), evolução dos últimos 12 meses, produtos mais vendidos | decidido |
| Métricas vêm dos pedidos entregues (vendas); o Financeiro continua sendo o caixa (entradas, saídas, saldo). Mediana fica de fora | decidido |
| Com poucas vendas, mostrar aviso em vez de médias/crescimento enganosos | decidido |
| Remover gráficos de pizza/barra atuais | decidido |
| Para quem só usa Receitas: produtos sem ficha técnica e de menor margem | decidido |

### Estoque, conversões e cadastros (UX)
| Item | Status |
|---|---|
| Revisar a experiência das telas de ingredientes, insumos e receitas: estoque, compra e ajuste estão funcionais mas confusos | ideia |
| Simplificar as unidades de conversão (como criar, onde aparecem, o seletor "Unidade / referência" nas tabelas) | ideia |
| Revisar os formulários de cadastro (produto, ingrediente, insumo, receita, cliente): campos, ordem, textos de ajuda | ideia |

### Geral
| Item | Status |
|---|---|
| Revisar acentos e textos ("Preco de venda", "conversoes", "Salvar alteracoes", "Estoque minimo") | ideia |
| Usar toasts (sonner já está instalado) para sucesso/erro de forma consistente | ideia |
| Tela de carregamento no lugar da tela em branco enquanto a sessão carrega | ideia |
| Interruptor "Controlar estoque" na receita (o backend já aceita) | ideia |
| Alertas de estoque do dashboard incluírem receitas | ideia |
| Mensagens em inglês no `AdjustStockMovementHandler` | ideia |

### Segurança e base técnica
| Item | Status |
|---|---|
| Filtrar por usuário logado nos handlers que ainda buscam só por Id (filtro global do EF por `UserId`) | ideia |
| Remover o token do `localStorage` e usar só o cookie HttpOnly | ideia |
| Testes automatizados do ciclo de vida do pedido e da produção | ideia |
| `StockLevel` como value object (owned type do EF) no lugar do bloco de estoque repetido em Product/Ingredient/Supply/Recipe | ideia |
| Modelar venda e entrega separadas (Venda + Entrega opcional) — **gatilho:** só quando a encomenda ganhar dados próprios (sinal/entrada, endereço por pedido, agenda de produção) | ideia |

### Arquitetura do front
| Item | Status |
|---|---|
| Fábrica de query keys por feature (hoje são strings soltas como `['products']`, `['finance']`) | ideia |
| Componentes genéricos de item de estoque (ingredientes, insumos e receitas repetem célula de estoque, seletor de unidade, modal de conversões e de compra) | ideia |
| Formulários com `react-hook-form` + `zod` (zod já está instalado) no lugar de `useState` por campo | ideia |
| Quebrar componentes grandes (`OrderDetails` ~620 linhas, `OrderStatusActions` ~400) | ideia |
| Erro de lint já existente: `setState` dentro de efeito no `app/(private)/layout.tsx` (estado da sidebar) | ideia |

## 2. Funcionalidades futuras (candidatas a plano pago)

| Ideia | Status |
|---|---|
| Quadro de pedidos (kanban) por status, arrastando os cards (`@dnd-kit` já está no projeto) | ideia |
| Repetir um pedido anterior do mesmo cliente | ideia |
| Agenda/calendário de entregas | ideia |
| Relatórios avançados (lucro por produto, clientes mais frequentes) | ideia |
| Planos e cobrança por módulo | ideia |
