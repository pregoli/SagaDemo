FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY src/SagaDemo.Contracts/*.csproj src/SagaDemo.Contracts/
COPY src/SagaDemo.Application/*.csproj src/SagaDemo.Application/
COPY src/SagaDemo.Infrastructure/*.csproj src/SagaDemo.Infrastructure/
COPY src/SagaDemo.ShippingService/*.csproj src/SagaDemo.ShippingService/

RUN dotnet restore src/SagaDemo.ShippingService/SagaDemo.ShippingService.csproj

COPY src/SagaDemo.Contracts/ src/SagaDemo.Contracts/
COPY src/SagaDemo.Application/ src/SagaDemo.Application/
COPY src/SagaDemo.Infrastructure/ src/SagaDemo.Infrastructure/
COPY src/SagaDemo.ShippingService/ src/SagaDemo.ShippingService/

RUN dotnet publish src/SagaDemo.ShippingService/SagaDemo.ShippingService.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/runtime:10.0
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "SagaDemo.ShippingService.dll"]
