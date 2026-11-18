const assert = require("node:assert/strict");
const fs = require("node:fs");
const os = require("node:os");
const path = require("node:path");
const test = require("node:test");
const { pathToFileURL } = require("node:url");

const satellitePath = path.join(
    __dirname,
    "../../src/BlazorAnime.Waapi/wwwroot/BlazorAnime.Waapi.lib.module.js");
const eventsPath = path.join(
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

const stage = fs.mkdtempSync(path.join(os.tmpdir(), "blazor-anime-waapi-"));
fs.cpSync(path.dirname(corePath), path.join(stage, "BlazorAnime"), { recursive: true });
fs.cpSync(path.dirname(satellitePath), path.join(stage, "BlazorAnime.Waapi"), { recursive: true });
const stagedSatellite = path.join(stage, "BlazorAnime.Waapi", "BlazorAnime.Waapi.lib.module.js");
test.after(() => {
    Date.now = realDateNow;
    fs.rmSync(stage, { recursive: true, force: true });
});

class DOMMatrix {
    inverse() { return new DOMMatrix(); }
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
        this.tagName = "DIV";
        this.id = "";
    }

    animate(_keyframes, params) {
        const animation = {
            playbackRate: 1,
            currentTime: 0,
            play() {},
            pause() {},
            reverse() {},
            cancel() {},
            commitStyles() {},
            effect: { getTiming: () => ({ duration: params.duration }) }
        };
        return animation;
    }

    getBoundingClientRect() {
        return { top: 0, right: 28, bottom: 28, left: 0, width: 28, height: 28, x: 0, y: 0 };
    }

    addEventListener() {}
    removeEventListener() {}
    getAttribute() { return ""; }
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

const frames = [];
let frameId = 0;
let nowMs = 1_000_000;
const realDateNow = Date.now;

function installDom() {
    function NodeList() {}
    function HTMLCollection() {}
    function Node() {}
    function Element() {}
    function SVGElement() {}
    Node.ELEMENT_NODE = 1;
    Node.TEXT_NODE = 3;
    Node.COMMENT_NODE = 8;
    Object.setPrototypeOf(FakeElement.prototype, Element.prototype);
    Object.setPrototypeOf(Element.prototype, Node.prototype);

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
        CSS: { registerProperty() {}, escape: (value) => String(value), supports: () => false },
        window: globalThis,
        innerWidth: 800,
        innerHeight: 600,
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
        getComputedStyle: () => new Proxy({ transform: "", display: "block" }, {
            get(target, prop) {
                if (prop === "getPropertyValue") return () => "0";
                return target[prop] ?? "0";
            }
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
}

async function loadWaapi() {
    if (!globalThis.__blazorAnimeWaapiLoaded) {
        installDom();
        await import(pathToFileURL(stagedSatellite).href);
        globalThis.__blazorAnimeWaapiLoaded = true;
    }
    globalThis.AnimeJs.setSpeed(1);
    return globalThis.AnimeJsWaapi;
}

test("satellite imports the core module and one engine", async () => {
    const source = fs.readFileSync(satellitePath, "utf8");
    assert.match(source, /\.\.\/BlazorAnime\//);
    const core = fs.readFileSync(corePath, "utf8");
    assert.doesNotMatch(core, /AnimeJsWaapi/);
    assert.doesNotMatch(core, /waapi-entry/);
    assert.doesNotMatch(fs.readFileSync(eventsPath, "utf8"), /AnimeJsWaapi/);
    assert.doesNotMatch(fs.readFileSync(scopePath, "utf8"), /AnimeJsWaapi/);
    assert.doesNotMatch(fs.readFileSync(layoutPath, "utf8"), /AnimeJsWaapi/);
    assert.doesNotMatch(fs.readFileSync(draggablePath, "utf8"), /AnimeJsWaapi/);
    assert.doesNotMatch(source, /AnimeJsDraggable/);
    assert.doesNotMatch(source, /AnimeJsLayout/);
    assert.doesNotMatch(source, /AnimeJsScope/);
    assert.doesNotMatch(source, /AnimeJsEvents/);

    const api = await loadWaapi();
    assert.equal(globalThis.AnimeJs.version(), "4.5.0");
    assert.equal(globalThis.AnimeJS.length, 1);

    const element = new FakeElement(globalThis.document.body);
    const created = await api.create(element, {
        x: 80,
        duration: 400,
        autoplay: false
    });
    assert.equal(created.then, undefined);
    const handle = created.handle();
    assert.equal(handle.then, undefined);
    assert.equal(typeof handle.pause, "function");
    assert.equal(created.play(), undefined);
    assert.equal(await created.pause(), undefined);
    assert.equal(created.seek(10), undefined);
    assert.equal(created.revert(), undefined);
    assert.equal(await created.commitStyles(), undefined);

    const linear = api.convertEase({
        propType: "easeFn",
        value: { fn: "spring", mass: 1, stiffness: 100, damping: 10, velocity: 0 }
    });
    assert.match(linear, /^linear\(/);

    const synced = await api.create(element, { x: 40, duration: 300, autoplay: false });
    globalThis.AnimeJs.setSpeed(4);
    assert.equal(synced.speed(), 1);
    const timeline = await globalThis.AnimeJs.createTimeline({ autoplay: false });
    assert.equal(timeline.sync(synced.handle(), 0), undefined);
    assert.equal(await timeline.sync(synced.handle(), 120), undefined);
});
