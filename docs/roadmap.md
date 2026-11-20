# Roadmap do BolluFlow

**Regra:** tudo que existe hoje faz parte do plano gratuito. Enquanto isso, o foco é **polir o que já existe**. Funcionalidade nova entra primeiro aqui, como plano, e só é implementada quando for decidida.

Status: `ideia` → `decidido` → `em andamento` → `feito`.

## 1. Polimento do que já existe

### Pedidos
| Item | Status |
|---|---|
| Abrir o pedido recém-criado direto na tela de detalhes (hoje o modal fecha e a pessoa precisa achar o pedido na lista) | ideia |
| Criar pedido em uma tela só: cliente, itens, entrega e pagamento juntos (hoje são dois passos e um modal por item) | ideia |
| Cadastrar cliente sem sair do pedido, só com nome e WhatsApp | ideia |
| Endereço do cliente opcional (para retirada no local) | ideia |
| Atalhos de data de entrega ("Hoje", "Amanhã") e forma de pagamento em botões em vez de select | ideia |
| Avançar status direto pela lista de pedidos | ideia |

### Dashboard
| Item | Status |
|---|---|
| Virar uma tela "Hoje": entregas de hoje/amanhã, pedidos prontos, saldo do mês e alertas, conforme os módulos ligados | ideia |
| Remover gráficos de pizza/barra, que dizem pouco para um negócio pequeno | ideia |
| Para quem só usa Receitas: atalhos para custo dos produtos e produtos sem ficha técnica | ideia |

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

## 2. Funcionalidades futuras (candidatas a plano pago)

| Ideia | Status |
|---|---|
| Quadro de pedidos (kanban) por status, arrastando os cards (`@dnd-kit` já está no projeto) | ideia |
| Repetir um pedido anterior do mesmo cliente | ideia |
| Agenda/calendário de entregas | ideia |
| Relatórios avançados (lucro por produto, clientes mais frequentes) | ideia |
| Planos e cobrança por módulo | ideia |
