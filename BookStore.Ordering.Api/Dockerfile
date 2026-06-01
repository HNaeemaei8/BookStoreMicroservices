FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY . .

RUN dotnet restore "BookStore.Ordering.API/BookStore.Ordering.API.csproj"

RUN dotnet publish "BookStore.Ordering.API/BookStore.Ordering.API.csproj" \
-c Release \
    -o /app/publish

FROM base AS final
WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet","BookStore.Ordering.API.dll"]