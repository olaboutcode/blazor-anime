const assert = require("node:assert/strict");
const fs = require("node:fs");
const os = require("node:os");
const path = require("node:path");
const test = require("node:test");
const { pathToFileURL } = require("node:url");

const satellitePath = path.join(
    __dirname,
    "../../src/BlazorAnime.Text/wwwroot/BlazorAnime.Text.lib.module.js");
const waapiPath = path.join(
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

const stage = fs.mkdtempSync(path.join(os.tmpdir(), "blazor-anime-text-"));
fs.cpSync(path.dirname(corePath), path.join(stage, "BlazorAnime"), { recursive: true });
fs.cpSync(path.dirname(satellitePath), path.join(stage, "BlazorAnime.Text"), { recursive: true });
const stagedSatellite = path.join(stage, "BlazorAnime.Text", "BlazorAnime.Text.lib.module.js");
test.after(() => fs.rmSync(stage, { recursive: true, force: true }));

class MiniNode {
    constructor() {
        this.parentNode = null;
        this.childNodes = [];
    }

    appendChild(child) {
        if (child.nodeType === 11) {
            for (const nested of [...child.childNodes]) this.appendChild(nested);
            return child;
        }
        if (child.parentNode) child.parentNode.removeChild(child);
        child.parentNode = this;
        this.childNodes.push(child);
        return child;
    }

    insertBefore(child, reference) {
        if (child.nodeType === 11) {
            for (const nested of [...child.childNodes]) this.insertBefore(nested, reference);
            return child;
        }
        if (child.parentNode) child.parentNode.removeChild(child);
        child.parentNode = this;
        const index = reference ? this.childNodes.indexOf(reference) : this.childNodes.length;
        this.childNodes.splice(index < 0 ? this.childNodes.length : index, 0, child);
        return child;
    }

    replaceChild(next, previous) {
        const index = this.childNodes.indexOf(previous);
        if (previous.parentNode === this) previous.parentNode = null;
        if (index >= 0) this.childNodes.splice(index, 1);
        if (next.nodeType === 11) {
            const children = [...next.childNodes];
            children.forEach((child, offset) => {
                if (child.parentNode) child.parentNode.removeChild(child);
                child.parentNode = this;
                this.childNodes.splice((index < 0 ? this.childNodes.length : index) + offset, 0, child);
            });
        } else {
            if (next.parentNode) next.parentNode.removeChild(next);
            next.parentNode = this;
            this.childNodes.splice(index < 0 ? this.childNodes.length : index, 0, next);
        }
        return previous;
    }

    removeChild(child) {
        const index = this.childNodes.indexOf(child);
        if (index >= 0) this.childNodes.splice(index, 1);
        child.parentNode = null;
        return child;
    }

    get textContent() {
        return this.childNodes.map((child) => child.textContent).join("");
    }

    set textContent(value) {
        this.childNodes = [];
        if (value) this.appendChild(new MiniText(String(value)));
    }

    get parentElement() {
        return this.parentNode && this.parentNode.nodeType === 1 ? this.parentNode : null;
    }
}

class MiniText extends MiniNode {
    constructor(value) {
        super();
        this.nodeType = 3;
        this.nodeValue = String(value);
    }

    get textContent() { return this.nodeValue; }
    set textContent(value) { this.nodeValue = String(value); }
    cloneNode() { return new MiniText(this.nodeValue); }
}

class MiniElement extends MiniNode {
    constructor(tag) {
        super();
        this.nodeType = 1;
        this.tagName = String(tag || "div").toUpperCase();
        this.attrs = {};
        this.style = {};
        this.offsetWidth = 120;
    }

    setAttribute(name, value) { this.attrs[name] = String(value); }
    getAttribute(name) { return Object.prototype.hasOwnProperty.call(this.attrs, name) ? this.attrs[name] : null; }
    get className() { return this.attrs.class || ""; }

    get children() { return this.childNodes.filter((child) => child.nodeType === 1); }
    get childElementCount() { return this.children.length; }
    get firstElementChild() { return this.children[0] || null; }

    get innerHTML() { return this.childNodes.map(serialize).join(""); }
    set innerHTML(value) {
        if (this.tagName === "TEMPLATE") {
            this.content = parseFragment(String(value));
            return;
        }
        const parsed = parseFragment(String(value));
        this.childNodes = [];
        for (const child of [...parsed.childNodes]) this.appendChild(child);
    }

    cloneNode(deep) {
        const copy = new MiniElement(this.tagName);
        copy.attrs = { ...this.attrs };
        if (this.content) copy.content = this.content.cloneNode(true);
        if (deep) {
            for (const child of this.childNodes) copy.appendChild(child.cloneNode(true));
        }
        return copy;
    }

    replaceWith(...nodes) {
        const parent = this.parentNode;
        if (!parent) return;
        const index = parent.childNodes.indexOf(this);
        parent.childNodes.splice(index, 1);
        this.parentNode = null;
        nodes.forEach((node, offset) => {
            if (node.parentNode) node.parentNode.removeChild(node);
            node.parentNode = parent;
            parent.childNodes.splice(index + offset, 0, node);
        });
    }

    querySelector(selector) { return queryAll(this, selector)[0] || null; }
    querySelectorAll(selector) { return queryAll(this, selector); }
    getBoundingClientRect() { return { top: 0, left: 0, width: 20, height: 12, right: 20, bottom: 12 }; }
}

class MiniFragment extends MiniNode {
    constructor() { super(); this.nodeType = 11; }
    get firstElementChild() { return this.childNodes.find((child) => child.nodeType === 1) || null; }
    querySelector(selector) { return queryAll(this, selector)[0] || null; }
    querySelectorAll(selector) { return queryAll(this, selector); }
    cloneNode(deep) {
        const copy = new MiniFragment();
        if (deep) {
            for (const child of this.childNodes) copy.appendChild(child.cloneNode(true));
        }
        return copy;
    }
}

function serialize(node) {
    if (node.nodeType === 3) return node.nodeValue;
    if (node.nodeType === 11) return node.childNodes.map(serialize).join("");
    const attrs = Object.entries(node.attrs).map(([name, value]) => ` ${name}="${value}"`).join("");
    return `<${node.tagName.toLowerCase()}${attrs}>${node.childNodes.map(serialize).join("")}</${node.tagName.toLowerCase()}>`;
}

function parseFragment(html) {
    const fragment = new MiniFragment();
    const stack = [fragment];
    const pattern = /<\/([a-zA-Z0-9]+)>|<([a-zA-Z0-9]+)([^>]*?)(\/?)>|([^<]+)/g;
    let match;
    while ((match = pattern.exec(html))) {
        if (match[1]) {
            const name = match[1].toUpperCase();
            while (stack.length > 1 && stack[stack.length - 1].tagName !== name) stack.pop();
            if (stack.length > 1) stack.pop();
            continue;
        }
        if (match[5]) {
            stack[stack.length - 1].appendChild(new MiniText(match[5]));
            continue;
        }
        const element = new MiniElement(match[2]);
        const attrs = match[3] || "";
        const attrPattern = /([^\s=\/]+)(?:="([^"]*)")?/g;
        let attr;
        while ((attr = attrPattern.exec(attrs))) {
            if (attr[1]) element.setAttribute(attr[1], attr[2] ?? "");
        }
        stack[stack.length - 1].appendChild(element);
        if (!match[4] && element.tagName !== "BR") stack.push(element);
    }
    return fragment;
}

