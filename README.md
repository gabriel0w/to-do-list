# To‑Do List — Angular + .NET (Clean Architecture)

Aplicação de lista de tarefas com frontend Angular 18 e backend .NET 8. Inclui CRUD, concluir/reabrir, reordenação com Drag&Drop, filtros/ordenação, notificações (snackbar e Web Notifications), testes (unit/integration/E2E) e ambiente Docker para desenvolvimento.

## Tecnologias

| Camada   | Stack |
|---|---|
| Frontend | Angular 18+, Typescript, Angular Material, Reactive Forms, CDK DragDrop |
| Backend  | .NET 8 Web API, EF Core (Npgsql), FluentValidation |
| Banco    | PostgreSQL (Docker) |
| Testes   | xUnit, FluentAssertions, Testcontainers (PostgreSQL), Playwright (E2E) |
| Infra    | Docker, Docker Compose, GitHub Actions |

## Estrutura do Repositório

```
to-do-list/
  backend/
    src/
      Todo.Domain/
      Todo.Application/
      Todo.Infrastructure/
      Todo.Api/
    tests/
      Todo.UnitTests/
      Todo.IntegrationTests/
    Dockerfile
  frontend/
    src/
    e2e/
    Dockerfile
    nginx.conf
  deploy/
    docker-compose.dev.yml
  scripts/
    compose-up.ps1 | compose-down.ps1 | compose-logs.ps1
    api-migrate.ps1 | api-run.ps1 | test-unit.ps1 | test-int.ps1 | e2e.ps1
  startup.ps1 | startup.sh
  README.md
```

## Dev Setup

### Pré‑requisitos
- .NET 8 SDK
- Docker + Docker Compose
- Node 20+

### Subir tudo com 1 comando (Docker)
- Windows (PowerShell): `./startup.ps1`
- Linux/macOS (Bash): `./startup.sh`

URLs:
- API: `http://localhost:8080` (Swagger em `/swagger`)
- Web: `http://localhost:8081`

> Dica: os scripts habilitam o BuildKit durante a sessão para builds mais rápidos (cache de NuGet/npm).

### Alternativo: dev local (ng serve + dotnet run)
- `cd deploy && docker compose -f docker-compose.dev.yml up -d db`
- `cd backend && dotnet ef database update --project src/Todo.Infrastructure --startup-project src/Todo.Api && dotnet run --project src/Todo.Api`
- `cd frontend && npm install && npm start`

### Connection string (exemplo)
```
Host=localhost;Port=5432;Database=tododb;Username=todo;Password=todo
```

### População de dados (opcional)
- Via scripts (recomendado):
  - Windows: `scripts/populate-db.ps1 -ApiBase 'http://localhost:8080'`
  - Linux/macOS: `scripts/populate-db.sh http://localhost:8080`
  - Ou já na subida: `./startup.ps1 -PopulateDb` | `./startup.sh --populate-db`
- Via flags no backend (apenas quando desejar e em Development):
  - `Seed__Reset=true` limpa e insere dados padrão
  - `Seed__Force=true` apenas insere dados padrão (sem limpar)
  - Observação: por padrão o seeder não executa nada; só roda quando essas flags forem definidas.

## API (principais endpoints)
- `GET /api/tasks` — filtros por query: `status=all|open|done`, `sort=orderIndex|createdAt`, `direction=asc|desc`
- `POST /api/tasks` — cria tarefa (validação via FluentValidation)
- `GET /api/tasks/{id}` — obtém tarefa
- `PATCH /api/tasks/{id}/complete` — alterna conclusão
- `DELETE /api/tasks/{id}` — remove tarefa
- `PUT /api/tasks/reorder` — reordena em lote (`[{ id, orderIndex }, ...]`), idempotente

Erros 400 retornam ValidationProblemDetails (com `traceId`); 404 para recursos inexistentes.

## Frontend (Angular)
- Lista de tarefas com criar/excluir/concluir
- Reordenação com Drag&Drop (CDK) persistente
- Filtros (status) e ordenação (campo/direção)
- Notificações: snackbar e Web Notifications (solicita permissão e notifica ao concluir)

## Testes

Tipos:
- Unit (Domain/Application)
- Integração (repositório/EF + PostgreSQL via Testcontainers)
- E2E (Playwright)

Comandos (backend):
```
cd to-do-list/backend
# Unit
dotnet test tests/Todo.UnitTests -v minimal
# Integração (precisa Docker e rede)
dotnet test tests/Todo.IntegrationTests -v minimal
```

Cobertura (Coverlet):
```
dotnet test tests/Todo.UnitTests --collect:"XPlat Code Coverage" -v minimal
dotnet test tests/Todo.IntegrationTests --collect:"XPlat Code Coverage" -v minimal
```

E2E (frontend):
```
cd to-do-list/frontend
npm install
npx playwright install
npm run e2e
```

## Scripts de DX
- Compose
  - `scripts/compose-up.ps1` — sobe a stack (db+api+web)
  - `scripts/compose-down.ps1` — derruba stack (inclui volumes)
  - `scripts/compose-logs.ps1 [service]` — segue logs
- Backend
  - `scripts/api-migrate.ps1` — aplica migrações (EF)
  - `scripts/api-run.ps1` — executa API local
  - `scripts/test-unit.ps1` / `scripts/test-int.ps1` — testes
- Frontend
  - `scripts/e2e.ps1` — instala deps e roda Playwright
- Startup (raiz)
  - `./startup.ps1` (Windows) | `./startup.sh` (Linux/macOS)

## Observabilidade
- Logs estruturados (JSON) no console da API (útil em containers)
- Healthcheck: `GET /health` (liveness). O seeding de dev é tolerante a DB offline

## CI
- Backend: cache de NuGet + build + testes (deploy/ci-backend.yml)
- Pode ser expandido com cobertura/artifacts e E2E

## Troubleshooting
- Web mostra “Welcome to nginx!”
  - Rebuild sem cache: `docker compose -f deploy/docker-compose.dev.yml build web --no-cache && docker compose -f deploy/docker-compose.dev.yml up -d web`
  - Verifique `/usr/share/nginx/html` no container `todo_web` — deve conter `index.html` do Angular
- Portas em uso: ajuste mapeamentos em `deploy/docker-compose.dev.yml`
- DB indisponível: API não cai (seed protegido). Suba o `db` com compose
- Proxy do front: em compose, `/api` → `api:8080`; em dev local (ng serve), usamos `proxy.conf.json`

## Licença
MIT © 2025
