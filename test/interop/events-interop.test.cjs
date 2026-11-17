const assert = require("node:assert/strict");
const fs = require("node:fs");
const os = require("node:os");
const path = require("node:path");
const test = require("node:test");
const { pathToFileURL } = require("node:url");

const satellitePath = path.join(
    __dirname,
    "../../src/BlazorAnime.Events/wwwroot/BlazorAnime.Events.lib.module.js");
const scopePath = path.join(
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

const stage = fs.mkdtempSync(path.join(os.tmpdir(), "blazor-anime-events-"));
fs.cpSync(path.dirname(corePath), path.join(stage, "BlazorAnime"), { recursive: true });
fs.cpSync(path.dirname(satellitePath), path.join(stage, "BlazorAnime.Events"), { recursive: true });
const stagedSatellite = path.join(stage, "BlazorAnime.Events", "BlazorAnime.Events.lib.module.js");
test.after(() => {
    Date.now = realDateNow;
    fs.rmSync(stage, { recursive: true, force: true });
});

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
        get(target, prop) {
            if (prop === "removeProperty") return (name) => { delete target[name]; };
            if (prop === "setProperty") return (name, value) => { target[name] = value; };
            if (prop === "getPropertyValue") return (name) => target[name] ?? "";
            return target[prop] ?? "";
        },
        set(target, prop, value) {
            target[prop] = value;
            return true;
        },
        has(target, prop) {
            return typeof prop === "string" || prop in target;
        }
    });
}

class FakeElement {
    constructor(parent) {
        this.nodeType = 1;
        this.parentNode = parent;
        this.parentElement = parent;
        this.childNodes = [];
        this.style = styleOf();
        this.dataset = {};
        this.classList = { add() {}, remove() {}, contains() { return false; } };
        this.clientLeft = 0;
        this.clientTop = 0;
        this.clientWidth = 120;
        this.clientHeight = 80;
        this.offsetWidth = 120;
        this.offsetHeight = 80;
        this.scrollTop = 0;
        this.scrollLeft = 0;
        this.scrollWidth = 120;
        this.scrollHeight = 240;
    }

    getBoundingClientRect() {
        return { top: 0, right: 120, bottom: 80, left: 0, width: 120, height: 80, x: 0, y: 0 };
    }

    appendChild(child) {
        this.childNodes.push(child);
        child.parentNode = this;
        child.parentElement = this;
        return child;
    }

    removeChild(child) {
        const index = this.childNodes.indexOf(child);
        if (index >= 0) this.childNodes.splice(index, 1);
        child.parentNode = null;
        child.parentElement = null;
        return child;
    }

    addEventListener() {}
    removeEventListener() {}
    getAttribute() { return null; }
    setAttribute() {}
    removeAttribute() {}
    hasAttribute() { return false; }
    querySelector() { return null; }
    querySelectorAll() {
        const list = new globalThis.NodeList();
        list.length = 0;
        return list;
    }
}

// anime.js measures 100lvh by appending a probe, then refreshes the observer
// on the next frame. The engine captures requestAnimationFrame and Date.now
// when the bundle loads, so both have to be real before the import.
const frames = [];
let frameId = 0;
let nowMs = 1_000_000;
const realDateNow = Date.now;

function flushFrames(ms = 50) {
    nowMs += ms;
    const due = frames.splice(0, frames.length);
    for (const frame of due) frame.cb(nowMs);
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
    Date.now = () => nowMs;
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
        addEventListener() {},
        removeEventListener() {},
        requestAnimationFrame: (callback) => {
            const id = ++frameId;
            frames.push({ id, cb: callback });
            return id;
        },
        cancelAnimationFrame: (id) => {
            const index = frames.findIndex((frame) => frame.id === id);
            if (index >= 0) frames.splice(index, 1);
        },
        getComputedStyle: () => ({
            transform: "",
            display: "block",
            visibility: "visible",
            position: "static",
            marginLeft: "0",
            marginRight: "0",
            marginTop: "0",
            marginBottom: "0",
            getPropertyValue: () => "0"
        }),
        document: {
            hidden: false,
            documentElement: body,
            body,
            addEventListener() {},
            removeEventListener() {},
            createElement: () => new FakeElement(body),
            querySelector() { return null; },
            querySelectorAll() {
                const list = new NodeList();
                list.length = 0;
                return list;
            }
        }
    });
    return body;
}

async function loadEvents() {
    if (!globalThis.__blazorAnimeEventsLoaded) {
        installDom();
        await import(pathToFileURL(stagedSatellite).href);
        globalThis.__blazorAnimeEventsLoaded = true;
    }
    globalThis.AnimeJs.setSpeed(1);
    return globalThis.AnimeJsEvents;
}

test("satellite imports the core module and one engine", async () => {
    const source = fs.readFileSync(satellitePath, "utf8");
    assert.match(source, /\.\.\/BlazorAnime\//);
    const core = fs.readFileSync(corePath, "utf8");
    assert.doesNotMatch(core, /AnimeJsEvents/);
    assert.doesNotMatch(core, /events-entry/);
    assert.doesNotMatch(fs.readFileSync(scopePath, "utf8"), /AnimeJsEvents/);
    assert.doesNotMatch(fs.readFileSync(layoutPath, "utf8"), /AnimeJsEvents/);
    assert.doesNotMatch(fs.readFileSync(draggablePath, "utf8"), /AnimeJsEvents/);
    assert.doesNotMatch(source, /AnimeJsDraggable/);
    assert.doesNotMatch(source, /AnimeJsLayout/);
    assert.doesNotMatch(source, /AnimeJsScope/);

    const api = await loadEvents();
    assert.equal(globalThis.AnimeJs.version(), "4.5.0");
    assert.equal(globalThis.AnimeJS.length, 1);

    const created = api.create(null, null, {
        axis: "x",
        repeat: true,
        debug: false,
        enter: "bottom top",
        sync: "play pause"
    });
    flushFrames();
    assert.equal(created.then, undefined);
    const observer = created.handle();
    assert.equal(observer.horizontal, true);
    assert.equal(observer.repeat, true);
    assert.equal(observer.linked, null);
    assert.equal(created.refresh(), undefined);
    assert.equal(await created.refresh(), undefined);

    const element = new FakeElement(globalThis.document.body);
    const marker = (name, value) => ({ name, value: { propType: "setter", value } });
    const animation = await globalThis.AnimeJs.createAnimation({
        targets: marker("targets", element),
        x: marker("x", 20),
        duration: marker("duration", 500),
        autoplay: { name: "autoplay", value: { propType: "svgSetter", value: observer } }
    });
    assert.equal(observer.linked == null, false);
    assert.equal(animation.isPaused(), 1);
    assert.equal(created.revert(), undefined);
    assert.equal(await created.revert(), undefined);
});
