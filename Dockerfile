# Estágio de Build (SDK + Ferramentas de Compilação C++ para Native AOT)
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Instala ferramentas necessárias para a compilação nativa (clang e zlib)
RUN apt-get update && apt-get install -y --no-install-recommends \
    clang \
    zlib1g-dev \
    && rm -rf /var/lib/apt/lists/*

# Copia e restaura dependências (otimização de cache)
COPY ["PortalIRibeiro.API/PortalIRibeiro.API.csproj", "PortalIRibeiro.API/"]
RUN dotnet restore "PortalIRibeiro.API/PortalIRibeiro.API.csproj"

# Copia o restante do código
COPY PortalIRibeiro.API/ PortalIRibeiro.API/
WORKDIR "/src/PortalIRibeiro.API"

# Publica o binário nativo autônomo (Self-Contained + Linux-x64)
RUN dotnet publish "PortalIRibeiro.API.csproj" \
    -c Release \
    -r linux-x64 \
    --self-contained true \
    -o /app/publish

# Estágio Final (Apenas dependências de OS / imagem ultra leve)
FROM mcr.microsoft.com/dotnet/runtime-deps:10.0 AS final
WORKDIR /app

# Copia os arquivos gerados no publish nativo
COPY --from=build /app/publish .

# Expõe a porta padrão do Koyeb e container .NET (8080)
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

# Executa diretamente o binário nativo compilado
ENTRYPOINT ["./PortalIRibeiro.API"]