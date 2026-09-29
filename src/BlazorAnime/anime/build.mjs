import { readFileSync } from "node:fs";
import { fileURLToPath } from "node:url";
import { build } from "esbuild";

const version = JSON.parse(readFileSync(new URL("./node_modules/animejs/package.json", import.meta.url), "utf8")).version;
const outfile = process.env.OUTFILE
    ?? fileURLToPath(new URL("../wwwroot/BlazorAnime.lib.module.js", import.meta.url));

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
