# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Ecommerce.Api/Ecommerce.Api.csproj", "src/Ecommerce.Api/"]
COPY ["src/Modules/Catalog/Catalog.ApiContracts/Catalog.ApiContracts.csproj", "src/Modules/Catalog/Catalog.ApiContracts/"]
COPY ["src/Modules/Catalog/Catalog.Core/Catalog.Core.csproj", "src/Modules/Catalog/Catalog.Core/"]
COPY ["src/Shared/Shared.Abstractions/Shared.Abstractions.csproj", "src/Shared/Shared.Abstractions/"]
COPY ["src/Shared/Shared.Infrastructure/Shared.Infrastructure.csproj", "src/Shared/Shared.Infrastructure/"]
COPY ["src/Modules/Ordering/Ordering.Contracts/Ordering.Contracts.csproj", "src/Modules/Ordering/Ordering.Contracts/"]
COPY ["src/Modules/Catalog/Catalog.Contracts/Catalog.Contracts.csproj", "src/Modules/Catalog/Catalog.Contracts/"]
COPY ["src/Modules/Ordering/Ordering.ApiContracts/Ordering.ApiContracts.csproj", "src/Modules/Ordering/Ordering.ApiContracts/"]
COPY ["src/Modules/Ordering/Ordering.Core/Ordering.Core.csproj", "src/Modules/Ordering/Ordering.Core/"]
COPY ["src/Modules/Pay/Pay.Contracts/Pay.Contracts.csproj", "src/Modules/Pay/Pay.Contracts/"]
COPY ["src/Modules/Pay/Pay.ApiContracts/Pay.ApiContracts.csproj", "src/Modules/Pay/Pay.ApiContracts/"]
COPY ["src/Modules/Pay/Pay.Core/Pay.Core.csproj", "src/Modules/Pay/Pay.Core/"]
COPY ["src/Modules/Reporting/Reporting.Core/Reporting.Core.csproj", "src/Modules/Reporting/Reporting.Core/"]
RUN dotnet restore "./src/Ecommerce.Api/Ecommerce.Api.csproj"
COPY . .
WORKDIR "/src/src/Ecommerce.Api"
RUN dotnet build "./Ecommerce.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Ecommerce.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Ecommerce.Api.dll"]