#!/bin/bash
# Script para criar e executar a aplicação web

set -e

PROJECT_DIR="/home/tbytecreator/Dev/tremyen/SharpQWKReader/SharpQWKReader.Web"
WEB_IMAGE="sharpqwk-web:latest"
WEB_CONTAINER="sharpqwk-web"

echo "=========================================="
echo "SharpQWK Reader - Web App Build"
echo "=========================================="

RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m'

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

# Parar container anterior
if [ "$(docker ps -q -f name=$WEB_CONTAINER)" ]; then
    print_status "Parando container: $WEB_CONTAINER"
    docker stop $WEB_CONTAINER
fi

if [ "$(docker ps -aq -f name=$WEB_CONTAINER)" ]; then
    print_status "Removendo container anterior"
    docker rm $WEB_CONTAINER
fi

# Build
print_status "Construindo imagem: $WEB_IMAGE"
cd /home/tbytecreator/Dev/tremyen/SharpQWKReader
docker build -t $WEB_IMAGE -f SharpQWKReader.Web/Dockerfile .

if [ $? -eq 0 ]; then
    print_status "Imagem construída com sucesso!"
else
    print_error "Falha ao construir!"
    exit 1
fi

# Run
print_status "Iniciando aplicação..."
docker run -d \
    --name $WEB_CONTAINER \
    -p 80:80 \
    -p 443:443 \
    -e ASPNETCORE_ENVIRONMENT=Production \
    -v $(pwd)/uploads:/app/uploads \
    $WEB_IMAGE

print_status "Container iniciado!"
print_status "Acesse: http://localhost"
print_status "Logs: docker logs -f $WEB_CONTAINER"

echo "=========================================="
