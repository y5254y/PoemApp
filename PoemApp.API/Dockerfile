# PoemApp.API Dockerfile
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# copy sln and restore
COPY . ./
RUN dotnet restore PoemApp.sln

# publish
RUN dotnet publish PoemApp.API/PoemApp.API.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish ./
ENV ASPNETCORE_URLS=http://+:5000
EXPOSE 5000
ENTRYPOINT ["dotnet", "PoemApp.API.dll"]
