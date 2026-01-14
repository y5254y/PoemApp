# PoemApp.Web Dockerfile - Blazor WebAssembly static hosting
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# ensure nuget folders are Linux-friendly inside container
ENV NUGET_PACKAGES=/root/.nuget/packages
ENV NUGET_FALLBACK_PACKAGES=/usr/share/dotnet/sdk/NuGetFallbackFolder

# create fallback folder to avoid NU1301 when SDK expects it
RUN mkdir -p /usr/share/dotnet/sdk/NuGetFallbackFolder

# copy NuGet.config so restore uses container-friendly config
COPY NuGet.config ./

# copy project files and restore only Web project to avoid MAUI workloads
COPY PoemApp.Web/PoemApp.Web.csproj PoemApp.Web/
COPY PoemApp.Core/PoemApp.Core.csproj PoemApp.Core/
COPY PoemApp.Client.ApiClients/PoemApp.Client.ApiClients.csproj PoemApp.Client.ApiClients/

RUN dotnet restore PoemApp.Web/PoemApp.Web.csproj --configfile /src/NuGet.config -p:RestorePackagesPath=/root/.nuget/packages -p:RestoreFallbackFolders=/usr/share/dotnet/sdk/NuGetFallbackFolder

# copy the rest and publish (use MSBuild props to enforce package folders)
COPY . ./

# remove any host bin/obj (they may contain Windows absolute paths in assets) to avoid using host project.assets.json
RUN find . -type d -name bin -prune -exec rm -rf {} + || true
RUN find . -type d -name obj -prune -exec rm -rf {} + || true

# restore again after cleaning host obj so assets are generated in container
RUN dotnet restore PoemApp.Web/PoemApp.Web.csproj --configfile /src/NuGet.config -p:RestorePackagesPath=/root/.nuget/packages -p:RestoreFallbackFolders=/usr/share/dotnet/sdk/NuGetFallbackFolder

# Publish directly to /wwwroot (not /app/publish) to ensure _framework and _content are in the root
RUN dotnet publish PoemApp.Web/PoemApp.Web.csproj -c Release -o /wwwroot -p:RestorePackagesPath=/root/.nuget/packages -p:RestoreFallbackFolders=/usr/share/dotnet/sdk/NuGetFallbackFolder -p:PublishTrimmed=false

# Runtime: use nginx to serve static files (Blazor WASM is client-side, not a server app)
FROM nginx:stable

# Copy all published files (including _framework and _content) directly to nginx
COPY --from=build /wwwroot /usr/share/nginx/html

# Copy nginx config (for SPA routing fallback to index.html)
COPY nginx/web.conf /etc/nginx/conf.d/default.conf

# Verify structure
RUN echo "=== wwwroot listing ===" && \
    ls -la /usr/share/nginx/html/ && \
    echo "=== _framework sample ===" && \
    ls /usr/share/nginx/html/_framework | head -10

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]
