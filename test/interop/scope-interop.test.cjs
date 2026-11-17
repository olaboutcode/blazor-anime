const assert = require("node:assert/strict");
const fs = require("node:fs");
const os = require("node:os");
const path = require("node:path");
const test = require("node:test");
const { pathToFileURL } = require("node:url");

const satellitePath = path.join(
    __dirname,
    "../../src/BlazorAnime.Scope/wwwroot/BlazorAnime.Scope.lib.module.js");
const layoutPath = path.join(
    __dirname,
    "../../src/BlazorAnime.Layout/wwwroot/BlazorAnime.Layout.lib.module.js");
const draggablePath = path.join(
    __dirname,
    "../../src/BlazorAnime.Draggable/wwwroot/BlazorAnime.Draggable.lib.module.js");
const corePath = path.join(
    __dirname,
    "../../src/BlazorAnime/wwwroot/BlazorAnime.lib.module.js");

const stage = fs.mkdtempSync(path.join(os.tmpdir(), "blazor-anime-scope-"));
fs.cpSync(path.dirname(corePath), path.join(stage, "BlazorAnime"), { recursive: true });
fs.cpSync(path.dirname(satellitePath), path.join(stage, "BlazorAnime.Scope"), { recursive: true });
const stagedSatellite = path.join(stage, "BlazorAnime.Scope", "BlazorAnime.Scope.lib.module.js");
test.after(() => fs.rmSync(stage, { recursive: true, force: true }));

class DOMMatrix {
    inverse() { return new DOMMatrix(); }
}

class ResizeObserver {
    observe() {}
    disconnect() {}
}

function styleOf() {
    const values = { transform: "" };
    return new Proxy(values, {
        has() { return true; },
        get(target, prop) {
            if (prop === "removeProperty") return (name) => { delete target[name]; };
            if (prop === "setProperty") return (name, value) => { target[name] = value; };
            if (prop === "getPropertyValue") return (name) => target[name] ?? "";
            return target[prop] ?? "";
        },
        set(target, prop, value) {
            target[prop] = value;
            return true;
        }
    });
}

class FakeElement {
    constructor(parent) {
        this.nodeType = 1;
        this.parentElement = parent;
        this.style = styleOf();
        this.dataset = {};
        this.classList = { add() {}, remove() {}, contains() { return false; } };
        this.clientLeft = 0;
        this.clientTop = 0;
        this.clientWidth = 120;
        this.clientHeight = 80;
    }

    getBoundingClientRect() {
        return { top: 0, right: 120, bottom: 80, left: 0, width: 120, height: 80, x: 0, y: 0 };
    }

    querySelectorAll() {
        const list = new globalThis.NodeList();
        list.length = 0;
        return list;
    }
}

function listOf(nodes) {
    const list = new globalThis.NodeList();
    nodes.forEach((node, index) => { list[index] = node; });
    list.length = nodes.length;
    return list;
}

function installDom() {
    function NodeList() {}
    function HTMLCollection() {}
    function Node() {}
    function Element() {}
    function SVGElement() {}
    Node.ELEMENT_NODE = 1;
    Node.TEXT_NODE = 3;
    Node.COMMENT_NODE = 8;
    Object.setPrototypeOf(FakeElement.prototype, Node.prototype);

    const body = new FakeElement(null);
    const queries = [];
    globalThis.__scopeQueries = queries;
    Object.assign(globalThis, {
        NodeList,
        HTMLCollection,
        Node,
        Element,
        SVGElement,
        HTMLElement: Element,
        DOMMatrix,
        ResizeObserver,
        CSS: { registerProperty() {}, escape: (value) => String(value), supports: () => false },
        window: globalThis,
        innerWidth: 800,
        innerHeight: 600,
        scrollX: 0,
        scrollY: 0,
        requestAnimationFrame: () => 0,
        cancelAnimationFrame: () => {},
        matchMedia: (query) => ({
            matches: String(query).includes("max-width"),
            media: String(query),
            addEventListener() {},
            removeEventListener() {}
        }),
        getComputedStyle: () => ({
            transform: "",
            display: "block",
            visibility: "visible",
            position: "static",
            getPropertyValue: () => ""
        }),
        document: {
            hidden: false,
            documentElement: body,
            body,
            addEventListener() {},
            removeEventListener() {},
            querySelector() { return null; },
            querySelectorAll() {
                globalThis.__scopeQueries.push("document");
                throw new Error("selector escaped the scope root");
            }
        }
    });
    return body;
}

