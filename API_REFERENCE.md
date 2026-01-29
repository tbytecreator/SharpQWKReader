# SharpQWK Reader Web - API Reference

## 🔌 Endpoints da Aplicação

### 📌 Base URL
```
http://localhost       # Com Docker
http://localhost:5000  # Com dotnet run
```

---

## 📋 Endpoints

### 1. GET `/` ou `/qwk/index`
**Descrição:** Página inicial com formulário de upload

**Exemplo de Requisição:**
```http
GET / HTTP/1.1
Host: localhost
```

**Resposta:**
```html
<!-- Formulário de upload -->
```

**Status:** 200 OK

---

### 2. POST `/qwk/uploadpackage`
**Descrição:** Upload e processamento do arquivo QWK

**Parâmetros:**
- `file` (FormFile) - Arquivo .qwk

**Exemplo de Requisição:**
```http
POST /qwk/uploadpackage HTTP/1.1
Host: localhost
Content-Type: multipart/form-data

--boundary
Content-Disposition: form-data; name="file"; filename="ABUTRE2.QWK"

[arquivo binário]
--boundary--
```

**Resposta (Sucesso):**
```
HTTP/1.1 302 Found
Location: /qwk/package?bbsId=MYBBS
```

**Resposta (Erro):**
```
HTTP/1.1 200 OK
Content-Type: text/html

<!-- Página com mensagem de erro -->
```

**Validações:**
- ✅ Arquivo obrigatório
- ✅ Extensão .qwk recomendada
- ✅ Tamanho máximo: Configurável

---

### 3. GET `/qwk/package`
**Descrição:** Exibe informações do BBS e lista de fóruns

**Parâmetros Query:**
- `bbsId` (optional) - ID do BBS

**Exemplo de Requisição:**
```http
GET /qwk/package?bbsId=MYBBS HTTP/1.1
Host: localhost
```

**Resposta (200 OK):**
```html
<div>
  <h1>BBS Name</h1>
  <p>Location: City, State</p>
  <p>Phone: 555-1212</p>
  
  <div class="forums">
    <!-- Lista de fóruns -->
  </div>
</div>
```

**Dados Exibidos:**
```json
{
  "bbsInfo": {
    "bbsName": "My BBS",
    "bbsLocation": "New York, NY",
    "bbsPhone": "212-555-1212",
    "sysopName": "John Doe",
    "userName": "JANE DOE",
    "messagesInPacket": 999
  },
  "forums": [
    {
      "id": "0",
      "name": "Main Board",
      "numberOfMessages": 45
    }
  ]
}
```

---

### 4. GET `/qwk/forum/{forumId}`
**Descrição:** Lista todas as mensagens de um fórum

**Parâmetros:**
- `forumId` (path) - ID do fórum (string)

**Exemplo de Requisição:**
```http
GET /qwk/forum/0 HTTP/1.1
Host: localhost
```

**Resposta (200 OK):**
```html
<table>
  <thead>
    <tr>
      <th>#</th>
      <th>From</th>
      <th>To</th>
      <th>Subject</th>
      <th>Date</th>
      <th>Action</th>
    </tr>
  </thead>
  <tbody>
    <tr>
      <td>1</td>
      <td>John</td>
      <td>Jane</td>
      <td>Hello</td>
      <td>01-01-2025</td>
      <td><a href="/qwk/message/0">Read</a></td>
    </tr>
  </tbody>
</table>
```

**Dados Estruturados:**
```json
{
  "forumId": "0",
  "messages": [
    {
      "index": 0,
      "from": "John Doe",
      "to": "Jane Doe",
      "subject": "Hello World",
      "date": "01-01-2025",
      "time": "12:34:56",
      "number": "00000001",
      "statusFlag": "*",
      "deleteFlag": " "
    }
  ]
}
```

---

### 5. GET `/qwk/message/{messageNumber}`
**Descrição:** Exibe o conteúdo completo de uma mensagem

**Parâmetros:**
- `messageNumber` (path) - Número sequencial (ulong)

**Exemplo de Requisição:**
```http
GET /qwk/message/5 HTTP/1.1
Host: localhost
```

**Resposta (200 OK):**
```html
<div class="message">
  <h4>Message Subject</h4>
  
  <div class="meta">
    <strong>From:</strong> John Doe<br>
    <strong>To:</strong> Jane Doe<br>
    <strong>Date:</strong> 01-01-2025 12:34:56
  </div>
  
  <div class="body">
    [Corpo da mensagem em monospace]
  </div>
</div>
```

**Dados Estruturados:**
```json
{
  "index": 5,
  "number": "00000005",
  "from": "John Doe",
  "to": "Jane Doe",
  "subject": "Hello",
  "date": "01-01-2025",
  "time": "12:34:56",
  "body": "Este é o corpo da mensagem...",
  "referenceMessageNumber": "00000000",
  "conferenceNumber": "0",
  "statusFlag": "*",
  "deleteFlag": " "
}
```

---

## 🔄 Session Management

A aplicação usa **Session** para armazenar:

```csharp
// Armazenar após upload
HttpContext.Session.SetString("PackagePath", "/path/to/file.qwk");

// Recuperar em requisições subsequentes
var path = HttpContext.Session.GetString("PackagePath");
```

**Duração:** Padrão do ASP.NET Core (20 minutos)

