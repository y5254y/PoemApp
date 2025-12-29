#!/bin/sh
# entrypoint: wait for DB, run migrations, then start
set -e

host="db"
port=3306

until nc -z $host $port; do
  echo "Waiting for MySQL at $host:$port..."
  sleep 2
done

echo "Running EF Core migrations..."
# If dotnet-ef not installed, can run with 'dotnet ef database update' if tooling available
# attempt to run migrations
dotnet tool restore || true
if dotnet ef database update --help >/dev/null 2>&1; then
  dotnet ef database update
else
  echo "dotnet ef not available in runtime image. Skipping automatic migrations."
fi

exec dotnet PoemApp.API.dll
