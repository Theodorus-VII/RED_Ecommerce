FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine3.16 AS base
EXPOSE 80
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine3.16 AS build
RUN dotnet tool install --version 6.0.9 --global dotnet-ef
ENV PATH="$PATH:/root/.dotnet/tools"

WORKDIR /src
COPY "./Ecommerce/Ecommerce.csproj" .

RUN dotnet restore 
COPY . .

FROM build as publish
RUN dotnet publish ./Ecommerce/Ecommerce.csproj -c Release -o /app

FROM base AS runtime
WORKDIR /app
COPY --from=publish /app .

# Run EF Migrations

FROM runtime as migrations

ENTRYPOINT [ "dotnet", "Ecommerce.dll" ]