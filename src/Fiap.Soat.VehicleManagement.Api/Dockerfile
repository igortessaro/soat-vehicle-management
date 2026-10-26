FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Fiap.Soat.VehicleManagement.Api/Fiap.Soat.VehicleManagement.Api.csproj", "src/Fiap.Soat.VehicleManagement.Api/"]
COPY ["src/Fiap.Soat.VehicleManagement.Infrastructure/Fiap.Soat.VehicleManagement.Infrastructure.csproj", "src/Fiap.Soat.VehicleManagement.Infrastructure/"]
COPY ["src/Fiap.Soat.VehicleManagement.Application/Fiap.Soat.VehicleManagement.Application.csproj", "src/Fiap.Soat.VehicleManagement.Application/"]
COPY ["src/Fiap.Soat.VehicleManagement.Domain/Fiap.Soat.VehicleManagement.Domain.csproj", "src/Fiap.Soat.VehicleManagement.Domain/"]
RUN dotnet restore "src/Fiap.Soat.VehicleManagement.Api/Fiap.Soat.VehicleManagement.Api.csproj"
COPY . .
WORKDIR "/src/src/Fiap.Soat.VehicleManagement.Api"
RUN dotnet build "./Fiap.Soat.VehicleManagement.Api.csproj" \
    -c "$BUILD_CONFIGURATION" \
    -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Fiap.Soat.VehicleManagement.Api.csproj" \
    -c "$BUILD_CONFIGURATION" \
    -o /app/publish \
    /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Fiap.Soat.VehicleManagement.Api.dll"]
