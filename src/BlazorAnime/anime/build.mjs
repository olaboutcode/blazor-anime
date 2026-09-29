import { readFileSync } from "node:fs";
import { build } from "esbuild";

const version = JSON.parse(readFileSync(new URL("./node_modules/animejs/package.json", import.meta.url), "utf8")).version;
const outfile = process.env.SPIKE_OUTFILE ?? new URL("./dist/spike.js", import.meta.url).pathname;

await build({
    entryPoints: [new URL("./entry.js", import.meta.url).pathname],
    bundle: true,
    format: "esm",
    platform: "browser",
    target: "es2018",
    minify: true,
    legalComments: "inline",
    outfile,
    define: {
        __ANIMEJS_VERSION__: JSON.stringify(version)
    }
});

console.log(`${outfile} ${version}`);
