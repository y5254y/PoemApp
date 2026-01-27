# PoemApp.API Dockerfile
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src


COPY PoemApp.Web/ ./PoemApp.Web/
RUN dotnet publish PoemApp.Web/PoemApp.Web.csproj -c Release -o /app/publish

FROM nginx:alpine AS final
WORKDIR /usr/share/nginx/html
COPY --from=build /app/publish/wwwroot .
COPY PoemApp.Web/nginx.conf /etc/nginx/conf.d/default.conf


