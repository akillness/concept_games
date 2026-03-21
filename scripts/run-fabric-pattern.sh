#!/bin/zsh
set -euo pipefail

if [ $# -lt 1 ]; then
  echo "usage: $0 <pattern> [fabric-args...]" >&2
  exit 2
fi

PATTERN="$1"
shift

if [ -x /opt/homebrew/opt/fabric-ai/bin/fabric-ai ]; then
  FABRIC_BIN="/opt/homebrew/opt/fabric-ai/bin/fabric-ai"
elif command -v fabric-ai >/dev/null 2>&1; then
  FABRIC_BIN="$(command -v fabric-ai)"
elif command -v fabric >/dev/null 2>&1; then
  FABRIC_BIN="$(command -v fabric)"
else
  echo "fabric binary not found. Install with: brew install fabric-ai" >&2
  exit 1
fi

if ! grep -Eq '^(DEFAULT_VENDOR|DEFAULT_MODEL)=' "$HOME/.config/fabric/.env" 2>/dev/null; then
  echo "fabric is installed but not configured. Run: $FABRIC_BIN --setup" >&2
  exit 1
fi

exec "$FABRIC_BIN" -p "$PATTERN" "$@"