async function loadScope() {
    if (!globalThis.__blazorAnimeScopeLoaded) {
        installDom();
        await import(pathToFileURL(stagedSatellite).href);
        globalThis.__blazorAnimeScopeLoaded = true;
    }
    globalThis.AnimeJs.setSpeed(1);
    return globalThis.AnimeJsScope;
}

test("satellite imports the core module and one engine", async () => {
    const source = fs.readFileSync(satellitePath, "utf8");
    assert.match(source, /\.\.\/BlazorAnime\//);
    const core = fs.readFileSync(corePath, "utf8");
    assert.doesNotMatch(core, /AnimeJsScope/);
    assert.doesNotMatch(core, /scope-entry/);
    const layout = fs.readFileSync(layoutPath, "utf8");
    const draggable = fs.readFileSync(draggablePath, "utf8");
    assert.doesNotMatch(layout, /AnimeJsScope/);
    assert.doesNotMatch(draggable, /AnimeJsScope/);
    assert.doesNotMatch(source, /AnimeJsDraggable/);
    assert.doesNotMatch(source, /AnimeJsLayout/);
    assert.doesNotMatch(source, /draggable-entry/);
    assert.doesNotMatch(source, /layout-entry/);

    const api = await loadScope();
    assert.equal(globalThis.AnimeJs.version(), "4.5.0");
    assert.equal(globalThis.AnimeJS.length, 1);

    const root = new FakeElement(globalThis.document.body);
    const child = new FakeElement(root);
    root.querySelectorAll = (selector) => {
        globalThis.__scopeQueries.push(selector);
        return selector === ".square" ? listOf([child]) : listOf([]);
    };

    const scope = api.create(root, { duration: 321, ease: "linear" }, {
        isSmall: "(max-width: 200px)"
    });
    assert.equal(scope.then, undefined);
    assert.equal(scope.root(), root);
    assert.equal(scope.matches().isSmall, true);
    assert.equal(scope.registered(), 0);

    const marker = (name, value) => ({ name, value: { propType: "setter", value } });
    let runs = 0;
    let onceRuns = 0;
    let seenDuration = 0;
    assert.equal(await scope.add({
        invokeMethodAsync: async () => {
            runs += 1;
            const created = await globalThis.AnimeJs.createAnimation({
                targets: marker("targets", ".square"),
                x: marker("x", 12)
            });
            seenDuration = created.getDuration();
            return [];
        }
    }), undefined);
    assert.equal(runs, 1);
    assert.equal(seenDuration, 321);
    assert.ok(globalThis.__scopeQueries.length >= 1);
    assert.ok(globalThis.__scopeQueries.every((query) => query === ".square"));
    assert.equal(scope.registered(), 1);

    const outside = await globalThis.AnimeJs.createAnimation({
        targets: marker("targets", child),
        x: marker("x", 4)
    });
    assert.equal(outside.getDuration(), 1000);
    assert.equal(scope.registered(), 1);

    await scope.refresh();
    assert.equal(runs, 2);
    assert.equal(scope.registered(), 1);
    assert.equal(await scope.addOnce({
        invokeMethodAsync: async () => {
            onceRuns += 1;
            await globalThis.AnimeJs.createAnimation({
                targets: marker("targets", ".square"),
                x: marker("x", 1)
            });
            return [{
                invokeMethodAsync: async () => {},
                dispose() {}
            }];
        }
    }), undefined);
    assert.equal(onceRuns, 1);
    assert.equal(scope.registered(), 2);

    scope.stateSet("i", 4);
    assert.equal(scope.stateGet("i"), 4);
    await scope.refresh();
    assert.equal(runs, 3);
    assert.equal(onceRuns, 1);
    assert.equal(scope.stateGet("i"), 4);
    assert.equal(scope.registered(), 2);

    assert.equal(await scope.revert(), undefined);
    assert.equal(scope.registered(), 0);
    assert.equal(scope.stateGet("i"), 0);
    assert.equal(await scope.refresh(), undefined);
});
