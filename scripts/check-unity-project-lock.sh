#!/bin/zsh
set -euo pipefail

PROJECT_PATH="${1:-/Users/jangyoung/.superset/projects/concept_games/concept_game}"

echo "Project: $PROJECT_PATH"
echo

if [ -f "$PROJECT_PATH/Temp/UnityLockfile" ]; then
  echo "Lockfile: present"
else
  echo "Lockfile: missing"
fi

echo
echo "Unity processes:"
pgrep -af 'Unity.app/Contents/MacOS/Unity|UnityPackageManager|LicenseClient' || true
