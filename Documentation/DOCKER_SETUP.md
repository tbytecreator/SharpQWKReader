# Docker Setup para SharpQWKReader

Esse arquivo contém instruções para executar a solução C# SharpQWKReader em um container Docker.

## Requisitos

- **Docker** instalado no sistema
- **Docker Compose** (opcional, mas recomendado)

## Quick Start

### Opção 1: Usando Docker Compose (Recomendado)

⚠️ **IMPORTANTE:** Use o arquivo `docker-compose.web.yml` (não o antigo `docker-compose.yml`)

```bash
# Construir e executar (WEB VERSION - RECOMENDADO)
docker-compose -f docker-compose.web.yml up --build

# Executar em background
docker-compose -f docker-compose.web.yml up -d --build

# Ver logs
docker-compose -f docker-compose.web.yml logs -f

# Parar
docker-compose -f docker-compose.web.yml down

# Acessar: http://localhost
```

**Nota:** O arquivo antigo `docker-compose.yml` era para Windows Forms e não funciona em Linux. Use apenas `docker-compose.web.yml`.

### Opção 2: Usando Script Automático

```bash
# Para a aplicação WEB (recomendado)
chmod +x run-web.sh
./run-web.sh

# Acessar: http://localhost
```

### Opção 3: Comandos Docker Manuais

```bash
# Construir a imagem
docker build -t sharpqwkreader:latest .

# Executar o container
docker run -d \
  --name sharpqwkreader \
  -v $(pwd)/data:/app/data \
  sharpqwkreader:latest

# Ver logs
docker logs -f sharpqwkreader

# Parar o container
docker stop sharpqwkreader
docker rm sharpqwkreader
```

## Estrutura

**Para a versão WEB (recomendado):**
- **Dockerfile.web** - Multi-stage build para ASP.NET Core 8.0
- **docker-compose.web.yml** - Configuração para a aplicação Web
- **.dockerignore** - Arquivos a ignorar durante build

**Antigos (não funciona em Linux):**
- **Dockerfile** - Para Windows Forms (.NET Framework 4.8 - descontinuado)
- **docker-compose.yml** - Para Windows Forms (descontinuado)

## Volumes

- `/app/data` - Diretório para armazenar/acessar arquivos QWK

## Notas Importantes

### Aplicação Web vs Windows Forms

**VERSÃO WEB (RECOMENDADA - FUNCIONA EM LINUX)**
- ✅ ASP.NET Core 8.0
- ✅ Funciona em Docker Linux
- ✅ Interface responsiva
- ✅ Acessível via http://localhost
- ✅ Use: `docker-compose.web.yml`

**VERSÃO ANTIGO (NÃO FUNCIONA EM LINUX)**
- ❌ Windows Forms (.NET Framework 4.8)
- ❌ Não funciona em Linux/Docker
- ❌ Apenas para Windows Desktop
- ⚠️ Não use: `docker-compose.yml`

## Troubleshooting

### Erro: "load metadata for mcr.microsoft.com/dotnet/framework/sdk:4.8"

**Solução:** Você está tentando usar o arquivo antigo. Use o novo:
```bash
# ❌ ERRADO (não funciona em Linux)
docker-compose up --build

# ✅ CORRETO (funciona em Linux)
docker-compose -f docker-compose.web.yml up --build
```

### Build falha com erro

```bash
# Limpar cache e tentar novamente
docker system prune -a

# Rebuild com novo arquivo
docker-compose -f docker-compose.web.yml build --no-cache
```

### Container não inicia

```bash
# Ver logs detalhados
docker-compose -f docker-compose.web.yml logs

# Tentar restart
docker-compose -f docker-compose.web.yml restart
```

### Problema com permissões

```bash
chmod +x run-web.sh
```

## Recursos Adicionais

Para mais informações sobre a versão web, consulte:
- **README_WEB.md** - Documentação completa
- **QUICKSTART.md** - Início rápido (30 segundos)
- **API_REFERENCE.md** - Referência de endpoints
- **DEPLOYMENT.md** - Deploy em produção
