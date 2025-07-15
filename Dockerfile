# Etapa de build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copia tudo
COPY . ./

# Restaura dependências
RUN dotnet restore

# Publica o projeto API (ajuste o nome do csproj se for diferente)
RUN dotnet publish ./GameNewsBoard.Api/GameNewsBoard.Api.csproj -c Release -o out

# Etapa final
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

COPY --from=build /app/out ./

# Porta padrão
EXPOSE 80

# Comando para iniciar a aplicação
ENTRYPOINT ["dotnet", "GameNewsBoard.Api.dll"]
