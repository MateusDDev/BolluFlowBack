# === Build Stage ===
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copia solução e todos os projetos
COPY BolluFlow.sln . 
COPY BolluFlow.Domain/*.csproj ./BolluFlow.Domain/
COPY BolluFlow.Application/*.csproj ./BolluFlow.Application/
COPY BolluFlow.Infrastructure/*.csproj ./BolluFlow.Infrastructure/
COPY BolluFlow.Api/*.csproj ./BolluFlow.Api/

# Restaura dependências
RUN dotnet restore

# Copia todo o código
COPY . .

# Publica a API
WORKDIR /src/BolluFlow.Api
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
