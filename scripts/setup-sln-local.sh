#!/usr/bin/env bash
set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"

if ! command -v dotnet >/dev/null 2>&1; then
  echo "dotnet not found. Installing local SDK..."
  bash "${ROOT_DIR}/scripts/install-dotnet-sdk.sh"
  export DOTNET_ROOT="${DOTNET_ROOT:-$HOME/.dotnet}"
  export PATH="${DOTNET_ROOT}:${DOTNET_ROOT}/tools:${PATH}"
fi

echo "Using dotnet from: $(command -v dotnet)"
dotnet --info >/dev/null

echo "Restoring solution packages..."
dotnet restore "${ROOT_DIR}/MiniShopee.sln"

echo "Building solution..."
dotnet build "${ROOT_DIR}/MiniShopee.sln" -c Debug --no-restore

echo "Preparing EF Core local tool (optional for migrations)..."
dotnet tool install --global dotnet-ef --version 8.* >/dev/null 2>&1 || true
dotnet tool update --global dotnet-ef --version 8.* >/dev/null 2>&1 || true

echo "Done. To run app:"
echo "  cd ${ROOT_DIR}/src/MiniShopee && dotnet run"
