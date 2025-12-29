# PoemApp.Web Dockerfile
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# copy project files and restore only Web project to avoid MAUI workloads
COPY PoemApp.Web/PoemApp.Web.csproj PoemApp.Web/
COPY PoemApp.Core/PoemApp.Core.csproj PoemApp.Core/
COPY PoemApp.Client.ApiClients/PoemApp.Client.ApiClients.csproj PoemApp.Client.ApiClients/

RUN dotnet restore PoemApp.Web/PoemApp.Web.csproj

# copy the rest and publish
COPY . ./
RUN dotnet publish PoemApp.Web/PoemApp.Web.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish ./
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80
ENTRYPOINT ["dotnet", "PoemApp.Web.dll"]
