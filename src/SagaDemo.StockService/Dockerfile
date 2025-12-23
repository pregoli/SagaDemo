FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY src/SagaDemo.Contracts/*.csproj src/SagaDemo.Contracts/
COPY src/SagaDemo.Application/*.csproj src/SagaDemo.Application/
COPY src/SagaDemo.Infrastructure/*.csproj src/SagaDemo.Infrastructure/
COPY src/SagaDemo.StockService/*.csproj src/SagaDemo.StockService/

RUN dotnet restore src/SagaDemo.StockService/SagaDemo.StockService.csproj

COPY src/SagaDemo.Contracts/ src/SagaDemo.Contracts/
COPY src/SagaDemo.Application/ src/SagaDemo.Application/
COPY src/SagaDemo.Infrastructure/ src/SagaDemo.Infrastructure/
COPY src/SagaDemo.StockService/ src/SagaDemo.StockService/

RUN dotnet publish src/SagaDemo.StockService/SagaDemo.StockService.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/runtime:10.0
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "SagaDemo.StockService.dll"]
