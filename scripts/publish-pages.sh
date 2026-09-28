#!/usr/bin/env bash
# Publish the WebAssembly samples into a static site for GitHub Pages.
# Usage: scripts/publish-pages.sh [output-dir] [base-path]
# Example: scripts/publish-pages.sh site /blazor-anime
set -euo pipefail

root="$(cd "$(dirname "$0")/.." && pwd)"
out="${1:-"$root/site"}"
base="${2:-/blazor-anime}"
base="${base%/}"

if [[ "$base" != /* ]]; then
  echo "Base path must start with / (got: $base)" >&2
  exit 1
fi

tmp="$(mktemp -d)"
trap 'rm -rf "$tmp"' EXIT

# CompressionEnabled is the static-web-assets switch. GitHub Pages serves .br/.gz
# as ordinary files, without a Content-Encoding header, so leave them out.
dotnet publish "$root/samples/Examples.WebAssembly/Examples.WebAssembly.csproj" \
  --configuration Release \
  -p:CompressionEnabled=false \
  --output "$tmp/examples"
dotnet publish "$root/samples/PageTransitions/PageTransitions.csproj" \
  --configuration Release \
  -p:CompressionEnabled=false \
  --output "$tmp/travel"

rm -rf "$out"
mkdir -p "$out/examples" "$out/travel"
cp -a "$tmp/examples/wwwroot/." "$out/examples/"
cp -a "$tmp/travel/wwwroot/." "$out/travel/"
find "$out" -type f \( -name '*.br' -o -name '*.gz' \) -delete
: > "$out/.nojekyll"

python3 - "$out" "$base" "$root/pages" <<'PY'
import pathlib, sys
out = pathlib.Path(sys.argv[1])
base = sys.argv[2].rstrip("/")
pages = pathlib.Path(sys.argv[3])

for app in ("examples", "travel"):
    index = out / app / "index.html"
    text = index.read_text()
    marker = '<base href="/" />'
    href = f'<base href="{base}/{app}/" />'
    if marker not in text:
        raise SystemExit(f"{index} is missing {marker}")
    index.write_text(text.replace(marker, href, 1))

def publish_static(name: str) -> None:
    source = (pages / name).read_text().replace("__BASE__", base)
    (out / name).write_text(source)

publish_static("index.html")
publish_static("404.html")
PY

echo "Published $out with base $base"
