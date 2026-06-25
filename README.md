# 🎵 Portal do Aluno de Música

Aplicação full-stack conteinerizada (DDD + .NET 10 + PostgreSQL + Redis + React PWA).

```
sistema-aulas-musica/
├── docker-compose.yml          # 4 serviços: api, web, postgres, redis
├── Makefile                    # DX: up / down / logs / restart / migrate ...
├── .env.example
├── backend/                    # .NET 10 — Arquitetura DDD
│   ├── Dockerfile              # multi-stage (build → runtime/dev)
│   └── src/
│       ├── PortalAluno.Domain          # Entidades + interfaces de repositório
│       ├── PortalAluno.Application     # Casos de uso (MediatR) + DTOs + ICacheService
│       ├── PortalAluno.Infrastructure  # EF Core (Postgres) + Redis + repositórios
│       └── PortalAluno.API             # Controllers REST + DI completa
└── frontend/                   # React + TS + Vite + PWA — estética "Pro Audio Dark"
    ├── Dockerfile              # multi-stage (Node build → Nginx)
    ├── nginx.conf
    └── src/
        ├── components/ui        # Shadcn/ui (tema Zinc) — button, card, tabs, table, badge
        ├── components/charts    # Tremor — AreaChart (BPM) + DonutChart (repertório)
        ├── components/aceternity# GlowCard (Framer Motion) — destaque da Sprint Atual
        └── components/layout    # Bottom Navigation (Phosphor Icons, "LED aceso")
```

### Hierarquia visual do frontend
| Camada            | Biblioteca            | Onde é usada                                        |
|-------------------|-----------------------|-----------------------------------------------------|
| Estrutura/base    | **Shadcn/ui** (Zinc)  | botões, cards, tabela de repertório, badges         |
| Visualização      | **Tremor**            | AreaChart de BPM + DonutChart (dominadas vs. aprend.)|
| Destaque premium  | **Aceternity / Framer** | card com brilho que segue o cursor (Sprint Atual)  |
| Iconografia       | **@phosphor-icons**   | bottom nav com `weight` ativo (fill+âmbar) / inativo (regular+zinc) |

## 🚀 Subir tudo (1 comando)

```bash
cp .env.example .env      # opcional — há defaults sensatos
make up
```

Isso faz **build + start** dos 4 contêineres. A API aplica as **migrations do EF Core**
e popula dados de demonstração automaticamente no boot.

| Serviço            | URL                              |
|--------------------|----------------------------------|
| Web (React PWA)    | http://localhost:3000            |
| API (.NET 10)      | http://localhost:8080/swagger    |
| Health check       | http://localhost:8080/health     |
| PostgreSQL         | localhost:5432                   |
| Redis              | localhost:6379                   |

### Login de demonstração
Use o e-mail **`ana@portal.dev`** (ou o ID retornado) na tela de entrada.
O login emite um **JWT**; o frontend o envia em `Authorization: Bearer` e os endpoints
do aluno exigem autenticação (`[Authorize]` + checagem de posse do recurso).

### Área do Professor (super usuário)
Acesse **http://localhost:5173/admin/login** (login por e-mail + senha, role `Admin`).
As credenciais são semeadas a partir de configuração — em dev, defina `Admin__Password`
via variável de ambiente (nunca em arquivo versionado). O professor pode:
- **Criar alunos**, atribuir **plano** (catálogo dos 4 pacotes) e **editar preço/qtd** por aluno;
- Cadastrar **repertório** e **materiais** (URLs do Google Drive);
- **Agendar aulas** e usar o **checklist do dia**: marcar *Feita* ou *Justificar* (motivo obrigatório);
- Acompanhar **quantas aulas foram feitas no mês** por aluno.

