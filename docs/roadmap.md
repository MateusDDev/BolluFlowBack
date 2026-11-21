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
| Botão de venda rápida sempre visível (atalho no topo ou flutuante) | ideia |

### Dashboard
| Item | Status |
|---|---|
| Topo "Hoje": entregas de hoje/amanhã, pedidos prontos e alertas de estoque, só quando houver algo pendente | feito |
| Métricas de vendas com seletor de período: faturamento e variação vs. período anterior, nº de vendas, ticket médio, lucro bruto e margem (usando o custo gravado no item do pedido), evolução dos últimos 12 meses, produtos mais vendidos | feito |
| Métricas vêm dos pedidos entregues (vendas); o Financeiro continua sendo o caixa (entradas, saídas, saldo). Mediana fica de fora | feito |
| Com poucas vendas, mostrar aviso em vez de médias/crescimento enganosos | feito |
| Remover gráficos de pizza/barra atuais | feito |
| Para quem só usa Receitas: produtos sem ficha técnica e de menor margem | feito |

### Estoque, conversões e cadastros (UX)
| Item | Status |
|---|---|
| Telas de ingredientes e insumos: cadastro com preço do pacote ("R$ 7,49 por 395 g"), quantidades em kg/L, "Registrar compra" (atualiza o custo do item e lança a despesa) e "Contar estoque" (o sistema calcula a diferença) | feito |
| Conversões viram "Embalagens" (lata, caixa) e aparecem como unidade na compra, na contagem e na quantidade usada na receita | feito |
| Cadastro de receita: rendimento com unidade (g, kg, ml, L, un), custo calculado na hora pelos ingredientes (sem "preço final" digitado), estoque mínimo salvo de verdade e ida direta para a aba de ingredientes | feito |
| Revisar os formulários de produto e cliente: campos, ordem, textos de ajuda | ideia |
| Custos sempre atuais: produto e receita usam o custo vigente dos ingredientes; a venda guarda o custo do momento | feito |

### Geral
| Item | Status |
|---|---|
| Primeiro uso: telas vazias que ensinam, card "primeiro produto" com lucro, dicas que aparecem uma vez e "?" nos conceitos | feito |
| Mensagem de boas-vindas pelo WhatsApp | ideia |
| Painel interno: onde os usuários param no checklist de primeiros passos | ideia |
| Campo de custo manual no produto (para quem não usa Receitas ver a margem) | ideia |
| Deixar claro nos formulários quais campos são obrigatórios e quais são opcionais (padrão único: marcar os opcionais com "(opcional)" e validar os obrigatórios com mensagem no campo) | ideia |
| Mostrar a margem resultante ao lado do markup na precificação ("markup 100% = margem 50%") | ideia |
| Revisar acentos e textos ("Preco de venda", "conversoes", "Salvar alteracoes", "Estoque minimo") | ideia |
| Usar toasts (sonner já está instalado) para sucesso/erro de forma consistente | ideia |
| Tela de carregamento no lugar da tela em branco enquanto a sessão carrega | ideia |
| Interruptor "Controlar estoque" na receita (o backend já aceita) | ideia |
| Alertas de estoque do dashboard incluírem receitas | ideia |
| Mensagens em inglês no `AdjustStockMovementHandler` | ideia |

### Cardápio digital
Hoje o pedido do cardápio entra no sistema, mas a empresa não é avisada, não sabe como o cliente vai pagar e o endereço é sempre obrigatório.

