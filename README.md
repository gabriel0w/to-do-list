# 📝 To-Do List — Angular + .NET Clean Architecture

Aplicação **To‑Do List** com frontend **Angular**, backend **.NET 8 (Clean Architecture)**.  
Funcionalidades: criar, listar, concluir (botão e Drag&Drop), excluir, reordenar, notificações, testes (unit/integração/E2E) e CI com GitHub Actions.

## 🚀 Tecnologias Principais

| Camada | Stack |
|---|---|
| **Frontend** | Angular 18+, TypeScript, Angular Material, Reactive Forms, CDK DragDrop |
| **Backend** | .NET 8 Web API, Clean Architecture, EF Core, FluentValidation, MediatR |
| **Banco** | **PostgreSQL** (Docker) + EF Core Provider Npgsql |
| **Testes** | xUnit, FluentAssertions, **Testcontainers for .NET (PostgreSQL)**, Playwright (E2E) |
| **Infra** | Docker, Docker Compose, GitHub Actions |

## 📁 Estrutura do Repositório

```
/todo-monorepo
├── frontend/                      # App Angular
├── backend/
│   ├── src/
│   │   ├── Todo.Domain/
│   │   ├── Todo.Application/
│   │   ├── Todo.Infrastructure/
│   │   └── Todo.Api/
│   └── tests/
│       ├── Todo.UnitTests/
│       └── Todo.IntegrationTests/
├── deploy/
│   └── docker-compose.dev.yml
└── .github/workflows/
    ├── ci-frontend.yml
    └── ci-backend.yml
```

## 🧩 Funcionalidades (MVP)
- Inserir, excluir, listar tarefas
- Concluir/abrir tarefa (ação ou Drag&Drop com reorder)
- Filtros (abertas, concluídas, prioridade) e ordenação
- Notificações (snackbar; Web Notifications opcional)
- Testes unitários e de integração (API + PostgreSQL via Testcontainers)

## ⚙️ Dev Setup (atualizado)
### Pré‑requisitos
- .NET 8 SDK
- Docker + Docker Compose (para Postgres local e testes de integração)
- Node 20+ (para o frontend, quando aplicável)

### Subir infraestrutura (Postgres)
```bash
cd to-do-list/deploy
docker compose -f docker-compose.dev.yml up -d
```
Serviços:
- PostgreSQL → `localhost:5432` (db: `tododb`, user: `todo`, pass: `todo`)

### Rodar API (.NET)
```bash
cd to-do-list/backend
# (primeira vez) aplicar migrações
dotnet ef database update --project src/Todo.Infrastructure --startup-project src/Todo.Api

# executar API
dotnet run --project src/Todo.Api
```
URLs em dev (padrão):
- Swagger: `http://localhost:5062/swagger`
- API base: `http://localhost:5062`

### Connection string
Em dev, a API aponta para Postgres local (Host=localhost). Se rodar a API em container, use `Host=db`.

```
Host=localhost;Port=5432;Database=tododb;Username=todo;Password=todo
```

## 🧱 Backend (.NET + Clean Architecture)
### Camadas
- **Domain**: entidades e regras (ex.: `Task`).
- **Application**: camada de lógica de aplicação, responsável por serviços, validações (FluentValidation) e mapeamentos entre entidades e DTOs.
- **Infrastructure**: EF Core (Npgsql), repositórios, DbContext e migrações.
- **API**: endpoints/Minimal APIs ou Controllers, DI, Swagger/Health.

### Comandos úteis
```bash
# dentro de /backend (após criar os projetos)
dotnet build
dotnet ef migrations add Initial --project src/Todo.Infrastructure --startup-project src/Todo.Api
dotnet ef database update --project src/Todo.Infrastructure --startup-project src/Todo.Api
dotnet run --project src/Todo.Api
```

### Testes
Tipos:
- Unit: regras de domínio/aplicação.
- Integração: repositório/EF + PostgreSQL via Testcontainers.

Comandos:
```bash
# na pasta backend
cd to-do-list/backend

# unit
dotnet test tests/Todo.UnitTests -v minimal

# integração (precisa Docker e rede para restaurar pacotes)
dotnet test tests/Todo.IntegrationTests -v minimal
```

#### Cobertura (coverage)
Coleta com data collector (Coverlet):
```bash
dotnet test tests/Todo.UnitTests --collect:"XPlat Code Coverage" -v minimal
dotnet test tests/Todo.IntegrationTests --collect:"XPlat Code Coverage" -v minimal
```
Saída: arquivos `coverage.cobertura.xml` em `TestResults/<run>/` de cada projeto.
Se necessário, adicione o coletor nos projetos de teste:
```bash
dotnet add tests/Todo.UnitTests package coverlet.collector
dotnet add tests/Todo.IntegrationTests package coverlet.collector
```
Para HTML consolidado, use ReportGenerator (opcional).

### Endpoints atuais (MVP + Sprint 2)
- `GET /api/tasks` com filtros por query: `status=all|open|done`, `sort=orderIndex|createdAt`, `direction=asc|desc`.
- `POST /api/tasks` — cria tarefa (validação via FluentValidation).
- `GET /api/tasks/{id}` — obtém tarefa.
- `PATCH /api/tasks/{id}/complete` — alterna conclusão.
- `DELETE /api/tasks/{id}` — remove tarefa.
- `PUT /api/tasks/reorder` — reordena em lote (`[{ id, orderIndex }, ...]`), idempotente.

Erros 400 retornam ValidationProblemDetails (com `traceId`); 404 para recursos inexistentes.

## 💻 Frontend (Angular)
```bash
cd frontend
npm install
npm start
```
Rotas principais: lista de tarefas, formulário (Reactive Forms), Drag&Drop (CDK).

## 🔄 Git Flow
- `main` (prod), `develop` (integração), `feature/*`, `release/*`, `hotfix/*`  
Commits: **Conventional Commits**.

## 🤖 GitHub Actions (CI)
- **ci-backend.yml**: build, testes unit/integration (.NET + Postgres service ou Testcontainers).
- **ci-frontend.yml**: lint, unit tests, build.
(Deploy/CD pode ser adicionado depois.)

MIT © 2025
