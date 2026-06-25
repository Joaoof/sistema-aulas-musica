# ─────────────────────────────────────────────
# Stage 1: build do bundle React/Vite
# ─────────────────────────────────────────────
FROM node:22-alpine AS build
WORKDIR /app

# A URL da API é injetada em build-time (Vite embute import.meta.env.*)
ARG VITE_API_URL=http://localhost:8080
ENV VITE_API_URL=$VITE_API_URL

COPY package*.json ./
RUN npm install
COPY . .
RUN npm run build

# ─────────────────────────────────────────────
# Stage 2: Nginx servindo os estáticos (SPA + PWA)
# ─────────────────────────────────────────────
FROM nginx:1.27-alpine AS runtime
COPY nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=build /app/dist /usr/share/nginx/html
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
