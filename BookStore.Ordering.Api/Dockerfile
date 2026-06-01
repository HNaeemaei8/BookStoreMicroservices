FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY BookStore.Ordering.API/BookStore.Ordering.API.csproj BookStore.Ordering.API/
COPY BookStore.Ordering.Application/BookStore.Ordering.Application.csproj BookStore.Ordering.Application/
COPY BookStore.Ordering.Infrastructure/BookStore.Ordering.Infrastructure.csproj BookStore.Ordering.Infrastructure/
COPY BookStore.Shared.API/BookStore.Shared.API.csproj BookStore.Shared.API/

RUN dotnet restore BookStore.Ordering.API/BookStore.Ordering.API.csproj

COPY . .

RUN dotnet publish BookStore.Ordering.API/BookStore.Ordering.API.csproj -c Release -o /app/publish

FROM base AS final
WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "BookStore.Ordering.API.dll"]