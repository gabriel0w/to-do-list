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

## ⚙️ Setup Rápido (Dev)
### Pré‑requisitos
- Node 20+
- .NET 8 SDK
- Docker + Docker Compose

### Subir ambiente (containers)
```bash
cd deploy
docker compose -f docker-compose.dev.yml up -d
```
**Serviços:**
- PostgreSQL → `localhost:5432` (db: `tododb`, user: `todo`, pass: `todo`)
- API .NET → `http://localhost:8080`
- Angular (quando buildado) → `http://localhost:4200`

### String de conexão (exemplo)
```
Host=db;Port=5432;Database=tododb;Username=todo;Password=todo
```
> Em desenvolvimento local fora do Docker, use `Host=localhost`.

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
```bash
dotnet test backend/tests/Todo.UnitTests
dotnet test backend/tests/Todo.IntegrationTests
```
Os testes de integração usam **Testcontainers** com PostgreSQL.

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

## 🧠 Sprint 1 (alvo)
- Monorepo + README (este arquivo) ✅
- Backend skeleton (Clean Arch) + Swagger + Health
- EF Core + Npgsql + migração inicial
- Entidade `Task` + endpoints básicos (GET/POST/DELETE)
- Unit tests Domain/Application
- CI backend (build + unit)

MIT © 2025