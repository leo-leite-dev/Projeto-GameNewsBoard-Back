# Etapa de build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copia tudo
COPY . ./

# Restaura dependências
RUN dotnet restore

# Publica o projeto API
RUN dotnet publish ./GameNewsBoard.Api/GameNewsBoard.Api.csproj -c Release -o out

# Etapa final
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

COPY --from=build /app/out ./

# Define a porta que o app vai escutar
ENV ASPNETCORE_URLS=http://+:80

# Expõe a porta 80
EXPOSE 80

# Comando para iniciar a aplicação
ENTRYPOINT ["dotnet", "GameNewsBoard.Api.dll"]
