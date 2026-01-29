#!/bin/bash
# Script para construir e executar o container Docker do SharpQWKReader

set -e

CONTAINER_NAME="sharpqwkreader"
IMAGE_NAME="sharpqwkreader:latest"

echo "=========================================="
echo "SharpQWKReader - Docker Build Script"
echo "=========================================="

# Cores para output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Função para imprimir mensagens
print_status() {
    echo -e "${GREEN}[INFO]${NC} $1"
}

print_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

print_warning() {
    echo -e "${YELLOW}[WARNING]${NC} $1"
}

# Verificar se Docker está instalado
if ! command -v docker &> /dev/null; then
    print_error "Docker não está instalado!"
    exit 1
fi

print_status "Docker encontrado: $(docker --version)"

# Parar container anterior se existir
if [ "$(docker ps -q -f name=$CONTAINER_NAME)" ]; then
    print_status "Parando container existente: $CONTAINER_NAME"
    docker stop $CONTAINER_NAME
fi

if [ "$(docker ps -aq -f name=$CONTAINER_NAME)" ]; then
    print_status "Removendo container anterior: $CONTAINER_NAME"
    docker rm $CONTAINER_NAME
fi

# Build da imagem
print_status "Construindo imagem Docker: $IMAGE_NAME"
docker build -t $IMAGE_NAME .

if [ $? -eq 0 ]; then
    print_status "Imagem construída com sucesso!"
else
    print_error "Falha ao construir a imagem!"
    exit 1
fi

# Executar container
print_status "Iniciando container: $CONTAINER_NAME"
docker run -d \
    --name $CONTAINER_NAME \
    -v $(pwd)/data:/app/data \
    $IMAGE_NAME

print_status "Container iniciado com ID: $(docker ps -q -f name=$CONTAINER_NAME)"
print_status "Para ver os logs: docker logs -f $CONTAINER_NAME"
print_status "Para parar o container: docker stop $CONTAINER_NAME"

echo "=========================================="
echo "Concluído!"
echo "=========================================="
