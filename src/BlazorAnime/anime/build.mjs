import { mkdir, readdir, readFile, rm, writeFile } from "node:fs/promises";
import { readFileSync } from "node:fs";
import path from "node:path";
import { fileURLToPath } from "node:url";
import { build } from "esbuild";

const version = JSON.parse(readFileSync(new URL("./node_modules/animejs/package.json", import.meta.url), "utf8")).version;
const outdir = fileURLToPath(new URL("./dist/bundle/", import.meta.url));
const coreWww = fileURLToPath(new URL("../wwwroot/", import.meta.url));
const satelliteWww = fileURLToPath(new URL("../../BlazorAnime.Animatable/wwwroot/", import.meta.url));
const coreName = "BlazorAnime.lib.module.js";
const satelliteName = "BlazorAnime.Animatable.lib.module.js";

await rm(outdir, { recursive: true, force: true });
await mkdir(outdir, { recursive: true });

const result = await build({
    entryPoints: {
        "BlazorAnime.lib.module": fileURLToPath(new URL("./entry.js", import.meta.url)),
        "BlazorAnime.Animatable.lib.module": fileURLToPath(new URL("./animatable-entry.js", import.meta.url))
    },
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
await mkdir(satelliteWww, { recursive: true });

for (const file of await readdir(path.join(outdir, "chunks"))) {
    await writeFile(
        path.join(coreWww, "chunks", file),
        await readFile(path.join(outdir, "chunks", file)));
}

await writeFile(path.join(coreWww, coreName), await readFile(path.join(outdir, coreName)));

const satelliteSource = retarget(await readFile(path.join(outdir, satelliteName), "utf8"));
if (!satelliteSource.includes("../BlazorAnime/")) {
    throw new Error("Satellite module does not import the core package.");
}
await writeFile(path.join(satelliteWww, satelliteName), satelliteSource);
await rm(outdir, { recursive: true, force: true });

console.log(`${path.join(coreWww, coreName)} ${version}`);
console.log(`${path.join(satelliteWww, satelliteName)} ${version}`);

// The satellite file is moved into another package. Point its imports at the core wwwroot.
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

    const core = outputNamed(outputs, coreName);
    const reached = reachable(core, outputs);
    for (const file of reached) {
        for (const input of Object.keys(outputs[file].inputs)) {
            const normalized = input.replaceAll("\\", "/");
            if (normalized.endsWith("/animatable/animatable.js") || normalized.endsWith("/animatable-entry.js")) {
                throw new Error(`Core bundle includes createAnimatable via ${file} <- ${normalized}`);
            }
        }
    }

    const satellite = outputNamed(outputs, satelliteName);
    const satelliteReached = reachable(satellite, outputs);
    const sharesEngine = [...satelliteReached].some((file) =>
        Object.keys(outputs[file].inputs).some((input) => input.replaceAll("\\", "/").endsWith("/engine/engine.js")));
    if (!sharesEngine) {
        throw new Error("Satellite bundle does not share the core engine.");
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
