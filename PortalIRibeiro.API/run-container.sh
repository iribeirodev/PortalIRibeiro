#!/bin/bash

# Interrompe o script imediatamente se qualquer comando falhar
set -e

# ============================================================================
# CONFIGURAÇÕES DO CONTAINER E IMAGEM
# ============================================================================
CONTAINER_NAME="portal_api_teste"
IMAGE_NAME="portal-iribeiro-api:local"
LOCAL_PORT=5000
DOCKER_PORT=8080

# ============================================================================
# EXTRAÇÃO AUTOMÁTICA DO ARQUIVO .ENV LOCAL
# ============================================================================
# Esse bloco lê o .env real e isola as variáveis sem expor nada no script
if [ -f .env ]; then
    export $(cat .env | grep -v '^#' | xargs)
else
    echo "Erro: Arquivo .env não encontrado"
    exit 1
fi

# Ajusta o Host para o Docker se a string apontar para localhost
DB_CONNECTION=$ConnectionStrings__DefaultConnection
REDIS_CONNECTION=$(echo $ConnectionStrings__Redis | sed 's/localhost/host.docker.internal/g')

echo "======================================================="
echo " 1/3: GERANDO NOVA IMAGEM DOCKER (BUILD)"
echo "======================================================="
docker build -t $IMAGE_NAME ..

echo ""
echo "======================================================="
echo " 2/3: LIMPANDO CONTAINERS ANTIGOS (Se existirem)"
echo "======================================================="
# Desativa temporariamente o 'set -e' porque o docker rm falhar se o container não existir
set +e
docker rm -f $CONTAINER_NAME 2>/dev/null
set -e

echo ""
echo "======================================================="
echo " 3/3: EXECUTANDO O NOVO CONTAINER (RUN)"
echo "======================================================="
docker run -d \
  --name "$CONTAINER_NAME" \
  -p $LOCAL_PORT:$DOCKER_PORT \
  -e ConnectionStrings__DefaultConnection="Host=172.17.0.1;Database=portal;Username=admin;Password=california;SslMode=Disable;TrustServerCertificate=true;" \
  -e ConnectionStrings__Redis="$ConnectionStrings__Redis" \
  -e Gemini__ApiKey="$Gemini__ApiKey" \
  -e ASPNETCORE_ENVIRONMENT="$ASPNETCORE_ENVIRONMENT" \
  $IMAGE_NAME

echo "-------------------------------------------------------"
echo " Processo concluído com sucesso!"
echo "-------------------------------------------------------"
sleep 2
docker ps -f name=$CONTAINER_NAME

echo ""
echo "Para testar o Health Check, rode:"
echo "curl -i http://localhost:$LOCAL_PORT/health"
echo ""
echo "Para ver os logs em tempo real, rode:"
echo "docker logs -f $CONTAINER_NAME"
echo "-------------------------------------------------------"