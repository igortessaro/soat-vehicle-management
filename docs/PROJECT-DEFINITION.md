# Definição inicial do projeto

> Documento de referência para as próximas etapas do Trabalho Substitutivo de Tech Challenge — Pós-Tech SOAT, Fase 4.

## 1. Objetivo

Construir uma plataforma de revenda de veículos com uma API para posterior integração com um frontend. A solução deve permitir o cadastro e a edição de veículos, a compra de veículos e a comunicação com um processador de pagamentos.

O trabalho exige dois serviços com responsabilidades separadas, bancos de dados segregados, testes automatizados, cobertura mínima de 80% e CI/CD acionado por merge na branch principal.

## 2. Fonte do escopo

O escopo original está descrito no arquivo `Fase 4 - Trabalho Reposição Tech Challenge SOAT.pdf`, fornecido pela disciplina.

O PDF exige:

- Cadastro de veículo para venda: marca, modelo, ano, cor e preço.
- Edição dos dados do veículo.
- Venda de um veículo, com CPF do comprador e data da venda.
- Webhook para informar pagamento efetuado ou cancelado.
- Listagem de veículos à venda por preço crescente.
- Listagem de veículos vendidos por preço crescente.
- Serviço de venda isolado para listagens e compras, com banco próprio.
- Comunicação HTTP entre o serviço principal e o serviço de vendas.
- Dois repositórios, com README, código funcional, testes e CI/CD.
- Deploy automatizado após merge na branch principal.
- Vídeo com demonstração ponta a ponta, infraestrutura, segregação dos serviços, testes e cobertura.

## 3. Repositórios

### `soat-vehicle-management`

Serviço principal da solução:

- Cadastro e edição de veículos.
- Fonte oficial dos dados cadastrais do veículo.
- Controle do estado do veículo.
- Banco de dados próprio.
- API HTTP para o serviço de vendas.

### `soat-vehicle-sales`

Serviço de vendas:

- Listagem de veículos disponíveis e vendidos.
- Criação de uma venda.
- Controle do comprador, da venda e do pagamento.
- Webhook de pagamento.
- Banco de dados próprio.
- Comunicação HTTP com `soat-vehicle-management`.

## 4. Decisões de domínio

### 4.1 Estados do veículo

Foi escolhido o modelo mínimo abaixo:

| Estado | Significado |
|---|---|
| `AVAILABLE` | Veículo disponível para venda |
| `RESERVED` | Veículo reservado durante uma compra pendente |
| `SOLD` | Pagamento confirmado e venda efetivada |
| `DELIVERED` | Veículo entregue ao comprador |

O estado do veículo não deve ser usado para representar diretamente o estado do pagamento ou da venda.

### 4.2 Transições do veículo

```text
AVAILABLE -> RESERVED
RESERVED  -> AVAILABLE
RESERVED  -> SOLD
SOLD      -> DELIVERED
```

Regras:

- Só um veículo `AVAILABLE` pode iniciar uma compra.
- Um veículo `RESERVED` não aparece na listagem de disponíveis.
- Pagamento cancelado libera o veículo e o retorna para `AVAILABLE`.
- Pagamento confirmado altera o veículo para `SOLD`.
- Veículos `SOLD` ou `DELIVERED` não podem ser vendidos novamente.
- Uma nova compra após cancelamento cria uma nova venda e um novo pagamento.

### 4.3 Estados da venda

```text
PENDING_PAYMENT -> PAID
PENDING_PAYMENT -> CANCELED
PAID            -> COMPLETED
```

| Estado | Significado |
|---|---|
| `PENDING_PAYMENT` | Compra criada e aguardando confirmação |
| `PAID` | Pagamento confirmado; venda efetivada |
| `CANCELED` | Pagamento cancelado; reserva liberada |
| `COMPLETED` | Entrega concluída |

Nesta primeira versão, uma venda pode possuir no máximo um pagamento confirmado. Não haverá múltiplas tentativas de pagamento dentro da mesma venda.

### 4.4 Estados do pagamento

```text
PENDING -> PAID
PENDING -> CANCELED
```

Um pagamento confirmado não volta para pendente nem é cancelado pelo fluxo normal. Um pagamento cancelado não é confirmado posteriormente. Para uma nova tentativa, deve ser criada uma nova venda.

## 5. Código do pagamento

O serviço de vendas deve gerar o código do pagamento na criação da venda.

Decisão inicial:

- Utilizar um UUID aleatório.
- Não incluir CPF ou informações sensíveis no código.
- Garantir unicidade com restrição/índice no banco.
- Retornar o código ao consumidor da API de criação da venda.
- Usar o mesmo código na integração com o processador de pagamentos e no webhook.

Exemplo:

```text
550e8400-e29b-41d4-a716-446655440000
```

## 6. Webhook de pagamento

Endpoint inicial proposto:

```http
POST /api/v1/webhooks/payments
```

Payload inicial:

```json
{
  "eventId": "evt_123456",
  "paymentCode": "550e8400-e29b-41d4-a716-446655440000",
  "status": "PAID",
  "occurredAt": "2026-10-05T20:00:00Z"
}
```

Valores aceitos para `status`:

- `PAID`
- `CANCELED`

O webhook deve ser idempotente. O serviço deve persistir o `eventId` e, ao receber o mesmo evento novamente, retornar sucesso sem repetir os efeitos de negócio.

Efeitos esperados:

| Evento | Serviço de vendas | Serviço principal |
|---|---|---|
| `PAID` | Pagamento e venda passam para pagos | Veículo passa para `SOLD` |
| `CANCELED` | Pagamento e venda passam para cancelados | Veículo retorna para `AVAILABLE` |

## 7. Responsabilidade e dados

