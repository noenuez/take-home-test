# syntax=docker/dockerfile:1

# ---- build stage ----
FROM node:20-alpine AS build
WORKDIR /app

COPY package*.json ./
RUN npm ci

COPY . .
RUN npm run build

# ---- runtime stage (nginx serves the SPA and proxies /api to the backend) ----
FROM nginx:alpine AS runtime
COPY nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=build /app/dist/temp/browser /usr/share/nginx/html
EXPOSE 80
