# === Build Stage ===
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copia solução e projetos
COPY BolluFlow.sln . 
COPY BolluFlow.Domain/*.csproj ./BolluFlow.Domain/
COPY BolluFlow.Application/*.csproj ./BolluFlow.Application/
COPY BolluFlow.Infrastructure/*.csproj ./BolluFlow.Infrastructure/
COPY BolluFlow.Api/*.csproj ./BolluFlow.Api/

# Restaura dependências
RUN dotnet restore

# Copia todo o código
COPY . .

# Vai para o projeto da API
WORKDIR /src/BolluFlow.Api

# Restaura tools
RUN dotnet tool restore

# Roda migrations
RUN dotnet ef database update -s BolluFlow.Api -p BolluFlow.Infrastructure

# Publica a API
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# === Runtime Stage ===
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# Copia arquivos publicados
COPY --from=build /app/publish .

EXPOSE 80

ENTRYPOINT ["dotnet", "BolluFlow.Api.dll"]

# Healthcheck
HEALTHCHECK --interval=30s --timeout=5s --start-period=10s \
 CMD curl -f http://localhost:80/health || exit 1
