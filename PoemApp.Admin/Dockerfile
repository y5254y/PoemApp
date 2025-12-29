# PoemApp.Admin Dockerfile
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# ensure nuget folders are Linux-friendly inside container
ENV NUGET_PACKAGES=/root/.nuget/packages
ENV NUGET_FALLBACK_PACKAGES=/usr/share/dotnet/sdk/NuGetFallbackFolder

RUN mkdir -p /usr/share/dotnet/sdk/NuGetFallbackFolder

# copy project files and restore only Admin project to avoid MAUI workloads
COPY PoemApp.Admin/PoemApp.Admin.csproj PoemApp.Admin/
COPY PoemApp.Core/PoemApp.Core.csproj PoemApp.Core/
COPY PoemApp.Infrastructure/PoemApp.Infrastructure.csproj PoemApp.Infrastructure/
COPY PoemApp.Client.ApiClients/PoemApp.Client.ApiClients.csproj PoemApp.Client.ApiClients/

RUN dotnet restore PoemApp.Admin/PoemApp.Admin.csproj

# copy the rest and publish
COPY . ./
RUN dotnet publish PoemApp.Admin/PoemApp.Admin.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish ./
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80
ENTRYPOINT ["dotnet", "PoemApp.Admin.dll"]
