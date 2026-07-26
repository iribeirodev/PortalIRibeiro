#!/usr/bin/env bash
# ==============================================================================
# Script: dump-schema-portal.sh
# Descrição:
#   Realiza o dump completo (DDL + Dados) de todos os objetos contidos no
#   schema 'portal' (tabelas, visões, sequências, funções, etc.) rodando em
#   um container PostgreSQL via Docker.
#
# Pré-requisitos:
#   - Docker em execução na máquina host.
#   - Container do PostgreSQL (postgres17) ativo e saudável.
#
# Comportamento:
#   - Gera um arquivo SQL idempotente (contém DROP IF EXISTS antes do CREATE).
#   - Cria o arquivo com marcação de data e hora para evitar sobrescritas.
#
# Uso:
#   $ chmod +x dump-schema-portal.sh
#   $ ./dump-schema-portal.sh
# ==============================================================================

# Encerra o script se houver erro
set -e

CONTAINER_NAME="postgres17"
DB_USER="admin"
DB_NAME="postgres"
SCHEMA_NAME="portal"

# Define o nome do arquivo com timestamp
TIMESTAMP=$(date +"%Y%m%d_%H%M%S")
OUTPUT_FILE="schema_${SCHEMA_NAME}_dump_${TIMESTAMP}.sql"

echo "🔍 Verificando se o container '$CONTAINER_NAME' está ativo..."

if ! docker ps --format '{{.Names}}' | grep -q "^${CONTAINER_NAME}$"; then
    echo "❌ Erro: O container '$CONTAINER_NAME' não está em execução."
    exit 1
fi

echo "📦 Gerando dump COMPLETO do schema '$SCHEMA_NAME'..."

docker exec -t "$CONTAINER_NAME" pg_dump \
  -U "$DB_USER" \
  -d "$DB_NAME" \
  -n "$SCHEMA_NAME" \
  --clean \
  --if-exists > "$OUTPUT_FILE"

echo "----------------------------------------------------"
echo "🎉 Dump do schema '$SCHEMA_NAME' gerado com sucesso!"
echo "📁 Arquivo: ./$OUTPUT_FILE"
echo "----------------------------------------------------"
