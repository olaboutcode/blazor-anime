#!/usr/bin/env python3
"""Check the eight BlazorAnime nupkgs before a nuget.org push."""

import sys
import zipfile
from pathlib import Path
from xml.etree import ElementTree as ET

EXPECTED = [
    "BlazorAnime",
    "BlazorAnime.Animatable",
    "BlazorAnime.Draggable",
    "BlazorAnime.Layout",
    "BlazorAnime.Scope",
    "BlazorAnime.Events",
    "BlazorAnime.Waapi",
    "BlazorAnime.Text",
]
SATELLITES = EXPECTED[1:]
FORBIDDEN = ("package.json", "package-lock.json", "node_modules")
REPO = "https://github.com/olaboutcode/blazor-anime"


def local(tag):
    return tag.rsplit("}", 1)[-1]


def child(parent, name):
    for item in parent:
        if local(item.tag) == name:
            return item
    return None


def children(parent, name):
    return [item for item in parent if local(item.tag) == name]


def main(folder):
    errors = []
    packages = sorted(Path(folder).glob("*.nupkg"))
    by_id = {}
    for path in packages:
        with zipfile.ZipFile(path) as archive:
            names = archive.namelist()
            nuspec_name = next(name for name in names if name.endswith(".nuspec"))
            nuspec = ET.fromstring(archive.read(nuspec_name))
        metadata = child(nuspec, "metadata")
        package_id = child(metadata, "id").text
        by_id[package_id] = (path, names, metadata)

    found = set(by_id)
    if found != set(EXPECTED):
        errors.append(f"expected {EXPECTED}, found {sorted(found)}")

    versions = {}
    for package_id in EXPECTED:
        if package_id not in by_id:
            continue
        path, names, metadata = by_id[package_id]
        version = child(metadata, "version").text
        versions[package_id] = version
        if version in ("0.0.1", "1.0.0"):
            errors.append(f"{package_id} version {version} must not be published")
        if "-" not in version:
            errors.append(f"{package_id} version {version} is not a prerelease")

        for name in names:
            lowered = name.replace("\\", "/").lower()
            if any(piece in lowered for piece in FORBIDDEN):
                errors.append(f"{package_id} ships {name}")

        license_node = child(metadata, "license")
        if license_node is None or license_node.attrib.get("type") != "expression" or license_node.text != "MIT":
            errors.append(f"{package_id} license must be MIT")

        repository = child(metadata, "repository")
        if repository is None or not (repository.attrib.get("url") or "").startswith(REPO):
            errors.append(f"{package_id} repository must point at {REPO}")

        if "README.md" not in names:
            errors.append(f"{package_id} is missing README.md")
        if not any(name.endswith(".lib.module.js") for name in names):
            errors.append(f"{package_id} is missing its lib.module.js")

        dependency_ids = []
        dependencies = child(metadata, "dependencies")
        if dependencies is not None:
            for group in children(dependencies, "group"):
                for dependency in children(group, "dependency"):
                    dependency_ids.append((dependency.attrib.get("id"), dependency.attrib.get("version")))

        if package_id == "BlazorAnime":
            if not any("/chunks/" in name.replace("\\", "/") for name in names):
                errors.append("BlazorAnime is missing wwwroot chunks")
            extra = [item for item in dependency_ids if item[0].startswith("BlazorAnime")]
            if extra:
                errors.append(f"BlazorAnime depends on {extra}")
        else:
            core = [item for item in dependency_ids if item[0] == "BlazorAnime"]
            others = [item for item in dependency_ids if item[0] in SATELLITES]
            if core != [("BlazorAnime", version)]:
                errors.append(f"{package_id} must depend on BlazorAnime {version}, found {core}")
            if others:
                errors.append(f"{package_id} depends on another satellite: {others}")

    distinct = set(versions.values())
    if len(distinct) != 1:
        errors.append(f"package versions differ: {versions}")

    if errors:
        for error in errors:
            print("FAIL:", error)
        return 1
    print("OK", next(iter(distinct)), "packages:", ", ".join(EXPECTED))
    return 0


if __name__ == "__main__":
    if len(sys.argv) != 2:
        print("usage: check-nupkgs.py <nupkg-directory>")
        sys.exit(2)
    sys.exit(main(sys.argv[1]))
