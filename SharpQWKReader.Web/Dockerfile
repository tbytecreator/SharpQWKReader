FROM mcr.microsoft.com/dotnet/sdk:8.0 AS builder

WORKDIR /src

COPY . .

RUN dotnet restore "SharpQWKReader.Web.csproj"
RUN dotnet publish "SharpQWKReader.Web.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0

WORKDIR /app

COPY --from=builder /app/publish .

EXPOSE 80
EXPOSE 443

ENTRYPOINT ["dotnet", "SharpQWKReader.Web.dll"]
