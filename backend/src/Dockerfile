# syntax=docker/dockerfile:1

# ---- build stage ----
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Restore first (better layer caching): copy only project files.
COPY Fundo.Applications.WebApi/Fundo.Applications.WebApi.csproj Fundo.Applications.WebApi/
COPY Fundo.Applications.Domain/Fundo.Applications.Domain.csproj Fundo.Applications.Domain/
COPY Fundo.Applications.Data/Fundo.Applications.Data.csproj Fundo.Applications.Data/
RUN dotnet restore Fundo.Applications.WebApi/Fundo.Applications.WebApi.csproj

# Copy the rest and publish.
COPY . .
RUN dotnet publish Fundo.Applications.WebApi/Fundo.Applications.WebApi.csproj \
    -c Release -o /app --no-restore

# ---- runtime stage ----
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "Fundo.Applications.WebApi.dll"]
