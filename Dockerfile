# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy project file and restore
COPY ["HelpEmpowermentApi.csproj", "./"]
RUN dotnet restore "HelpEmpowermentApi.csproj"

# Copy everything and build
COPY . .
RUN dotnet build "HelpEmpowermentApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "HelpEmpowermentApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app

# Copy published files
COPY --from=publish /app/publish .

# ✅ Explicitly copy Google credentials from source
# This ensures the file is available even if publish didn't include it
COPY --from=build /src/Common/test-erp-68be7-b83f4e97f6be.json /app/Common/test-erp-68be7-b83f4e97f6be.json

# Verify file exists (optional - for debugging)
RUN ls -la /app/Common/ || echo "Common directory not found"

ENTRYPOINT ["dotnet", "HelpEmpowermentApi.dll"]