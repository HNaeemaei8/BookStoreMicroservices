FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY BookStore.Catalog.API/BookStore.Catalog.API.csproj BookStore.Catalog.API/
COPY BookStore.Catalog.Infrastructure/BookStore.Catalog.Infrastructure.csproj BookStore.Catalog.Infrastructure/
COPY BookStore.Catalog.Application/BookStore.Catalog.Application.csproj BookStore.Catalog.Application/

RUN dotnet restore BookStore.Catalog.API/BookStore.Catalog.API.csproj

COPY . .

RUN dotnet publish BookStore.Catalog.API/BookStore.Catalog.API.csproj -c Release -o /app/publish

FROM base AS final
WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "BookStore.Catalog.API.dll"]