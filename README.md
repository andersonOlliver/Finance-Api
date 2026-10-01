# Finance API

API para controle de finanças pessoais: lançamentos (receitas/despesas), categorias, formas de pagamento, veículos (com consumo de combustível), cartões de crédito e compras parceladas.

## Sumário

- [Visão geral](#visão-geral)
- [Arquitetura](#arquitetura)
- [Stack](#stack)
- [Domínios e endpoints](#domínios-e-endpoints)
- [Como executar](#como-executar)
- [Configuração](#configuração)
- [Testes](#testes)
- [Estrutura do projeto](#estrutura-do-projeto)

## Visão geral

A API permite:

- Registrar **lançamentos** (receitas/despesas), categorizados e opcionalmente vinculados a uma forma de pagamento, a um veículo ou a uma compra parcelada.
- Gerenciar **categorias** e **formas de pagamento** próprias do usuário, além de um conjunto padrão compartilhado por todos.
- Cadastrar **veículos** (carro/moto) e registrar **abastecimentos**, que atualizam a quilometragem e calculam o consumo (km/L) desde o último abastecimento.
- Cadastrar **cartões de crédito** (sem dados sensíveis como número ou CVV) e criar **compras parceladas**, que geram automaticamente um lançamento por parcela e permitem consultar, mês a mês, qual parcela de cada compra está ativa (ex.: "5/12").

Autenticação é feita via Keycloak (OAuth2/OIDC), e cada recurso é isolado por usuário.

## Arquitetura

O projeto segue **Clean Architecture** com CQRS (via [MediatR](https://github.com/jbogard/MediatR)):

```
Finance.Domain          Entidades, regras de negócio e abstrações (sem dependências externas)
Finance.Application     Commands/Queries, handlers, validators (FluentValidation)
Finance.Infrastructure  EF Core (Postgres), repositórios, autenticação (Keycloak), Dapper
Finance.Api             Controllers, DTOs de request, composição/DI
```

Regras principais:

- **Escrita** (commands) passa por EF Core, através de repositórios por agregado (`ITransactionRepository`, `IVehicleRepository`, etc.).
- **Leitura** (queries) usa [Dapper](https://github.com/DapperLib/Dapper) com SQL direto, para controle fino de performance e shape da resposta.
- Toda query/command passa por um pipeline de `ValidationBehavior` + `LoggingBehavior`.
- Handlers e validators são `internal` — a regra é verificada por testes de arquitetura (`Finance.ArchitectureTests`).
- Identificadores novos usam **UUID v7** (`Guid.CreateVersion7()`), ordenáveis cronologicamente.

## Stack

- **.NET 10** / ASP.NET Core Web API
- **PostgreSQL** via EF Core (escrita) + Dapper (leitura)
- **MediatR** (CQRS) + **FluentValidation**
- **Keycloak** para autenticação (JWT Bearer)
- **Serilog** com sink para **Seq**
- **xUnit**, **FluentAssertions**, **NSubstitute**, **Testcontainers** (testes de integração com Postgres real)
- **Docker Compose** para orquestração do ambiente local

## Domínios e endpoints

Todos os endpoints (exceto registro/login) exigem autenticação (`Authorization: Bearer <token>`).

| Recurso | Base route | Operações |
|---|---|---|
| Usuários | `/api/users` | `POST /register`, `POST /login`, `GET /me` |
| Categorias | `/api/categories` | `GET`, `GET /{id}`, `POST`, `PUT /{id}`, `DELETE /{id}` |
| Formas de pagamento | `/api/payments` | `GET`, `GET /{id}`, `POST`, `PUT /{id}`, `DELETE /{id}` |
| Lançamentos | `/api/transactions` | `GET` (filtros: data, categoria, pagamento, veículo), `GET /{id}`, `POST`, `PUT /{id}`, `DELETE /{id}` |
| Veículos | `/api/vehicles` | `GET`, `GET /{id}`, `POST`, `PUT /{id}`, `DELETE /{id}` |
| Abastecimentos | `/api/vehicles/{id}/refuels` | `GET`, `POST` (gera lançamento, atualiza km e calcula consumo) |
| Cartões de crédito | `/api/credit-cards` | `GET`, `GET /{id}`, `POST`, `PUT /{id}`, `DELETE /{id}` |
| Compras parceladas | `/api/installment-purchases` | `GET` (filtros: mês, cartão), `GET /{id}`, `POST` (gera 1 lançamento por parcela), `DELETE /{id}` |

Categorias e formas de pagamento têm registros **padrão** (compartilhados, sem dono) além dos próprios de cada usuário; veículos e cartões de crédito são sempre exclusivos do usuário autenticado.

Com a API em execução, a documentação interativa (Swagger) fica disponível em `/swagger`.

## Como executar

Pré-requisitos: [Docker](https://www.docker.com/) e [Docker Compose](https://docs.docker.com/compose/).

```bash
docker compose up -d
```

Isso inicia:

| Serviço | Porta | Descrição |
|---|---|---|
| `finance.api` | `8080` / `8081` | API (HTTP/HTTPS) |
| `finance-db` | `5432` | PostgreSQL |
| `finance-idp` | `18080` | Keycloak (realm `finance` já importado) |
| `pgadmin` | `8082` | Administração do Postgres |
| `finance-seq` | `8083` (UI) / `5431` (ingestão) | Visualização de logs estruturados |

As migrations do EF Core são aplicadas automaticamente na inicialização (em ambiente de desenvolvimento), junto com um seed inicial de usuário, categorias e formas de pagamento padrão.

> O serviço `finance.api` só inicia depois que o Postgres reporta estar saudável (`healthcheck`), evitando falhas de conexão na subida.

### Executando sem Docker

Com um Postgres acessível e a connection string configurada (veja [Configuração](#configuração)):

```bash
dotnet run --project src/Finance.Api
```

## Configuração

As configurações vivem em `src/Finance.Api/appsettings.Development.json` (não versionado com segredos reais em produção):

```json
{
  "ConnectionStrings": {
    "Database": "Host=finance-db;Port=5432;Database=finance;Username=olliver;Password=q1w2e3r4;"
  },
  "Authentication": {
    "ValidIssuer": "http://finance-idp:8080/auth/realms/finance"
  },
  "Keycloak": {
    "BaseUrl": "http://finance-idp:8080"
  }
}
```

Para gerar uma nova migration após alterar o modelo:

```bash
dotnet ef migrations add NomeDaMigration --project src/Finance.Infrastructure --startup-project src/Finance.Api
```

## Testes

```bash
# Testes unitários (handlers com dependências mockadas via NSubstitute)
dotnet test test/Finance.UnitTests

# Testes de integração (sobem um Postgres real via Testcontainers — exige Docker em execução)
dotnet test test/Finance.IntegrationTests

# Testes de arquitetura (convenções de camadas, visibilidade de handlers/validators)
dotnet test test/Finance.ArchitectureTests
```

## Estrutura do projeto

```
src/
  Finance.Domain/          Entidades (Transaction, Category, Payment, Vehicle, CreditCard, InstallmentPurchase...)
  Finance.Application/     Commands, Queries, Handlers e Validators, organizados por recurso
  Finance.Infrastructure/  DbContext, Configurations (EF), Repositories, Migrations, Autenticação
  Finance.Api/             Controllers e Requests
test/
  Finance.UnitTests/         Testes de domínio e de handlers (mocks)
  Finance.IntegrationTests/  Testes ponta a ponta contra Postgres real (Testcontainers)
  Finance.ArchitectureTests/ Testes de convenção de arquitetura
```
