"""Rewrite published sample index files and copy the Pages landing pages."""
import pathlib
import sys

out = pathlib.Path(sys.argv[1])
base = sys.argv[2].rstrip("/")
pages = pathlib.Path(sys.argv[3])

for app in ("examples", "travel"):
    index = out / app / "index.html"
    text = index.read_text(encoding="utf-8")
    marker = '<base href="/" />'
    href = f'<base href="{base}/{app}/" />'
    if marker not in text:
        raise SystemExit(f"{index} is missing {marker}")
    index.write_text(text.replace(marker, href, 1), encoding="utf-8", newline="\n")


def publish_static(name: str) -> None:
    source = (pages / name).read_text(encoding="utf-8").replace("__BASE__", base)
    (out / name).write_text(source, encoding="utf-8", newline="\n")


publish_static("index.html")
publish_static("404.html")