---

## 📊 HTTP Status Codes

| Código | Situação | Exemplo |
|--------|----------|---------|
| 200 | OK - Requisição bem-sucedida | GET /qwk/package |
| 302 | Redirect - Upload com sucesso | POST /qwk/uploadpackage |
| 400 | Bad Request - Parâmetros inválidos | Falta arquivo |
| 404 | Not Found - Fórum/mensagem não existe | GET /qwk/message/999 |
| 500 | Server Error - Erro na aplicação | Erro ao processar QWK |

---

## 🔐 Validações

### Upload
```csharp
if (file == null || file.Length == 0)
    return BadRequest("Arquivo obrigatório");
```

### Acesso de Sessão
```csharp
var path = HttpContext.Session.GetString("PackagePath");
if (string.IsNullOrEmpty(path))
    return Redirect("/");  // Redireciona se sessão expirou
```

### Parâmetros
```csharp
if (string.IsNullOrEmpty(forumId))
    return BadRequest("ForumId obrigatório");
```

---

## 🔀 Content-Type

| Endpoint | Content-Type |
|----------|-------------|
| GET / | text/html; charset=utf-8 |
| POST /upload | multipart/form-data |
| GET /package | text/html; charset=utf-8 |
| GET /forum/{id} | text/html; charset=utf-8 |
| GET /message/{id} | text/html; charset=utf-8 |

---

## 🔄 Fluxo de Sessão

```
1. Acessar / → Sessão criada
   ↓
2. POST /qwk/uploadpackage com arquivo
   → Session["PackagePath"] = path
   → Redirect para /qwk/package
   ↓
3. GET /qwk/package
   → Lê Session["PackagePath"]
   → Mostra BBS Info
   ↓
4. GET /qwk/forum/{id}
   → Continua usando Session["PackagePath"]
   ↓
5. GET /qwk/message/{num}
   → Continua usando Session["PackagePath"]
   ↓
6. Sessão expira ou logout
   → Session limpada
   → Próximo acesso redireciona para /
```

---

## 📝 Tratamento de Erros

### Erro de Upload
```html
<div class="alert alert-danger">
  Error: File not found or invalid QWK package
</div>
```

### Erro de Processamento
```html
<div class="alert alert-warning">
  Error: Unable to parse QWK package
</div>
```

### Logging
```csharp
_logger.LogError(ex, "Error uploading package");
_logger.LogInformation($"QWK packet opened: {packetPath}");
```

---

## 🚀 Performance

| Operação | Tempo Típico |
|----------|-------------|
| Upload 10 MB | 1-2 segundos |
| Parse CONTROL.DAT | <100ms |
| Listar 100 mensagens | <50ms |
| Ler mensagem | <10ms |

---

## 🔗 Navegação em Links

```html
<!-- Upload Form -->
<form method="post" action="/qwk/uploadpackage">

<!-- Forum Link -->
<a href="/qwk/forum/0">View Forum</a>

<!-- Message Link -->
<a href="/qwk/message/5">Read Message</a>

<!-- Back Link -->
<a href="javascript:history.back()">Back</a>
```

---

## 📱 Responsividade

Todos os endpoints retornam HTML responsivo com Bootstrap 5:

```css
/* Mobile First */
@media (max-width: 576px) {
  /* Dispositivos pequenos */
}

@media (min-width: 768px) {
  /* Tablets */
}

@media (min-width: 1200px) {
  /* Desktop */
}
```

---

## 🧪 Testando os Endpoints

### Com cURL

```bash
# 1. Acessar homepage
curl http://localhost/

# 2. Upload de arquivo
curl -X POST \
  -F "file=@ABUTRE2.QWK" \
  http://localhost/qwk/uploadpackage

# 3. Ver BBS Info (após upload bem-sucedido)
curl http://localhost/qwk/package

# 4. Listar mensagens
curl http://localhost/qwk/forum/0

# 5. Ler mensagem
curl http://localhost/qwk/message/0
```

### Com Postman

1. Criar nova requisição POST
2. URL: `http://localhost/qwk/uploadpackage`
3. Body → form-data
4. Key: `file`, Value: selecionar arquivo .qwk
5. Send

---

## 📚 Integração Futura com API REST

Para criar uma API REST completa, adicione em `Program.cs`:

```csharp
builder.Services.AddControllers();

app.MapControllers();  // Em vez de MapControllerRoute
```

Então crie `ApiController.cs`:

```csharp
[ApiController]
[Route("api/[controller]")]
public class QWKApiController : ControllerBase
{
    [HttpGet("forums")]
    public IActionResult GetForums() { }
    
    [HttpGet("messages/{forumId}")]
    public IActionResult GetMessages(string forumId) { }
    
    [HttpGet("message/{id}")]
    public IActionResult GetMessage(ulong id) { }
}
```

---

## 🔗 Relacionamentos

```
Upload (.qwk file)
  ↓
CONTROL.DAT parsing
  ↓
BBSInfo extracted
  ↓
Forums list (indexes: 0, 1, 2, ...)
  ↓
Forum selected
  ↓
Messages (NDX + DAT files)
  ↓
Message selected
  ↓
Message body displayed
```

---

**Documentação de API - v1.0**
*Última atualização: 29 de Janeiro de 2026*
