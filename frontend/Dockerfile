# syntax=docker/dockerfile:1

FROM node:20-alpine AS build
WORKDIR /app
COPY package*.json ./
# cache npm data across builds
RUN --mount=type=cache,target=/root/.npm npm ci --prefer-offline --no-audit --no-fund
COPY . .
RUN npm run build -- --configuration production --output-path=/app/www

FROM nginx:alpine AS final
# Clean default content just in case
RUN rm -rf /usr/share/nginx/html/*
# Copy our SPA assets
COPY --from=build /app/www /usr/share/nginx/html
# Basic SPA routing + API proxy
COPY nginx.conf /etc/nginx/conf.d/default.conf
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
