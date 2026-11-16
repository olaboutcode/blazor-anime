const assert = require("node:assert/strict");
const fs = require("node:fs");
const os = require("node:os");
const path = require("node:path");
const test = require("node:test");
const { pathToFileURL } = require("node:url");

const satellitePath = path.join(
    __dirname,
    "../../src/BlazorAnime.Draggable/wwwroot/BlazorAnime.Draggable.lib.module.js");
const animatablePath = path.join(
    __dirname,
    "../../src/BlazorAnime.Animatable/wwwroot/BlazorAnime.Animatable.lib.module.js");
const corePath = path.join(
    __dirname,
    "../../src/BlazorAnime/wwwroot/BlazorAnime.lib.module.js");

// Browser URLs are /_content/BlazorAnime.Draggable/... and /_content/BlazorAnime/chunks/...
const stage = fs.mkdtempSync(path.join(os.tmpdir(), "blazor-anime-"));
fs.cpSync(path.dirname(corePath), path.join(stage, "BlazorAnime"), { recursive: true });
fs.cpSync(path.dirname(satellitePath), path.join(stage, "BlazorAnime.Draggable"), { recursive: true });
const stagedSatellite = path.join(stage, "BlazorAnime.Draggable", "BlazorAnime.Draggable.lib.module.js");
test.after(() => fs.rmSync(stage, { recursive: true, force: true }));

let clock = 1_000_000;

class DOMMatrix {
    inverse() { return new DOMMatrix(); }
    preMultiplySelf() { return this; }
}

class DOMPoint {
    matrixTransform() { return { x: this.x || 0, y: this.y || 0 }; }
}

class ResizeObserver {
    observe() {}
    unobserve() {}
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
        this.classList = { add() {}, remove() {}, contains() { return false; } };
        this.scrollWidth = 800;
        this.scrollHeight = 600;
        this.scrollLeft = 0;
        this.scrollTop = 0;
        this.clientWidth = 400;
        this.clientHeight = 300;
        this.listeners = [];
    }

    addEventListener(type, fn) { this.listeners.push({ type, fn }); }
    removeEventListener() {}
    getBoundingClientRect() {
        return { top: 0, right: 400, bottom: 300, left: 0, width: 400, height: 300, x: 0, y: 0 };
    }
    getAttribute() { return null; }
}

