FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/TransportService.API/TransportService.API.csproj", "src/TransportService.API/"]
COPY ["src/TransportService.Core/TransportService.Core.csproj", "src/TransportService.Core/"]
COPY ["src/TransportService.Infrastructure/TransportService.Infrastructure.csproj", "src/TransportService.Infrastructure/"]
RUN dotnet restore "./src/TransportService.API/TransportService.API.csproj"
COPY . .
WORKDIR "/src/src/TransportService.API"
RUN dotnet build "./TransportService.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./TransportService.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TransportService.API.dll"]