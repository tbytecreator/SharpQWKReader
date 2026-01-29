# SharpQWK Reader - Web Refactoring Guide

## ✅ Refatoração Concluída

A aplicação foi refatorada de **Windows Forms** para **ASP.NET Core 8.0 Web Application**.

### 📁 Estrutura do Projeto Web

```
SharpQWKReader.Web/
├── Controllers/
│   └── QWKController.cs          # Controladores da aplicação
├── Services/
│   └── QWKService.cs             # Serviço de processamento QWK
├── Models/
│   └── ViewModels.cs             # ViewModels
├── Views/
│   ├── QWK/
│   │   ├── Index.cshtml          # Upload de pacotes
│   │   ├── Package.cshtml        # Visualização de BBS e fóruns
│   │   ├── Forum.cshtml          # Lista de mensagens
│   │   └── Message.cshtml        # Leitura de mensagem
│   └── Shared/
│       ├── _Layout.cshtml        # Layout principal
│       └── _ViewStart.cshtml     # Inicialização de views
├── Program.cs                     # Configuração da aplicação
├── SharpQWKReader.Web.csproj     # Arquivo de projeto
└── Dockerfile                     # Para containerização
```

### 🚀 Como Executar

#### Opção 1: Localmente (Requer .NET 8.0)

```bash
cd SharpQWKReader.Web
dotnet restore
dotnet run
```

Acesse: `http://localhost:5000`

#### Opção 2: Com Docker

```bash
docker-compose -f docker-compose.web.yml up --build
```

Acesse: `http://localhost`

#### Opção 3: Com Docker Compose

```bash
docker-compose -f docker-compose.web.yml up -d
```

### 🎯 Funcionalidades

1. **Upload de Pacotes QWK** - Interface para selecionar arquivo .qwk
2. **Visualização de BBS Info** - Mostra informações do BBS
3. **Listagem de Fóruns** - Navegação entre os fóruns
4. **Leitura de Mensagens** - Visualização completa de mensagens
5. **Interface Responsiva** - Bootstrap 5 para mobile-friendly

### 📦 Dependências

- ASP.NET Core 8.0
- System.IO.Compression.ZipFile
- Bootstrap 5 (CDN)

### 🔄 Fluxo da Aplicação

```
Upload QWK → Parse CONTROL.DAT → Show BBS Info
                              ↓
                          Select Forum
                              ↓
                       View Forum Messages
                              ↓
                       Read Message Body
```

### 🛠️ Tecnologias Utilizadas

- **Framework:** ASP.NET Core 8.0
- **UI Framework:** Bootstrap 5
- **Pattern:** MVC
- **Containerização:** Docker & Docker Compose
- **Linguagem:** C# 12.0

### 📝 Arquivo de Configuração Appsettings

Se necessário criar `appsettings.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  },
  "AllowedHosts": "*"
}
```

### 🐳 Variáveis de Ambiente Docker

- `ASPNETCORE_ENVIRONMENT` - Development/Production
- `ASPNETCORE_URLS` - URL de escuta (padrão: http://+:80)

### 📌 Próximos Passos Opcionais

1. **Database** - Adicionar persistência de dados com Entity Framework Core
2. **Autenticação** - Implementar login/registros
3. **API REST** - Expor endpoints para mobile apps
4. **Cache** - Redis para melhor performance
5. **Tests** - Unit tests com xUnit

### ⚠️ Notas Importantes

- O código de leitura QWK foi mantido intacto para compatibilidade
- Sessão é usada para armazenar o caminho do pacote (considerar implementar melhor para produção)
- Arquivos são salvos em `wwwroot/uploads` (configure em produção)

## Estrutura de Pasta do Projeto

```
SharpQWKReader/
├── QWK/                          # Biblioteca QWK (mantida)
├── SharpQWKReader/               # Projeto antigo (Windows Forms)
├── SharpQWKReader.Web/           # Novo projeto ASP.NET Core
├── docker-compose.yml            # Compose antigo (Windows Forms)
├── docker-compose.web.yml        # Compose novo (Web)
└── README.md
```
