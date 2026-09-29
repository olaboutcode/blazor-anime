const assert = require("node:assert/strict");
const fs = require("node:fs");
const path = require("node:path");
const test = require("node:test");
const { pathToFileURL } = require("node:url");

const outfile = path.join(__dirname, "../../src/BlazorAnime/anime/dist/spike.js");

test("plain object seeks to 100", async () => {
    function NodeList() {}
    function HTMLCollection() {}
    function Node() {}
    function Element() {}
    function SVGElement() {}
    function HTMLInputElement() {}

    globalThis.NodeList = NodeList;
    globalThis.HTMLCollection = HTMLCollection;
    globalThis.Node = Node;
    globalThis.Element = Element;
    globalThis.SVGElement = SVGElement;
    globalThis.HTMLInputElement = HTMLInputElement;
    globalThis.window = globalThis;
    globalThis.Date = Date;
    globalThis.requestAnimationFrame = (callback) => setTimeout(() => callback(Date.now()), 16);
    globalThis.cancelAnimationFrame = (id) => clearTimeout(id);
    globalThis.getComputedStyle = () => ({});
    globalThis.document = {
        hidden: false,
        documentElement: {},
        body: {},
        addEventListener() {},
        removeEventListener() {},
        querySelector() { return null; },
        querySelectorAll() { return []; }
    };

    await import(pathToFileURL(outfile).href);

    const api = globalThis.AnimeJs;
    assert.equal(api.version(), "4.5.0");

    const target = { x: 0 };
    const animation = api.createAnimation(target, {
        x: 100,
        duration: 100,
        autoplay: false,
        ease: "linear"
    });
    animation.seek(100);

    assert.equal(target.x, 100);
    process.stdout.write(`spike bytes ${fs.statSync(outfile).size}\n`);
});
