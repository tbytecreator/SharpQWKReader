# SharpQWKReader

## Fazer um leitor QWK em C#

Um pacote QWK é um zip file simples com alguns arquivos com layout definido.

⚠️  IMPORTANTE - LEIA ANTES DE CONTINUAR

Este projeto foi REFATORADO de Windows Forms para ASP.NET Core 8.0 WEB.

═══════════════════════════════════════════════════════════════════

✅ USE:
  docker-compose -f docker-compose.web.yml up --build
  docker build -f Dockerfile.web ...
  (Novo arquivo ASP.NET Core 8.0 - funciona em Linux)

═══════════════════════════════════════════════════════════════════

Arquivos Novos (Web - use):
  • Dockerfile.web                  (✅ Novo)
  • docker-compose.web.yml          (✅ Novo)
  • SharpQWKReader.Web/             (✅ Nova aplicação web)
  • run-web.sh                      (✅ Script para iniciar)

═══════════════════════════════════════════════════════════════════

Para mais informações, abra: DOCKER_SETUP.md


## Referencia para o Layot de arquivos de um pacote QWK

http://fileformats.archiveteam.org/wiki/QWK