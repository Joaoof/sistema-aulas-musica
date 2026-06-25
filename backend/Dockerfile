# ─────────────────────────────────────────────
# Stage 1: build & publish (.NET 10 SDK)
# ─────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Restore primeiro -> melhor cache de layers para dependências
COPY PortalAluno.sln ./
COPY src/PortalAluno.Domain/*.csproj          src/PortalAluno.Domain/
COPY src/PortalAluno.Application/*.csproj      src/PortalAluno.Application/
COPY src/PortalAluno.Infrastructure/*.csproj   src/PortalAluno.Infrastructure/
COPY src/PortalAluno.API/*.csproj              src/PortalAluno.API/
RUN dotnet restore PortalAluno.sln

COPY . .
RUN dotnet publish src/PortalAluno.API/PortalAluno.API.csproj \
    -c Release -o /app/publish /p:UseAppHost=false

# ─────────────────────────────────────────────
# Stage 2 (prod): runtime enxuto ASP.NET
#   -> use este target em produção: `--target runtime`
# ─────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
# curl: usado pelo healthcheck do compose (GET /health)
RUN apt-get update \
    && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*
COPY --from=build /app/publish ./
EXPOSE 8080
ENTRYPOINT ["dotnet", "PortalAluno.API.dll"]

# ─────────────────────────────────────────────
# Stage 3 (dev - DEFAULT): SDK + dotnet-ef + fonte
#   -> permite `make migrate` / `make migration` via compose exec
# ─────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS dev
WORKDIR /src
ENV ASPNETCORE_URLS=http://+:8080
ENV PATH="$PATH:/root/.dotnet/tools"
RUN dotnet tool install --global dotnet-ef --version 10.*

# código-fonte (para migrations) + binários publicados
COPY --from=build /src /src
COPY --from=build /app/publish /app/publish

EXPOSE 8080
ENTRYPOINT ["dotnet", "/app/publish/PortalAluno.API.dll"]
