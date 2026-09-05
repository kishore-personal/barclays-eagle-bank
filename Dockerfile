FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY EagleBank.sln .
COPY src/EagleBank.Domain/EagleBank.Domain.csproj src/EagleBank.Domain/
COPY src/EagleBank.Application/EagleBank.Application.csproj src/EagleBank.Application/
COPY src/EagleBank.Infrastructure/EagleBank.Infrastructure.csproj src/EagleBank.Infrastructure/
COPY src/EagleBank.Api/EagleBank.Api.csproj src/EagleBank.Api/
RUN dotnet restore src/EagleBank.Api/EagleBank.Api.csproj
COPY src/ src/
COPY openapi.yaml .
RUN dotnet publish src/EagleBank.Api/EagleBank.Api.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app .
COPY --from=build /src/openapi.yaml ./openapi.yaml
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Development
EXPOSE 8080
ENTRYPOINT ["dotnet", "EagleBank.Api.dll"]
