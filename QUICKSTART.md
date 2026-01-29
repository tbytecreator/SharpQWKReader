# 🚀 Quick Start Guide

## 30 Segundos para Rodar a Aplicação

### Opção 1: Docker Compose (Mais Fácil)
```bash
cd /home/tbytecreator/Dev/tremyen/SharpQWKReader
docker-compose -f docker-compose.web.yml up --build
```
Acesse: **http://localhost**

---

### Opção 2: Localmente (Requer .NET 8.0)
```bash
cd SharpQWKReader.Web
dotnet restore
dotnet run
```
Acesse: **http://localhost:5000**

---

### Opção 3: Script Automático
```bash
chmod +x run-web.sh
./run-web.sh
```
Acesse: **http://localhost**

---

## 📋 Requisitos

| Opção | Requisitos |
|---|---|
| Docker | Docker Desktop |
| Localmente | .NET 8.0 SDK |
| Script | Docker + Bash |

---

## 📂 Estrutura de Pastas

```
SharpQWKReader/
├── QWK/                     # Biblioteca (leitura QWK)
├── SharpQWKReader/          # App Desktop antigo
├── SharpQWKReader.Web/ ⭐   # App Web NOVO
├── docker-compose.web.yml   # Compose web
├── run-web.sh              # Script runner
└── README_WEB.md           # Docs completo
```

---

## 🎯 Fluxo de Uso

```
1. Acesse http://localhost
2. Clique em "Upload de Arquivo"
3. Selecione um arquivo .qwk
4. Veja BBS Info e Fóruns
5. Clique em um Fórum
6. Selecione uma Mensagem
7. Leia o conteúdo completo
```

---

## 🔍 Verificar Instalação

```bash
# Verify Docker
docker --version

# Verify .NET
dotnet --version

# Verify estrutura
chmod +x verify-web.sh
./verify-web.sh
```

---

## 🐛 Troubleshooting

### Porta 80 ocupada
```bash
docker-compose -f docker-compose.web.yml down
docker run -d -p 8080:80 sharpqwk-web:latest
```

### Limpar tudo
```bash
docker system prune -a
docker volume prune
```

### Ver logs
```bash
docker logs -f sharpqwk-web
```

---

## 📊 Status da Aplicação

- **Framework:** ASP.NET Core 8.0 ✅
- **Frontend:** Bootstrap 5 ✅
- **Backend:** MVC Pattern ✅
- **Docker:** Multi-stage ✅
- **Database:** Pronto (pode adicionar) 📋

---

## 📞 Support

Veja documentação completa em:
- `README_WEB.md` - Guia completo
- `WEB_REFACTORING.md` - Detalhes técnicos
- `REFACTORING_SUMMARY.md` - Análise antes/depois

---

**Criado com ❤️ em C# e .NET**
