FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY src/SagaDemo.Contracts/*.csproj src/SagaDemo.Contracts/
COPY src/SagaDemo.Application/*.csproj src/SagaDemo.Application/
COPY src/SagaDemo.Infrastructure/*.csproj src/SagaDemo.Infrastructure/
COPY src/SagaDemo.Api/*.csproj src/SagaDemo.Api/

RUN dotnet restore src/SagaDemo.Api/SagaDemo.Api.csproj

COPY src/SagaDemo.Contracts/ src/SagaDemo.Contracts/
COPY src/SagaDemo.Application/ src/SagaDemo.Application/
COPY src/SagaDemo.Infrastructure/ src/SagaDemo.Infrastructure/
COPY src/SagaDemo.Api/ src/SagaDemo.Api/

RUN dotnet publish src/SagaDemo.Api/SagaDemo.Api.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app .
EXPOSE 8080
ENTRYPOINT ["dotnet", "SagaDemo.Api.dll"]
