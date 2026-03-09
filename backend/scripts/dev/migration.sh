#!/bin/bash
set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT="$SCRIPT_DIR/../../Stockama.Data/Stockama.Data.csproj"

if [ -z "$1" ]; then
  echo "❌ Migration name gerekli!"
  echo "Kullanım: ./backend/scripts/dev/migration.sh MigrationName"
  exit 1
fi

NAME=$1

echo "🔧 Adding migration '$NAME'..."
dotnet ef migrations add "$NAME" --project "$PROJECT"

echo "🚀 Updating database..."
dotnet ef database update --project "$PROJECT"

echo "✅ Migration eklendi ve veritabanı güncellendi!"
