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
# Lê o .env preservando valores com espaços/quotes e sem expor nada no script
if [ -f .env ]; then
    set -a
    while IFS='=' read -r key value; do
        case "$key" in ''|\#*) continue ;; esac
        export "$key=${value//\"/}"
    done < .env
    set +a
else
    echo "Erro: Arquivo .env não encontrado"
    exit 1
fi

# Ajusta o Host para o Docker se a string apontar para localhost
DB_CONNECTION=$(echo "$ConnectionStrings__DefaultConnection" | sed 's/localhost/host.docker.internal/g')
REDIS_CONNECTION=$(echo "$ConnectionStrings__Redis" | sed 's/localhost/host.docker.internal/g')

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
  -e ConnectionStrings__DefaultConnection="$DB_CONNECTION" \
  -e ConnectionStrings__Redis="$REDIS_CONNECTION" \
  -e Gemini__ApiKey="$Gemini__ApiKey" \
  -e ASPNETCORE_ENVIRONMENT="$ASPNETCORE_ENVIRONMENT" \
  -e GeoIp__BaseUrl="$GeoIp__BaseUrl" \
  -e GeoIp__FallbackIp="$GeoIp__FallbackIp" \
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