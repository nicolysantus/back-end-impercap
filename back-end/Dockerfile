# Acesse https://aka.ms/customizecontainer para saber como personalizar seu contêiner de depuração

# Esta fase é usada durante a execução no VS no modo rápido (Padrão para a configuração de Depuração)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Esta fase é usada para compilar o projeto de serviço
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
# CORREÇÃO: Removemos o prefixo "back-end/" pois o contexto já é a pasta do projeto
COPY ["back-end.csproj", "."]
RUN dotnet restore "./back-end.csproj"
COPY . .
WORKDIR "/src"
RUN dotnet build "./back-end.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Etapa de Runtime (com SDK)
FROM mcr.microsoft.com/dotnet/sdk:9.0
WORKDIR /app
COPY --from=build /app/publish .

# Esta fase é usada para publicar o projeto de serviço
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./back-end.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Esta fase é usada na produção ou quando executada no VS no modo normal
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "back-end.dll"]