### Endpoints principais
| Método | Rota | Auth | Descrição |
|--------|------|------|-----------|
| POST | `/api/auth/login` | — | Login do aluno (e-mail/ID) → JWT role Student |
| POST | `/api/auth/admin/login` | — | Login do professor (e-mail+senha) → JWT role Admin |
| GET | `/api/students/{id}/dashboard` | Student | Dashboard (cacheado no Redis) |
| POST | `/api/students/{id}/practice` | Student | Registra BPM e **invalida** o cache |
| GET/POST | `/api/admin/students` | Admin | Listar / criar alunos |
| GET | `/api/admin/students/{id}` | Admin | Detalhe (plano, repertório, materiais, aulas) |
| PUT | `/api/admin/students/{id}/plan` | Admin | Atribuir/editar plano |
| POST | `/api/admin/students/{id}/repertoire` · `/materials` | Admin | Adicionar conteúdo |
| POST | `/api/admin/lessons` | Admin | Agendar aula |
| GET | `/api/admin/lessons/today` | Admin | Checklist do dia |
| POST | `/api/admin/lessons/{id}/complete` · `/justify` | Admin | Marcar feita / justificar |
| GET | `/api/admin/lessons/stats` | Admin | Aulas feitas no mês por aluno |
| GET | `/api/admin/plans` | Admin | Catálogo de pacotes |

## 🛠️ Comandos (Makefile)

```bash
make help            # lista todos os comandos
make up              # build + sobe o ecossistema
make down            # para e remove containers (mantém volumes)
make restart         # reinicia os serviços
make logs            # tail de todos os logs
make logs-api        # tail só da API
make migrate         # aplica migrations do EF Core no Postgres
make migration name=AddX   # cria nova migration
make db-shell        # psql no Postgres
make redis-cli       # redis-cli no Redis
make clean           # down + APAGA volumes (reseta o banco)
```

## 🧩 Decisões de arquitetura

- **DDD estrito**: o domínio (`Student`, `Repertoire`, `Material`) não depende de nada externo;
  invariantes protegidas por construtores e setters privados.
- **Cache-aside com Redis**: `GetStudentDashboardQuery` consulta o Redis via
  `IDistributedCache` (abstraído por `ICacheService`) antes de tocar o Postgres. TTL de 2 min.
- **Estratégia de arquivos**: PDFs/vídeos são **URLs externas do Google Drive** —
  o sistema apenas referencia (`ExternalUrl` / `VideoUrl`), nunca armazena binários.
- **Frontend PWA**: instalável, com service worker (`vite-plugin-pwa`) e cache `NetworkFirst`
  para chamadas `/api`. Mobile-first com bottom navigation; charts (Tremor/recharts) em
  **lazy-load** para não pesar o carregamento inicial.
- **BPM real**: o backend expõe `PracticeSession` (entidade no agregado `Student`), e o
  `GetStudentDashboardQuery` devolve `bpmHistory` + `repertoireStats` já prontos para os gráficos.

## 📌 Observações

- O estágio `dev` do Dockerfile do backend inclui o SDK + `dotnet-ef` para que
  `make migrate` funcione via `compose exec`. Em produção, troque para `--target runtime`
  (imagem ASP.NET enxuta) no `docker-compose.yml`.
- Os ícones PWA (`public/pwa-192x192.png`, `pwa-512x512.png`) são opcionais para rodar;
  adicione-os para instalação completa como app.
- **Aviso NU1903 (System.Security.Cryptography.Xml)**: provém apenas de
  `Microsoft.EntityFrameworkCore.Design → Microsoft.Build.Tasks.Core`, uma dependência
  **de tooling/migrations** (`PrivateAssets=all`). **Não** é incluída no output de runtime
  publicado (imagem `runtime`), logo não afeta o contêiner de produção. Sem versão corrigida
  disponível no momento — item a monitorar quando o EF Core Design publicar atualização.

## ✔️ Validação executada

- **Backend**: `dotnet build -c Release` → **0 erros** (.NET 10 SDK 10.0.106), com JWT + endpoint de prática.
- **Testes**: `make test` → **13/13 aprovados** (xUnit + NSubstitute): invalidação de cache no
  registro de BPM, cache-aside do dashboard (hit/miss/not-found), emissão de JWT e fluxo de login.
- **Frontend**: `npm run build` → **OK**, PWA/service worker + ícones 192/512, charts em lazy-load.
- **Runtime e2e**: não validado — exige Postgres+Redis (Docker), bloqueado pelo grupo `docker`.
- **`make up`**: não validado neste ambiente — usuário `joaoof` não está no grupo `docker`
  (rode `sudo make up` ou `sudo usermod -aG docker $USER && newgrp docker`).
