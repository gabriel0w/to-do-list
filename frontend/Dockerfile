# syntax=docker/dockerfile:1

FROM node:20-alpine AS build
WORKDIR /app
COPY package*.json ./
# cache npm data across builds
RUN --mount=type=cache,target=/root/.npm npm ci --prefer-offline --no-audit --no-fund
COPY . .
RUN npm run build -- --configuration production \
 && mkdir -p /app/www \
 && if [ -d dist/frontend/browser ]; then cp -r dist/frontend/browser/* /app/www/; else cp -r dist/frontend/* /app/www/; fi

FROM nginx:alpine AS final
COPY --from=build /app/www /usr/share/nginx/html
# Basic SPA routing: serve index.html for not found routes
COPY nginx.conf /etc/nginx/conf.d/default.conf
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
