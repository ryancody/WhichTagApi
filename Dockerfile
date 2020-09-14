FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
WORKDIR /app

# Copy everything else and build
COPY . ./
RUN dotnet restore
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "WhichTagApi.dll"]

ARG ASPNETCORE_ENVIRONMENT=${ENVIRONMENT_NAME}

COPY --from=build-env /nginx-shared/whichtag-api/appsettings.${ASPNETCORE_ENVIRONMENT}.json .