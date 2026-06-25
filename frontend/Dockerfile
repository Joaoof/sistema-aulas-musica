# ─────────────────────────────────────────────
# Stage 1: build do bundle React/Vite
# ─────────────────────────────────────────────
FROM node:22-alpine AS build
WORKDIR /app

# A URL da API é injetada em build-time (Vite embute import.meta.env.*)
ARG VITE_API_URL=http://localhost:8080
ENV VITE_API_URL=$VITE_API_URL

# Heap explícito: em servidores com pouca RAM o Node 22 auto-reduz o heap e
# o `vite build` (6.9k módulos) pode estourar. Fixar evita "heap out of memory".
ENV NODE_OPTIONS=--max-old-space-size=2048

COPY package*.json ./
RUN npm install --no-audit --no-fund
COPY . .
RUN npm run build

# ─────────────────────────────────────────────
# Stage 2: Caddy servindo os estáticos (SPA + PWA) + proxy de /api
# ─────────────────────────────────────────────
FROM caddy:2-alpine AS runtime
COPY Caddyfile /etc/caddy/Caddyfile
COPY --from=build /app/dist /usr/share/caddy
EXPOSE 80
# A imagem oficial já roda: caddy run --config /etc/caddy/Caddyfile --adapter caddyfile
