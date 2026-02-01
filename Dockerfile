# =========================
# Base runtime image
# =========================
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 5000
EXPOSE 5001

# =========================
# Build stage
# =========================
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy csproj and restore
COPY ["HelpEmpowermentApi.csproj", "./"]
RUN dotnet restore "HelpEmpowermentApi.csproj"

# Copy everything else and build
COPY . .
RUN dotnet build "HelpEmpowermentApi.csproj" -c Release -o /app/build

# =========================
# Publish stage
# =========================
FROM build AS publish
RUN dotnet publish "HelpEmpowermentApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

# =========================
# Final runtime image
# =========================
FROM base AS final
WORKDIR /app

# Install curl (optional for healthcheck)
RUN apt-get update && apt-get install -
