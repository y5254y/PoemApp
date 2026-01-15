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

# Publish to /app/publish (standard location), wwwroot will be at /app/publish/wwwroot
RUN dotnet publish PoemApp.Web/PoemApp.Web.csproj -c Release -o /app/publish -p:RestorePackagesPath=/root/.nuget/packages -p:RestoreFallbackFolders=/usr/share/dotnet/sdk/NuGetFallbackFolder -p:PublishTrimmed=false

# Debug: verify wwwroot structure in build stage
RUN echo "=== /app/publish structure ===" && \
    find /app/publish -type f | head -30 && \
    echo "=== /app/publish/wwwroot ===" && \
    ls -la /app/publish/wwwroot/ 2>&1 || echo "wwwroot not found"

# Runtime: use nginx to serve static files
FROM nginx:stable

# Remove nginx default files that might interfere
RUN rm -f /usr/share/nginx/html/index.html /usr/share/nginx/html/50x.html

# Copy published wwwroot to temp location
COPY --from=build /app/publish/wwwroot /tmp/wwwroot

# Move all files from _content/PoemApp.Web to nginx html root (because StaticWebAssetBasePath puts them in a subdir)
RUN cp -r /tmp/wwwroot/_content/PoemApp.Web/* /usr/share/nginx/html/ && \
    # Also keep _content structure for client libraries
    mkdir -p /usr/share/nginx/html/_content && \
    cp -r /tmp/wwwroot/_content /usr/share/nginx/html/ 2>/dev/null || true

# Copy nginx config
COPY nginx/web.conf /etc/nginx/conf.d/default.conf

# Verify files
RUN echo "=== Checking /usr/share/nginx/html ===" && \
    ls -la /usr/share/nginx/html/ && \
    echo "=== Checking index.html ===" && \
    head -c 100 /usr/share/nginx/html/index.html

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]
