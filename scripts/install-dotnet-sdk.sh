#!/usr/bin/env bash
set -euo pipefail

DOTNET_CHANNEL="${DOTNET_CHANNEL:-8.0}"
DOTNET_VERSION="${DOTNET_VERSION:-8.0.100}"
INSTALL_DIR="${DOTNET_INSTALL_DIR:-$HOME/.dotnet}"

TMP_SCRIPT="/tmp/dotnet-install.sh"
URLS=(
  "https://dot.net/v1/dotnet-install.sh"
  "https://dotnet.microsoft.com/download/dotnet/scripts/v1/dotnet-install.sh"
)

echo "Installing .NET SDK ${DOTNET_VERSION} (channel ${DOTNET_CHANNEL}) into ${INSTALL_DIR}..."

ok=0
for url in "${URLS[@]}"; do
  if curl -fsSL "${url}" -o "${TMP_SCRIPT}"; then
    ok=1
    break
  fi
  echo "Cannot reach ${url}, trying next mirror..."
done

if [[ "${ok}" -ne 1 ]]; then
  cat <<'MSG'
ERROR: Không tải được dotnet-install.sh (thường do firewall/proxy/chặn mạng).
Bạn có thể:
1) Chạy qua Docker (không cần cài SDK host) theo README.
2) Hoặc cấu hình proxy rồi chạy lại script.
MSG
  exit 1
fi

bash "${TMP_SCRIPT}" --channel "${DOTNET_CHANNEL}" --version "${DOTNET_VERSION}" --install-dir "${INSTALL_DIR}"

cat <<'MSG'

Done. Add these lines to your shell profile if needed:
  export DOTNET_ROOT="$HOME/.dotnet"
  export PATH="$HOME/.dotnet:$HOME/.dotnet/tools:$PATH"

Then reopen terminal and run:
  dotnet --info
MSG
