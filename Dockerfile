# Stage 1: Build
FROM mcr.microsoft.com/dotnet/framework/sdk:4.8 AS builder

WORKDIR /src

# Copiar arquivos de solução e projeto
COPY . .

# Restaurar pacotes NuGet
RUN nuget restore SharpQWKReader.sln

# Compilar a solução
RUN msbuild SharpQWKReader.sln /p:Configuration=Release /p:Platform="Any CPU"

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/framework/aspnet:4.8

WORKDIR /app

# Copiar binários compilados do stage anterior
COPY --from=builder /src/SharpQWKReader/bin/Release ./SharpQWKReader/bin/Release
COPY --from=builder /src/QWK/bin/Debug ./QWK/bin/Debug

# Para Windows Forms, seria necessário um servidor X
# Esta imagem é mais adequada para executar aplicações de console ou ASP.NET

ENTRYPOINT ["cmd.exe"]
