import { mkdir, readdir, readFile, rm, writeFile } from "node:fs/promises";
import { readFileSync } from "node:fs";
import path from "node:path";
import { fileURLToPath } from "node:url";
import { build } from "esbuild";

const version = JSON.parse(readFileSync(new URL("./node_modules/animejs/package.json", import.meta.url), "utf8")).version;
const outdir = fileURLToPath(new URL("./dist/bundle/", import.meta.url));
const coreWww = fileURLToPath(new URL("../wwwroot/", import.meta.url));
const coreName = "BlazorAnime.lib.module.js";
const satellites = [
    {
        name: "BlazorAnime.Animatable.lib.module.js",
        entry: "BlazorAnime.Animatable.lib.module",
        source: fileURLToPath(new URL("./animatable-entry.js", import.meta.url)),
        www: fileURLToPath(new URL("../../BlazorAnime.Animatable/wwwroot/", import.meta.url)),
        forbidden: ["/animatable/animatable.js", "/animatable-entry.js"]
    },
    {
        name: "BlazorAnime.Draggable.lib.module.js",
        entry: "BlazorAnime.Draggable.lib.module",
        source: fileURLToPath(new URL("./draggable-entry.js", import.meta.url)),
        www: fileURLToPath(new URL("../../BlazorAnime.Draggable/wwwroot/", import.meta.url)),
        forbidden: ["/draggable/draggable.js", "/draggable-entry.js"]
    }
];

await rm(outdir, { recursive: true, force: true });
await mkdir(outdir, { recursive: true });

const entryPoints = {
    "BlazorAnime.lib.module": fileURLToPath(new URL("./entry.js", import.meta.url))
};
for (const satellite of satellites) entryPoints[satellite.entry] = satellite.source;

const result = await build({
    entryPoints,
    bundle: true,
    splitting: true,
    format: "esm",
    platform: "browser",
    target: "es2018",
    minify: true,
    legalComments: "inline",
    outdir,
    entryNames: "[name]",
    chunkNames: "chunks/[hash]",
    metafile: true,
    define: {
        __ANIMEJS_VERSION__: JSON.stringify(version)
    }
});

assertOneEngine(result.metafile);

await rm(path.join(coreWww, "chunks"), { recursive: true, force: true });
await mkdir(path.join(coreWww, "chunks"), { recursive: true });

for (const file of await readdir(path.join(outdir, "chunks"))) {
    await writeFile(
        path.join(coreWww, "chunks", file),
        await readFile(path.join(outdir, "chunks", file)));
}

await writeFile(path.join(coreWww, coreName), await readFile(path.join(outdir, coreName)));
console.log(`${path.join(coreWww, coreName)} ${version}`);

for (const satellite of satellites) {
    await mkdir(satellite.www, { recursive: true });
    const source = retarget(await readFile(path.join(outdir, satellite.name), "utf8"));
    if (!source.includes("../BlazorAnime/")) {
        throw new Error(`${satellite.name} does not import the core package.`);
    }
    await writeFile(path.join(satellite.www, satellite.name), source);
    console.log(`${path.join(satellite.www, satellite.name)} ${version}`);
}

await rm(outdir, { recursive: true, force: true });

// Satellite files move into other packages. Point their imports at the core wwwroot.
function retarget(source) {
    return source.replace(
        /(?:from|import)\s*(["'])\.\/(?:chunks\/|BlazorAnime\.lib\.module\.js)/g,
        (match) => match.replace("./", "../BlazorAnime/"));
}

function assertOneEngine(metafile) {
    const outputs = metafile.outputs;
    const engineOutputs = [];
    for (const [file, meta] of Object.entries(outputs)) {
        for (const input of Object.keys(meta.inputs)) {
            if (input.replaceAll("\\", "/").endsWith("/engine/engine.js")) engineOutputs.push(file);
        }
    }
    if (engineOutputs.length !== 1) {
        throw new Error(`engine.js was emitted ${engineOutputs.length} times (${engineOutputs.join(", ")}).`);
    }

    const forbidden = satellites.flatMap((satellite) => satellite.forbidden);
    const core = outputNamed(outputs, coreName);
    assertNotReached(core, outputs, forbidden, "Core bundle");

    for (const satellite of satellites) {
        const file = outputNamed(outputs, satellite.name);
        const reached = reachable(file, outputs);
        const sharesEngine = [...reached].some((output) =>
            Object.keys(outputs[output].inputs).some((input) => input.replaceAll("\\", "/").endsWith("/engine/engine.js")));
        if (!sharesEngine) {
            throw new Error(`${satellite.name} does not share the core engine.`);
        }
    }

    const animatable = outputNamed(outputs, "BlazorAnime.Animatable.lib.module.js");
    assertNotReached(animatable, outputs, ["/draggable/draggable.js", "/draggable-entry.js"], "Animatable bundle");
}

function assertNotReached(start, outputs, suffixes, label) {
    for (const file of reachable(start, outputs)) {
        for (const input of Object.keys(outputs[file].inputs)) {
            const normalized = input.replaceAll("\\", "/");
            if (suffixes.some((suffix) => normalized.endsWith(suffix))) {
                throw new Error(`${label} includes ${normalized} via ${file}`);
            }
        }
    }
}

function outputNamed(outputs, name) {
    const file = Object.keys(outputs).find((key) => key.endsWith(name));
    if (!file) throw new Error(`Missing esbuild output ${name}.`);
    return file;
}

function reachable(start, outputs) {
    const seen = new Set();
    const stack = [start];
    while (stack.length) {
        const file = stack.pop();
        if (!file || seen.has(file) || !outputs[file]) continue;
        seen.add(file);
        for (const item of outputs[file].imports ?? []) {
            const next = resolveOutput(file, item.path, outputs);
            if (next) stack.push(next);
        }
    }
    return seen;
}

function resolveOutput(fromFile, spec, outputs) {
    if (outputs[spec]) return spec;
    const resolved = path.posix.normalize(path.posix.join(path.posix.dirname(fromFile), spec));
    if (outputs[resolved]) return resolved;
    const suffix = Object.keys(outputs).find((key) => key.endsWith(spec) || key.endsWith(resolved));
    return suffix ?? null;
}
