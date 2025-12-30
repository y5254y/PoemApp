# PoemApp.API Dockerfile
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# ensure nuget folders are Linux-friendly inside container
ENV NUGET_PACKAGES=/root/.nuget/packages
ENV NUGET_FALLBACK_PACKAGES=/usr/share/dotnet/sdk/NuGetFallbackFolder

# create fallback folder to avoid NU1301 when SDK expects it
RUN mkdir -p /usr/share/dotnet/sdk/NuGetFallbackFolder

# copy NuGet.config to ensure container-friendly config
COPY NuGet.config ./

# copy project files and restore only API project to avoid MAUI workloads
COPY PoemApp.API/PoemApp.API.csproj PoemApp.API/
COPY PoemApp.Core/PoemApp.Core.csproj PoemApp.Core/
COPY PoemApp.Infrastructure/PoemApp.Infrastructure.csproj PoemApp.Infrastructure/
COPY PoemApp.Web/PoemApp.Web.csproj PoemApp.Web/
COPY PoemApp.Client.ApiClients/PoemApp.Client.ApiClients.csproj PoemApp.Client.ApiClients/

RUN dotnet restore PoemApp.API/PoemApp.API.csproj --configfile /src/NuGet.config -p:RestorePackagesPath=/root/.nuget/packages -p:RestoreFallbackFolders=/usr/share/dotnet/sdk/NuGetFallbackFolder

# copy the rest
COPY . ./
# remove host bin/obj to avoid Windows paths in assets
RUN find . -type d -name bin -prune -exec rm -rf {} + || true
RUN find . -type d -name obj -prune -exec rm -rf {} + || true

# Build/publish the Blazor WebAssembly client inside the same build stage and copy into API wwwroot
RUN dotnet restore PoemApp.Web/PoemApp.Web.csproj --configfile /src/NuGet.config -p:RestorePackagesPath=/root/.nuget/packages -p:RestoreFallbackFolders=/usr/share/dotnet/sdk/NuGetFallbackFolder
RUN dotnet publish PoemApp.Web/PoemApp.Web.csproj -c Release -o /tmp/poemapp.web.publish
RUN mkdir -p PoemApp.API/wwwroot && mkdir -p PoemApp.API/wwwroot/_framework
RUN cp -r /tmp/poemapp.web.publish/_framework/* PoemApp.API/wwwroot/_framework/ || true
RUN cp -r PoemApp.Web/wwwroot/* PoemApp.API/wwwroot/ || true

# restore again inside container (safe) and publish API without triggering PublishClient target
RUN dotnet restore PoemApp.API/PoemApp.API.csproj --configfile /src/NuGet.config -p:RestorePackagesPath=/root/.nuget/packages -p:RestoreFallbackFolders=/usr/share/dotnet/sdk/NuGetFallbackFolder
RUN dotnet publish PoemApp.API/PoemApp.API.csproj -c Release -o /app/publish -p:PublishClient=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish ./
ENV ASPNETCORE_URLS=http://+:5000
EXPOSE 5000
ENTRYPOINT ["dotnet", "PoemApp.API.dll"]
