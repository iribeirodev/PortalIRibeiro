#!/bin/sh
# Ativa os hooks versionados (.githooks) para este repositório
set -e

git config core.hooksPath .githooks
echo "Hooks ativados: $(git config core.hooksPath)"
