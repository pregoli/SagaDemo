FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY src/SagaDemo.Contracts/*.csproj src/SagaDemo.Contracts/
COPY src/SagaDemo.Application/*.csproj src/SagaDemo.Application/
COPY src/SagaDemo.Infrastructure/*.csproj src/SagaDemo.Infrastructure/
COPY src/SagaDemo.Workers/*.csproj src/SagaDemo.Workers/

RUN dotnet restore src/SagaDemo.Workers/SagaDemo.Workers.csproj

COPY src/SagaDemo.Contracts/ src/SagaDemo.Contracts/
COPY src/SagaDemo.Application/ src/SagaDemo.Application/
COPY src/SagaDemo.Infrastructure/ src/SagaDemo.Infrastructure/
COPY src/SagaDemo.Workers/ src/SagaDemo.Workers/

RUN dotnet publish src/SagaDemo.Workers/SagaDemo.Workers.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/runtime:10.0
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "SagaDemo.Workers.dll"]
