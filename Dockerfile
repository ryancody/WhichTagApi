FROM amazonlinux:latest
WORKDIR /
RUN yum update -y && yum install -y awslogs
RUN service awslogs start

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
ARG TWITTER_KEY=${TWITTER_KEY}
ARG TWITTER_SECRET=${TWITTER_SECRET}
ARG MONGO_USERNAME=${MONGO_USERNAME}
ARG MONGO_PASSWORD=${MONGO_PASSWORD}
ARG MONGO_DATABSENAME=${MONGO_DATABSENAME}