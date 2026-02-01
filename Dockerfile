# Base image for runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy csproj and restore
COPY ["HelpEmpowermentApi.csproj", "./"]
RUN dotnet restore "HelpEmpowermentApi.csproj"

# Copy everything else and build
COPY . .
RUN dotnet build "HelpEmpowermentApi.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "HelpEmpowermentApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final image
FROM base AS final
WORKDIR /app

# Install curl for healthcheck
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

COPY --from=publish /app/publish .

# Healthcheck (optional, adjust endpoint if different)
HEALTHCHECK --interval=30s --timeout=10s --start-period=60s --retries=5 \
  CMD curl --fail http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "HelpEmpowermentApi.dll"]
