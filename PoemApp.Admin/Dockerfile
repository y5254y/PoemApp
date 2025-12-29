# PoemApp.Admin Dockerfile
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# ensure nuget folders are Linux-friendly inside container
ENV NUGET_PACKAGES=/root/.nuget/packages
ENV NUGET_FALLBACK_PACKAGES=/usr/share/dotnet/sdk/NuGetFallbackFolder

# create fallback folder to avoid NU1301 when SDK expects it
RUN mkdir -p /usr/share/dotnet/sdk/NuGetFallbackFolder

# copy NuGet.config to ensure container-friendly config
COPY NuGet.config ./

# copy project files and restore only Admin project to avoid MAUI workloads
COPY PoemApp.Admin/PoemApp.Admin.csproj PoemApp.Admin/
COPY PoemApp.Core/PoemApp.Core.csproj PoemApp.Core/
COPY PoemApp.Infrastructure/PoemApp.Infrastructure.csproj PoemApp.Infrastructure/
COPY PoemApp.Client.ApiClients/PoemApp.Client.ApiClients.csproj PoemApp.Client.ApiClients/

RUN dotnet restore PoemApp.Admin/PoemApp.Admin.csproj --configfile /src/NuGet.config -p:RestorePackagesPath=/root/.nuget/packages -p:RestoreFallbackFolders=/usr/share/dotnet/sdk/NuGetFallbackFolder

# copy the rest
COPY . ./
# remove host bin/obj to avoid Windows paths in assets
RUN find . -type d -name bin -prune -exec rm -rf {} + || true
RUN find . -type d -name obj -prune -exec rm -rf {} + || true

# publish inside container
RUN dotnet publish PoemApp.Admin/PoemApp.Admin.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish ./
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80
ENTRYPOINT ["dotnet", "PoemApp.Admin.dll"]