function descendants(root) {
    const found = [];
    const visit = (node) => {
        for (const child of node.childNodes || []) {
            if (child.nodeType === 1) found.push(child);
            if (child.content) visit(child.content);
            visit(child);
        }
    };
    visit(root);
    return found;
}

function matchSimple(element, selector) {
    if (!element || element.nodeType !== 1) return false;
    let rest = selector.trim();
    const tag = rest.match(/^[a-zA-Z0-9]+/);
    if (tag) {
        if (element.tagName !== tag[0].toUpperCase()) return false;
        rest = rest.slice(tag[0].length);
    }
    const classes = [...rest.matchAll(/\.([a-zA-Z0-9_-]+)/g)].map((item) => item[1]);
    rest = rest.replace(/\.([a-zA-Z0-9_-]+)/g, "");
    const className = element.className.split(/\s+/).filter(Boolean);
    if (classes.some((name) => !className.includes(name))) return false;
    const attrs = [...rest.matchAll(/\[([^\]=~]+)(?:="([^"]*)")?\]/g)];
    return attrs.every((item) => element.getAttribute(item[1]) !== null && (item[2] === undefined || element.getAttribute(item[1]) === item[2]));
}

function hasAncestor(element, selector) {
    let parent = element.parentElement;
    while (parent) {
        if (matchSimple(parent, selector)) return true;
        parent = parent.parentElement;
    }
    return false;
}

function queryAll(root, selector) {
    const notAt = selector.lastIndexOf(":not(");
    let base = selector;
    let excluded = null;
    if (notAt >= 0) {
        base = selector.slice(0, notAt);
        excluded = selector.slice(notAt + 5, selector.lastIndexOf(")"));
    }
    return descendants(root).filter((element) => {
        if (!matchSimple(element, base)) return false;
        if (!excluded) return true;
        const parts = excluded.trim().split(/\s+/);
        return !parts.every((part, index) => index === parts.length - 1 ? matchSimple(element, part) : hasAncestor(element, part));
    });
}

function installDom() {
    const body = new MiniElement("body");
    const document = {
        body,
        documentElement: new MiniElement("html"),
        fonts: { status: "loaded", ready: Promise.resolve() },
        createElement: (tag) => new MiniElement(tag),
        createTextNode: (text) => new MiniText(text),
        createDocumentFragment: () => new MiniFragment(),
        querySelector: () => null,
        querySelectorAll: () => []
    };
    globalThis.window = globalThis;
    globalThis.document = document;
    globalThis.Node = MiniNode;
    globalThis.Element = MiniElement;
    globalThis.HTMLElement = MiniElement;
    globalThis.DocumentFragment = MiniFragment;
    globalThis.ResizeObserver = class {
        observe() {}
        disconnect() {}
        unobserve() {}
    };
    globalThis.requestAnimationFrame = (callback) => setTimeout(() => callback(Date.now()), 0);
    globalThis.cancelAnimationFrame = (id) => clearTimeout(id);
    globalThis.addEventListener = () => {};
    globalThis.removeEventListener = () => {};
    globalThis.innerWidth = 800;
    globalThis.innerHeight = 600;
    globalThis.getComputedStyle = () => new Proxy({ transform: "", display: "block" }, {
        get(target, prop) {
            if (prop === "getPropertyValue") return () => "0";
            return target[prop] ?? "0";
        }
    });
    if (!globalThis.CSS) globalThis.CSS = { registerProperty() {}, escape: (value) => String(value), supports: () => false };
    document.hidden = false;
    document.addEventListener = () => {};
    document.removeEventListener = () => {};
}

async function loadText() {
    if (!globalThis.__blazorAnimeTextLoaded) {
        installDom();
        await import(pathToFileURL(stagedSatellite).href);
        globalThis.__blazorAnimeTextLoaded = true;
    }
    globalThis.AnimeJs.setSpeed(1);
    return globalThis.AnimeJsText;
}

function paragraph(text) {
    const element = globalThis.document.createElement("p");
    element.appendChild(globalThis.document.createTextNode(text));
    return element;
}

test("satellite imports the core module and one engine", async () => {
    const source = fs.readFileSync(satellitePath, "utf8");
    assert.match(source, /\.\.\/BlazorAnime\//);
    const core = fs.readFileSync(corePath, "utf8");
    assert.doesNotMatch(core, /AnimeJsText/);
    assert.doesNotMatch(core, /text-entry/);
    for (const file of [waapiPath, eventsPath, scopePath, layoutPath, draggablePath]) {
        assert.doesNotMatch(fs.readFileSync(file, "utf8"), /AnimeJsText/);
    }
    assert.doesNotMatch(source, /AnimeJsDraggable/);
    assert.doesNotMatch(source, /AnimeJsLayout/);
    assert.doesNotMatch(source, /AnimeJsScope/);
    assert.doesNotMatch(source, /AnimeJsEvents/);
    assert.doesNotMatch(source, /AnimeJsWaapi/);

    const api = await loadText();
    assert.equal(globalThis.AnimeJs.version(), "4.5.0");
    assert.equal(globalThis.AnimeJS.length, 1);

    const element = paragraph("Hello there");
    const split = await api.create(element, { chars: true });
    const words = split.words();
    const chars = split.chars();
    assert.ok(words.length >= 2, `words were ${words.length}`);
    assert.ok(chars.length >= 5, `chars were ${chars.length}`);
    assert.equal(words[0].target.getAttribute("data-word"), "0");
    assert.equal(typeof words[0].target, "object");
    assert.equal(await split.refresh(), undefined);
    assert.ok(split.words().length >= 2);
    assert.equal(await split.revert(), undefined);
    assert.equal(element.textContent, "Hello there");

    const sentence = paragraph("Hello");
    const scramble = api.scramble({ chars: "uppercase", cursor: "_" });
    assert.equal(typeof scramble, "function");
    assert.equal(scramble.then, undefined);
    const first = scramble(sentence, 0, [sentence]);
    assert.equal(typeof first.modifier, "function");
    assert.equal(typeof first.duration, "number");
    globalThis.AnimeJs.setSpeed(4);
    const second = scramble(sentence, 0, [sentence]);
    assert.equal(second.duration, first.duration);
    globalThis.AnimeJs.setSpeed(1);
});
