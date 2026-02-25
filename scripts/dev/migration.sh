#!/bin/bash
set -e

if [ -z "$1" ]; then
  echo "❌ Migration name gerekli!"
  echo "Kullanım: ./scripts/dev/migration.sh MigrationName"
  exit 1
fi

NAME=$1
PROJECT="../../Stockama.Data/"

echo "🔧 Adding migration '$NAME'..."
dotnet ef migrations add "$NAME" --project "$PROJECT"

echo "🚀 Updating database..."
dotnet ef database update --project "$PROJECT"

echo "✅ Migration eklendi ve veritabanı güncellendi!"
