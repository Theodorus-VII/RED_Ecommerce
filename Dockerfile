FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine3.19 AS base
EXPOSE 80
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine3.19 AS build
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

WORKDIR /app/Public/Images
COPY "./Ecommerce/Public/Images" .

WORKDIR /app/.well-known
COPY "./Ecommerce/.well-known" .

FROM runtime as final
WORKDIR /app

ENTRYPOINT [ "dotnet", "Ecommerce.dll" ]