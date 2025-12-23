FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["SagaDemo.sln", "./"]
COPY ["src/SagaDemo.Api/SagaDemo.Api.csproj", "src/SagaDemo.Api/"]
COPY ["src/SagaDemo.Application/SagaDemo.Application.csproj", "src/SagaDemo.Application/"]
COPY ["src/SagaDemo.Contracts/SagaDemo.Contracts.csproj", "src/SagaDemo.Contracts/"]
COPY ["src/SagaDemo.Infrastructure/SagaDemo.Infrastructure.csproj", "src/SagaDemo.Infrastructure/"]
COPY ["src/SagaDemo.Workers/SagaDemo.Workers.csproj", "src/SagaDemo.Workers/"]
COPY ["src/SagaDemo.StockService/SagaDemo.StockService.csproj", "src/SagaDemo.StockService/"]
COPY ["src/SagaDemo.PaymentService/SagaDemo.PaymentService.csproj", "src/SagaDemo.PaymentService/"]
COPY ["src/SagaDemo.ShippingService/SagaDemo.ShippingService.csproj", "src/SagaDemo.ShippingService/"]

RUN dotnet restore

COPY . .

RUN dotnet publish "src/SagaDemo.Api/SagaDemo.Api.csproj" -c Release -o /app/api --no-restore
RUN dotnet publish "src/SagaDemo.Workers/SagaDemo.Workers.csproj" -c Release -o /app/workers --no-restore
RUN dotnet publish "src/SagaDemo.StockService/SagaDemo.StockService.csproj" -c Release -o /app/stock --no-restore
RUN dotnet publish "src/SagaDemo.PaymentService/SagaDemo.PaymentService.csproj" -c Release -o /app/payment --no-restore
RUN dotnet publish "src/SagaDemo.ShippingService/SagaDemo.ShippingService.csproj" -c Release -o /app/shipping --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS api
WORKDIR /app
COPY --from=build /app/api .
EXPOSE 8080
ENTRYPOINT ["dotnet", "SagaDemo.Api.dll"]

FROM mcr.microsoft.com/dotnet/runtime:10.0 AS workers
WORKDIR /app
COPY --from=build /app/workers .
ENTRYPOINT ["dotnet", "SagaDemo.Workers.dll"]

FROM mcr.microsoft.com/dotnet/runtime:10.0 AS stock
WORKDIR /app
COPY --from=build /app/stock .
ENTRYPOINT ["dotnet", "SagaDemo.StockService.dll"]

FROM mcr.microsoft.com/dotnet/runtime:10.0 AS payment
WORKDIR /app
COPY --from=build /app/payment .
ENTRYPOINT ["dotnet", "SagaDemo.PaymentService.dll"]

FROM mcr.microsoft.com/dotnet/runtime:10.0 AS shipping
WORKDIR /app
COPY --from=build /app/shipping .
ENTRYPOINT ["dotnet", "SagaDemo.ShippingService.dll"]