| Item | Status |
|---|---|
| Tela final com botão "Enviar pedido no WhatsApp" para a empresa, com o pedido já escrito (nº, itens, total, pagamento e troco, endereço, observação). Sem redirecionar sozinho | feito |
| Escolher a forma de pagamento no checkout (Dinheiro, Pix, Crédito, Débito), gravada no pedido | feito |
| Tempo de entrega do delivery (em minutos ou horas, da saída da doceria até o cliente) nas configurações do cardápio, mostrado no cardápio. Só informativo | feito |
| Dinheiro: "troco para quanto?" (opcional, gravado no pedido e mostrado nos detalhes); Pix: chave Pix da empresa nas configurações, mostrada com botão de copiar no checkout e na tela final | feito |
| Retirada ou entrega: o cliente escolhe "Retirar na doceria" e aí o endereço deixa de ser obrigatório. Na retirada o cardápio mostra o endereço da doceria, e o pedido fica marcado como retirada (detalhes, WhatsApp e PDF). A opção só aparece se a doceria tiver endereço | feito |
| Endereço da doceria nas configurações (rua, número, bairro, ponto de referência) | feito |
| Ponto de referência no endereço do cliente (cardápio e cadastro de cliente) | feito |
| Marcar retirada também no pedido criado pelo painel | ideia |
| Data desejada para a encomenda | decidido |
| Prévia do link ao compartilhar (nome, foto e descrição via `generateMetadata`) | decidido |
| Mais de uma foto por produto (até 5, a primeira é a capa; `ProductImage` no lugar de `ImageUrl`, carrossel no cardápio) | ideia |
| Sininho de notificações no topo do painel: todo pedido do cardápio gera uma notificação (não depende do cliente enviar o WhatsApp), com contador de não lidas, lista com link para o pedido e "marcar todas como lidas". Atualiza a cada 30 s com a aba visível. Base para outros tipos de aviso no futuro (estoque baixo, encomenda de amanhã) | feito |
| Notificação no celular/computador com o sistema fechado (Web Push com chaves VAPID no próprio back, sem serviço terceiro). Só envia se a pessoa não estiver usando o painel (a consulta do sininho serve de sinal de presença). No iPhone exige adicionar à tela inicial (PWA) | decidido (depois) |
| Sininho em tempo real com SignalR no lugar do polling de 30 s (notificação chega na hora). Manter polling espaçado (2 a 5 min) como reserva se a conexão cair. Conferir suporte a WebSocket na hospedagem; com mais de uma instância da API, precisa de backplane (Redis) | ideia (melhoria futura) |
| WhatsApp automático para a doceria pela API oficial (Meta cobra por mensagem) | ideia (plano pago) |
| Limpar notificações antigas (ex.: lidas há mais de 90 dias) | ideia |
| Carrinho salvo no navegador | ideia |
| Perfil da loja no topo: logo, capa, descrição, horário e "Loja fechada / não aceitando pedidos" | ideia |
| Categorias em abas fixas e busca | ideia |
| Taxa de entrega e pedido mínimo | ideia |
| Escolher no perfil quais formas de pagamento o cardápio aceita | ideia |
| Copiar link e gerar QR code do cardápio no painel | ideia |
| Página pública de acompanhamento do pedido | ideia |

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
| Componentes genéricos de item de estoque (ingredientes, insumos e receitas repetem célula de estoque, seletor de unidade, modal de conversões e de compra) | feito para ingredientes e insumos (`features/stockItems`); receitas ainda à parte |
| Formulários com `react-hook-form` + `zod` (zod já está instalado) no lugar de `useState` por campo | ideia |
| Quebrar componentes grandes (`OrderDetails` ~620 linhas, `OrderStatusActions` ~400) | ideia |
| Erro de lint já existente: `setState` dentro de efeito no `app/(private)/layout.tsx` (estado da sidebar) | ideia |
| Remover `src/app/dashboard/page.tsx` (página de exemplo do template shadcn, acessível em `/dashboard`) | ideia |

## 2. Funcionalidades futuras (candidatas a plano pago)

| Ideia | Status |
|---|---|
| Quadro de pedidos (kanban) por status, arrastando os cards (`@dnd-kit` já está no projeto) | ideia |
| Repetir um pedido anterior do mesmo cliente | ideia |
| Agenda/calendário de entregas | ideia |
| Relatórios avançados (lucro por produto, clientes mais frequentes) | ideia |
| Planos e cobrança por módulo | ideia |