### Banco do serviço principal

Fonte oficial dos dados do veículo:

```text
Vehicle
- Id
- Brand
- Model
- Year
- Color
- Price
- Status
- CreatedAt
- UpdatedAt
```

### Banco do serviço de vendas

Dados de venda e pagamento:

```text
Sale
- Id
- VehicleId
- BuyerCpf
- Status
- PaymentCode
- CreatedAt
- UpdatedAt
- PaidAt
- CanceledAt
```

O serviço de vendas poderá manter uma projeção local dos dados necessários para listagem:

```text
VehicleListing
- VehicleId
- Brand
- Model
- Year
- Color
- Price
- VehicleStatus
- SyncedAt
```

Essa projeção não substitui o cadastro oficial do serviço principal. Ela existe para que as listagens sejam atendidas pelo serviço de vendas e pelo seu banco isolado, conforme exigido pelo trabalho.

O cadastro de vendedor não faz parte do escopo atual, pois não é exigido pelo PDF.

## 8. Fluxos principais

### Cadastro

```text
Frontend -> Serviço principal -> Banco principal
```

### Compra

```text
Frontend
  -> Serviço de vendas
  -> Serviço principal: reserva o veículo
  -> Banco de vendas: cria venda pendente e código de pagamento
```

### Pagamento confirmado

```text
Processador de pagamento
  -> Webhook do serviço de vendas
  -> Serviço principal: marca veículo como vendido
  -> Banco de vendas: marca venda como paga
```

### Pagamento cancelado

```text
Processador de pagamento
  -> Webhook do serviço de vendas
  -> Serviço principal: libera o veículo
  -> Banco de vendas: cancela a venda
```

As operações entre os serviços devem ser idempotentes e devem tratar falhas de comunicação explicitamente.

## 9. Contrato inicial de endpoints

### Serviço principal

```http
POST /api/v1/vehicles
PUT  /api/v1/vehicles/{vehicleId}
GET  /api/v1/vehicles/{vehicleId}
```

Operações internas propostas para integração:

```http
POST /api/v1/internal/vehicles/{vehicleId}/reserve
POST /api/v1/internal/vehicles/{vehicleId}/sell
POST /api/v1/internal/vehicles/{vehicleId}/release
```

### Serviço de vendas

```http
GET  /api/v1/vehicles/available
GET  /api/v1/vehicles/sold
POST /api/v1/sales
GET  /api/v1/sales/{saleId}
POST /api/v1/webhooks/payments
```

As listagens devem ser ordenadas por preço crescente (`Price ASC`).

Os contratos de request, response, códigos HTTP, autenticação e timeouts ainda serão detalhados na etapa de desenho da API.

## 10. Tecnologia e execução local

Decisões atuais:

- .NET para os serviços.
- ASP.NET Core Web API.
- Docker para empacotamento.
- Docker Compose para execução local.
- Docker Hub para publicação das imagens.
- GitHub Actions para CI/CD.

Kubernetes não faz parte do primeiro recorte. A infraestrutura inicial deve ser simples o suficiente para ser demonstrada localmente com Docker Compose.

## 11. Qualidade e entrega

Cada repositório deve ter:

- README com objetivo, arquitetura, execução local e testes.
- Testes automatizados.
- Pipeline de CI.
- Build da aplicação e da imagem Docker.
- Publicação da imagem no Docker Hub.
- Deploy automatizado acionado após merge na branch principal.

O serviço de vendas deve demonstrar cobertura mínima de 80%. Como prática de qualidade, a mesma regra será aplicada ao serviço principal sempre que viável.

## 12. Organização sugerida do GitHub Project

Projeto: `https://github.com/users/igortessaro/projects/2`

Visualizações:

- Board para fluxo de trabalho.
- Table para planejamento e filtros.

Status:

```text
Backlog
Ready
In Progress
Review
Done
```

Campos:

- `Priority`: High, Medium, Low.
- `Area`: Requirements, Architecture, Vehicle Service, Sales Service, Testing, CI/CD, Documentation.
- `Type`: Feature, Bug, Technical Task, Documentation.

Labels sugeridas:

```text
area:requirements
area:architecture
area:vehicle-service
area:sales-service
area:testing
area:ci-cd
area:documentation
type:feature
type:bug
type:technical
priority:high
priority:medium
priority:low
```

Milestones sugeridas:

```text
M1 - Requisitos e arquitetura
M2 - Serviço principal
M3 - Serviço de vendas
M4 - Integração e pagamento
M5 - Testes e cobertura
M6 - CI/CD e Docker
M7 - Evidências e entrega
```

## 13. Decisões ainda abertas

Estas questões devem virar tarefas de refinamento antes da implementação:

- Definir o contrato detalhado de cada endpoint.
- Definir como a projeção `VehicleListing` será criada e atualizada.
- Definir a ordem exata das chamadas durante a compra e as compensações em caso de falha.
- Definir autenticação entre os serviços, se necessária na demonstração.
- Definir banco de dados e framework de persistência.
- Definir formato final de logs e tratamento de erros.
- Definir secrets do Docker Hub e configurações do GitHub Actions.
- Definir como a entrega altera a venda para `COMPLETED` e o veículo para `DELIVERED`.

## 14. Próximas etapas

1. Revisar e aprovar este documento.
2. Criar requisitos funcionais, não funcionais e critérios de aceite.
3. Criar o diagrama de componentes.
4. Criar diagramas de sequência de cadastro, compra, pagamento confirmado e cancelado.
5. Criar as issues e milestones.
6. Detalhar os contratos HTTP.
7. Implementar os dois serviços.
8. Implementar testes, Docker e CI/CD.
9. Executar o teste ponta a ponta e preparar as evidências.