function installDom() {
    function NodeList() {}
    function HTMLCollection() {}
    function Node() {}
    function Element() {}
    function SVGElement() {}
    function HTMLInputElement() {}

    const body = new FakeElement(null);
    Object.setPrototypeOf(FakeElement.prototype, Node.prototype);
    Date.now = () => clock;
    Object.assign(globalThis, {
        NodeList,
        HTMLCollection,
        Node,
        Element,
        SVGElement,
        HTMLInputElement,
        DOMMatrix,
        DOMPoint,
        ResizeObserver,
        window: globalThis,
        innerWidth: 800,
        innerHeight: 600,
        scrollX: 0,
        scrollY: 0,
        scrollBy() {},
        scrollTo() {},
        matchMedia: () => ({ matches: true }),
        requestAnimationFrame: () => 0,
        cancelAnimationFrame: () => {},
        getComputedStyle: () => ({
            transform: "",
            getPropertyValue(name) {
                if (name === "transform") return "";
                if (name === "position") return "absolute";
                if (name === "overflow" || name === "overflow-y") return "visible";
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
            querySelectorAll() { return []; }
        }
    });
    return body;
}

async function loadDraggable() {
    if (!globalThis.__blazorAnimeDraggableLoaded) {
        installDom();
        await import(pathToFileURL(stagedSatellite).href);
        globalThis.__blazorAnimeDraggableLoaded = true;
    }
    globalThis.AnimeJs.setSpeed(1);
    return globalThis.AnimeJsDraggable;
}

function element() {
    return new FakeElement(globalThis.document.body);
}

function settle() {
    clock += 1000;
    globalThis.AnimeJS[0].engine.update();
    clock += 1000;
    globalThis.AnimeJS[0].engine.update();
}

function assertCommand(value) {
    assert.equal(value, undefined);
    assert.equal(value && value.then, undefined);
}

test("satellite imports the core module and one engine", async () => {
    const source = fs.readFileSync(satellitePath, "utf8");
    assert.match(source, /\.\.\/BlazorAnime\//);
    const core = fs.readFileSync(corePath, "utf8");
    assert.doesNotMatch(core, /AnimeJsDraggable/);
    assert.doesNotMatch(core, /draggable-entry/);
    const animatable = fs.readFileSync(animatablePath, "utf8");
    assert.doesNotMatch(animatable, /AnimeJsDraggable/);

    const api = await loadDraggable();
    assert.equal(globalThis.AnimeJs.version(), "4.5.0");
    assert.equal(globalThis.AnimeJS.length, 1);

    globalThis.AnimeJs.setSpeed(4);
    assert.equal(globalThis.AnimeJS[0].engine.speed, 4);
    globalThis.AnimeJs.setSpeed(1);

    const created = api.create(element(), {});
    assert.equal(Object.getPrototypeOf(created), Object.prototype);
    assert.equal(created.then, undefined);
    assert.equal(typeof created.x(), "number");
    assert.equal(Number.isFinite(created.x()), true);
    assert.equal(created.xProp(), "translateX");
    assert.equal(created.yProp(), "translateY");
    assert.equal(created.enabled(), true);
    created.revert();
});

test("commands return undefined and setX moves the axis", async () => {
    const api = await loadDraggable();
    const created = api.create(element(), { snap: 10 });

    assertCommand(created.disable());
    assert.equal(created.enabled(), false);
    assertCommand(created.enable());
    assert.equal(created.enabled(), true);
    assertCommand(created.setX(24, false));
    assertCommand(created.setY(12, false));
    settle();
    assert.ok(Math.abs(created.x() - 24) < 0.001, `x=${created.x()}`);
    assert.ok(Math.abs(created.y() - 12) < 0.001, `y=${created.y()}`);
    assertCommand(created.stop());
    assertCommand(created.refresh());
    assertCommand(created.scrollInView(null, null, null));
    assertCommand(created.animateInView(null, null, null));
    assertCommand(created.reset());
    settle();
    assert.equal(created.x(), 0);
    assert.equal(created.y(), 0);
    assertCommand(created.revert());
});

test("grab callback receives a snapshot and element refs stay objects", async () => {
    const api = await loadDraggable();
    const target = element();
    const calls = [];
    const created = api.create(target, {
        x: false,
        onGrab: {
            propType: "callback",
            dotNetRef: {
                invokeMethodAsync(method, snapshot) {
                    calls.push({ method, snapshot });
                    return Promise.resolve();
                }
            }
        }
    });

    const down = target.listeners.find((item) => item.type === "mousedown");
    assert.ok(down, "mousedown listener");
    const event = {
        type: "mousedown",
        target,
        clientX: 30,
        clientY: 16,
        stopPropagation() {}
    };
    if (typeof down.fn === "function") down.fn(event);
    else down.fn.handleEvent(event);

    assert.equal(calls.length, 1);
    assert.equal(calls[0].method, "Invoke");
    assert.equal(typeof calls[0].snapshot.x, "number");
    assert.equal(calls[0].snapshot.grabbed, true);
    assert.equal(calls[0].snapshot.enabled, true);
    assert.equal(Number.isFinite(calls[0].snapshot.velocity), true);
    assert.equal(created.grabbed(), true);

    const published = [];
    const previous = globalThis.DotNet;
    globalThis.DotNet = {
        createJSObjectReference(value) {
            published.push(value);
            return { __jsObjectId: published.length };
        }
    };
    try {
        assert.deepEqual(created.target(), { __jsObjectId: 1 });
        assert.equal(published[0], target);
        assert.deepEqual(created.trigger(), { __jsObjectId: 2 });
        assert.deepEqual(created.scrollContainer(), { __jsObjectId: 3 });
        assert.equal(published[2], globalThis);
    } finally {
        globalThis.DotNet = previous;
    }

    created.revert();
});
