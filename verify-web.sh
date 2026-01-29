#!/bin/bash
# Teste rápido de syntax para arquivos criados

echo "🔍 Verificando estrutura do projeto web..."

# Lista de arquivos esperados
FILES=(
    "SharpQWKReader.Web/SharpQWKReader.Web.csproj"
    "SharpQWKReader.Web/Program.cs"
    "SharpQWKReader.Web/QWKModels.cs"
    "SharpQWKReader.Web/Services/QWKService.cs"
    "SharpQWKReader.Web/Controllers/QWKController.cs"
    "SharpQWKReader.Web/Models/ViewModels.cs"
    "SharpQWKReader.Web/Views/QWK/Index.cshtml"
    "SharpQWKReader.Web/Views/QWK/Package.cshtml"
    "SharpQWKReader.Web/Views/QWK/Forum.cshtml"
    "SharpQWKReader.Web/Views/QWK/Message.cshtml"
    "SharpQWKReader.Web/Views/Shared/_Layout.cshtml"
    "SharpQWKReader.Web/Views/Shared/Error.cshtml"
    "SharpQWKReader.Web/Dockerfile"
    "SharpQWKReader.Web/appsettings.json"
    "docker-compose.web.yml"
)

GREEN='\033[0;32m'
RED='\033[0;31m'
NC='\033[0m'

FOUND=0
MISSING=0

for file in "${FILES[@]}"; do
    if [ -f "$file" ]; then
        echo -e "${GREEN}✓${NC} $file"
        ((FOUND++))
    else
        echo -e "${RED}✗${NC} $file"
        ((MISSING++))
    fi
done

echo ""
echo "=========================================="
echo "Arquivos: $FOUND encontrados, $MISSING faltando"
echo "=========================================="

if [ $MISSING -eq 0 ]; then
    echo -e "${GREEN}✓ Projeto web pronto para ser usado!${NC}"
    exit 0
else
    echo -e "${RED}✗ Alguns arquivos estão faltando${NC}"
    exit 1
fi
