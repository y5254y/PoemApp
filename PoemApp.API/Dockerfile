# PoemApp.API Dockerfile
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# copy project files and restore only API project to avoid MAUI workloads
COPY PoemApp.API/PoemApp.API.csproj PoemApp.API/
COPY PoemApp.Core/PoemApp.Core.csproj PoemApp.Core/
COPY PoemApp.Infrastructure/PoemApp.Infrastructure.csproj PoemApp.Infrastructure/

RUN dotnet restore PoemApp.API/PoemApp.API.csproj

# copy the rest and publish
COPY . ./
RUN dotnet publish PoemApp.API/PoemApp.API.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish ./
ENV ASPNETCORE_URLS=http://+:5000
EXPOSE 5000
ENTRYPOINT ["dotnet", "PoemApp.API.dll"]
