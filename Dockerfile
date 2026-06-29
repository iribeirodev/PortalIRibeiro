# Estágio de Build (.NET 10 SDK)
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copia os arquivos de projeto primeiro para otimizar o cache de camadas do Docker
COPY ["PortalIRibeiro.API/PortalIRibeiro.API.csproj", "PortalIRibeiro.API/"]
RUN dotnet restore "PortalIRibeiro.API/PortalIRibeiro.API.csproj"

# Copia o restante do código da API e builda
COPY PortalIRibeiro.API/ PortalIRibeiro.API/
WORKDIR "/src/PortalIRibeiro.API"
RUN dotnet build "PortalIRibeiro.API.csproj" -c Release -o /app/build

# Publica a aplicação compilada
FROM build AS publish
RUN dotnet publish "PortalIRibeiro.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Estágio Final/Runtime (Imagem leve do ASP.NET Core 10)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Koyeb usa a porta 8080 ou 80 por padrão, vamos expor a 8080 que é o novo padrão do .NET 8+
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "PortalIRibeiro.API.dll"]