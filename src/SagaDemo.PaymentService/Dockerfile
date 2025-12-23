FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY src/SagaDemo.Contracts/*.csproj src/SagaDemo.Contracts/
COPY src/SagaDemo.Application/*.csproj src/SagaDemo.Application/
COPY src/SagaDemo.Infrastructure/*.csproj src/SagaDemo.Infrastructure/
COPY src/SagaDemo.PaymentService/*.csproj src/SagaDemo.PaymentService/

RUN dotnet restore src/SagaDemo.PaymentService/SagaDemo.PaymentService.csproj

COPY src/SagaDemo.Contracts/ src/SagaDemo.Contracts/
COPY src/SagaDemo.Application/ src/SagaDemo.Application/
COPY src/SagaDemo.Infrastructure/ src/SagaDemo.Infrastructure/
COPY src/SagaDemo.PaymentService/ src/SagaDemo.PaymentService/

RUN dotnet publish src/SagaDemo.PaymentService/SagaDemo.PaymentService.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/runtime:10.0
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "SagaDemo.PaymentService.dll"]
