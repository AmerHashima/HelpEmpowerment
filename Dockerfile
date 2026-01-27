# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY ["StandardArticture.csproj", "./"]
RUN dotnet restore "StandardArticture.csproj"

# Copy everything else and build
COPY . .
RUN dotnet build "StandardArtiture.csproj" -c Release -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
RUN dotnet publish "StandardArticture.csproj" -c Release -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Install SQL Server tools for troubleshooting (optional)
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

COPY --from=publish /app/publish .

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=60s --retries=3 \
  CMD curl --fail http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "StandardArticture.dll"]