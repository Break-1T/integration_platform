#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/integration_platform/integration_platform.csproj", "src/integration_platform/"]
COPY ["src/integration_platform.database/integration_platform.database.csproj", "src/integration_platform.database/"]
RUN dotnet restore "./src/integration_platform/integration_platform.csproj"
COPY . .
WORKDIR "/src/src/integration_platform"
RUN dotnet build "./integration_platform.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./integration_platform.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "integration_platform.dll"]