const assert = require("node:assert/strict");
const fs = require("node:fs");
const os = require("node:os");
const path = require("node:path");
const test = require("node:test");
const { pathToFileURL } = require("node:url");

const satellitePath = path.join(
    __dirname,
    "../../src/BlazorAnime.Layout/wwwroot/BlazorAnime.Layout.lib.module.js");
const draggablePath = path.join(
    __dirname,
    "../../src/BlazorAnime.Draggable/wwwroot/BlazorAnime.Draggable.lib.module.js");
const corePath = path.join(
    __dirname,
    "../../src/BlazorAnime/wwwroot/BlazorAnime.lib.module.js");

const stage = fs.mkdtempSync(path.join(os.tmpdir(), "blazor-anime-layout-"));
fs.cpSync(path.dirname(corePath), path.join(stage, "BlazorAnime"), { recursive: true });
fs.cpSync(path.dirname(satellitePath), path.join(stage, "BlazorAnime.Layout"), { recursive: true });
const stagedSatellite = path.join(stage, "BlazorAnime.Layout", "BlazorAnime.Layout.lib.module.js");
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
        this.classList = {
            add() {},
            remove() {},
            contains() { return false; }
        };
        this.clientLeft = 0;
        this.clientTop = 0;
        this.clientWidth = 120;
        this.clientHeight = 80;
        this.lastElementChild = null;
        this.previousElementSibling = null;
        this.previousSibling = null;
        this.nextSibling = null;
    }

    contains(node) {
        let current = node;
        while (current) {
            if (current === this) return true;
            current = current.parentElement;
        }
        return false;
    }

    querySelectorAll() {
        return emptyList();
    }

    getBoundingClientRect() {
        return { top: 0, right: 120, bottom: 80, left: 0, width: 120, height: 80, x: 0, y: 0 };
    }

    removeAttribute(name) {
        if (name === "data-layout-id") delete this.dataset.layoutId;
    }

    // Web Animations entry used by layout's transform pass. The test does not read pixels.
    animate() {
        return {
            cancel() {},
            finish() {},
            play() {},
            pause() {},
            commitStyles() {},
            persist() {},
            currentTime: 0,
            finished: Promise.resolve(),
            ready: Promise.resolve()
        };
    }
}

function emptyList() {
    const list = new NodeList();
    list.length = 0;
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

    const body = new FakeElement(null);
    Object.setPrototypeOf(FakeElement.prototype, Node.prototype);
    Object.setPrototypeOf(NodeList.prototype, Object.prototype);
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
        matchMedia: () => ({ matches: true }),
        getComputedStyle: () => ({
            transform: "",
            display: "block",
            visibility: "visible",
            position: "static",
            getPropertyValue(name) {
                if (name === "display") return "block";
                if (name === "visibility") return "visible";
                if (name === "position") return "static";
                return "";
            }
        }),
        document: {
            hidden: false,
            documentElement: body,
            body,
            addEventListener() {},
            removeEventListener() {},
            querySelector() { return null; },
            querySelectorAll() { return emptyList(); }
        }
    });
    return body;
}

async function loadLayout() {
    if (!globalThis.__blazorAnimeLayoutLoaded) {
        installDom();
        await import(pathToFileURL(stagedSatellite).href);
        globalThis.__blazorAnimeLayoutLoaded = true;
    }
    globalThis.AnimeJs.setSpeed(1);
    return globalThis.AnimeJsLayout;
}

function assertCommand(value) {
    assert.equal(value, undefined);
    assert.equal(value && value.then, undefined);
}

test("satellite imports the core module and one engine", async () => {
    const source = fs.readFileSync(satellitePath, "utf8");
    assert.match(source, /\.\.\/BlazorAnime\//);
    const core = fs.readFileSync(corePath, "utf8");
    assert.doesNotMatch(core, /AnimeJsLayout/);
    assert.doesNotMatch(core, /layout-entry/);
    const draggable = fs.readFileSync(draggablePath, "utf8");
    assert.doesNotMatch(draggable, /AnimeJsLayout/);
    assert.doesNotMatch(source, /AnimeJsDraggable/);
    assert.doesNotMatch(source, /draggable-entry/);

    const api = await loadLayout();
    assert.equal(globalThis.AnimeJs.version(), "4.5.0");
    assert.equal(globalThis.AnimeJS.length, 1);

    globalThis.AnimeJs.setSpeed(4);
    assert.equal(globalThis.AnimeJS[0].engine.speed, 4);
    globalThis.AnimeJs.setSpeed(1);

    const calls = [];
    const created = api.create(new FakeElement(globalThis.document.body), {
        duration: 200,
        delay: { propType: "stagger", value: 10, options: {} },
        ease: { propType: "easeFn", value: { fn: "steps", steps: 4 } },
        enterFrom: { opacity: 0 },
        onBegin: {
            propType: "callback",
            dotNetRef: {
                invokeMethodAsync(method, snapshot) {
                    calls.push({ method, snapshot });
                    return Promise.resolve();
                }
            }
        }
    });
    assert.equal(Object.getPrototypeOf(created), Object.prototype);
    assert.equal(created.then, undefined);
    assert.equal(typeof created.id(), "string");
    assert.ok(created.id().length > 0);
    assertCommand(created.record());
    assertCommand(created.animate({ duration: 120 }));
    assertCommand(created.revert());
    assert.equal(calls.length > 0, true);
    assert.equal(calls[0].method, "Invoke");
    assert.equal(typeof calls[0].snapshot.progress, "number");
    assert.equal(typeof calls[0].snapshot.loop, "number");
});
