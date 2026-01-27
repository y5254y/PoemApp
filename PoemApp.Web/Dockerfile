# PoemApp.web Dockerfile
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY PoemApp.Web/PoemApp.Web.csproj PoemApp.Web/
COPY PoemApp.Core/PoemApp.Core.csproj PoemApp.Core/
COPY PoemApp.Client.ApiClients/PoemApp.Client.ApiClients.csproj PoemApp.Client.ApiClients/

RUN dotnet restore PoemApp.Web/PoemApp.Web.csproj

COPY . ./

RUN dotnet publish PoemApp.Web/PoemApp.Web.csproj -c Release -o /app/publish

FROM nginx:alpine AS final
WORKDIR /usr/share/nginx/html
COPY --from=build /app/publish/wwwroot .
COPY PoemApp.Web/nginx.conf /etc/nginx/conf.d/default.conf


