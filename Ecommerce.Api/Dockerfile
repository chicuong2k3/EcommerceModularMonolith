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
COPY ["Ecommerce.Api/Ecommerce.Api.csproj", "Ecommerce.Api/"]
COPY ["Billing/Billing.ApiContracts/Billing.ApiContracts.csproj", "Billing/Billing.ApiContracts/"]
COPY ["Billing/Billing.Infrastructure/Billing.Infrastructure.csproj", "Billing/Billing.Infrastructure/"]
COPY ["Common/Common.Infrastructure/Common.Infrastructure/Common.Infrastructure.csproj", "Common/Common.Infrastructure/Common.Infrastructure/"]
COPY ["Common/Common.Application/Common.Application.csproj", "Common/Common.Application/"]
COPY ["Common/Common.Domain/Common.Domain.csproj", "Common/Common.Domain/"]
COPY ["Billing/Billing.Application/Billing.Application.csproj", "Billing/Billing.Application/"]
COPY ["Ordering/Ordering.Contracts/Ordering.Contracts.csproj", "Ordering/Ordering.Contracts/"]
COPY ["Billing/Billing.Domain/Billing.Domain.csproj", "Billing/Billing.Domain/"]
COPY ["Catalog/Catalog.ApiContracts/Catalog.ApiContracts.csproj", "Catalog/Catalog.ApiContracts/"]
COPY ["Catalog/Catalog.Infrastructure/Catalog.Infrastructure.csproj", "Catalog/Catalog.Infrastructure/"]
COPY ["Catalog/Catalog.Application/Catalog.Application.csproj", "Catalog/Catalog.Application/"]
COPY ["Catalog/Catalog.Contracts/Catalog.Contracts.csproj", "Catalog/Catalog.Contracts/"]
COPY ["Catalog/Catalog.Domain/Catalog.Domain.csproj", "Catalog/Catalog.Domain/"]
COPY ["Ordering/Ordering.ApiContracts/Ordering.ApiContracts.csproj", "Ordering/Ordering.ApiContracts/"]
COPY ["Ordering/Ordering.Infrastructure/Ordering.Infrastructure.csproj", "Ordering/Ordering.Infrastructure/"]
COPY ["Ordering/Ordering.Application/Ordering.Application.csproj", "Ordering/Ordering.Application/"]
COPY ["Billing/Billing.Contracts/Billing.Contracts.csproj", "Billing/Billing.Contracts/"]
COPY ["Ordering/Ordering.Domain/Ordering.Domain.csproj", "Ordering/Ordering.Domain/"]
RUN dotnet restore "./Ecommerce.Api/Ecommerce.Api.csproj"
COPY . .
WORKDIR "/src/Ecommerce.Api"
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