# syntax=docker/dockerfile:1

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# copy csproj and restore as distinct layers for better caching
COPY src/Todo.Api/Todo.Api.csproj src/Todo.Api/
COPY src/Todo.Application/Todo.Application.csproj src/Todo.Application/
COPY src/Todo.Domain/Todo.Domain.csproj src/Todo.Domain/
COPY src/Todo.Infrastructure/Todo.Infrastructure.csproj src/Todo.Infrastructure/
RUN dotnet restore src/Todo.Api/Todo.Api.csproj

# copy everything and build
COPY . .
RUN dotnet publish src/Todo.Api/Todo.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Expose HTTP port
ENV ASPNETCORE_URLS=http://0.0.0.0:8080
EXPOSE 8080

# Optional: enable globalization invariant to reduce image size
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1

# Connection string can be provided via env: ConnectionStrings__DefaultConnection
# Example: Host=db;Port=5432;Database=tododb;Username=todo;Password=todo

COPY --from=build /app/publish ./
ENTRYPOINT ["dotnet", "Todo.Api.dll"]

