# ──────────────────────────────────────────────────────────────
# Portal do Aluno de Música — Developer Experience (DX) commands
# ──────────────────────────────────────────────────────────────
# Local dev usa o compose de desenvolvimento (target dev: SDK + dotnet-ef).
# Produção (Coolify) usa docker-compose.yml por padrão.
COMPOSE := docker compose -f docker-compose.dev.yml
API_SVC := api

.DEFAULT_GOAL := help

.PHONY: help up down build start stop restart logs logs-api logs-web ps \
        migrate migration db-shell redis-cli clean nuke test

help: ## Lista os comandos disponíveis
	@grep -E '^[a-zA-Z_-]+:.*?## .*$$' $(MAKEFILE_LIST) \
		| awk 'BEGIN {FS = ":.*?## "}; {printf "  \033[36m%-14s\033[0m %s\n", $$1, $$2}'

up: ## Build + sobe todo o ecossistema (api, web, postgres, redis)
	$(COMPOSE) up --build -d
	@echo ""
	@echo "  ✅  Ecossistema no ar:"
	@echo "      • Web (React PWA) ->  http://localhost:3000"
	@echo "      • API (.NET 10)   ->  http://localhost:8080/swagger"
	@echo "      • Postgres        ->  localhost:5432"
	@echo "      • Redis           ->  localhost:6379"

down: ## Para e remove containers + redes (mantém volumes)
	$(COMPOSE) down

build: ## Apenas (re)build das imagens, sem subir
	$(COMPOSE) build

start: ## Inicia containers já criados
	$(COMPOSE) start

stop: ## Para containers sem remover
	$(COMPOSE) stop

restart: ## Reinicia todos os serviços
	$(COMPOSE) restart

logs: ## Tail dos logs de todos os serviços
	$(COMPOSE) logs -f --tail=100

logs-api: ## Tail dos logs apenas da API
	$(COMPOSE) logs -f --tail=100 $(API_SVC)

logs-web: ## Tail dos logs apenas do frontend
	$(COMPOSE) logs -f --tail=100 web

ps: ## Status dos containers
	$(COMPOSE) ps

migrate: ## Aplica as migrations do EF Core no Postgres
	$(COMPOSE) exec $(API_SVC) dotnet ef database update \
		--project src/PortalAluno.Infrastructure \
		--startup-project src/PortalAluno.API

migration: ## Cria nova migration: make migration name=NomeDaMigration
	$(COMPOSE) exec $(API_SVC) dotnet ef migrations add $(name) \
		--project src/PortalAluno.Infrastructure \
		--startup-project src/PortalAluno.API \
		--output-dir Persistence/Migrations

db-shell: ## Abre psql no container do Postgres
	$(COMPOSE) exec postgres psql -U $${POSTGRES_USER:-portal} -d $${POSTGRES_DB:-portal_aluno}

redis-cli: ## Abre redis-cli no container do Redis
	$(COMPOSE) exec redis redis-cli

test: ## Roda a suíte de testes do backend (xUnit) — não precisa de Docker
	cd backend && dotnet test PortalAluno.sln

clean: ## down + remove volumes (APAGA dados do banco)
	$(COMPOSE) down -v

nuke: ## clean + remove imagens construídas localmente
	$(COMPOSE) down -v --rmi local --remove-orphans
