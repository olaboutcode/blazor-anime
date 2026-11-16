const assert = require("node:assert/strict");
const fs = require("node:fs");
const os = require("node:os");
const path = require("node:path");
const test = require("node:test");
const { pathToFileURL } = require("node:url");

const satellitePath = path.join(
    __dirname,
    "../../src/BlazorAnime.Animatable/wwwroot/BlazorAnime.Animatable.lib.module.js");
const corePath = path.join(
    __dirname,
    "../../src/BlazorAnime/wwwroot/BlazorAnime.lib.module.js");

// Browser URLs are /_content/BlazorAnime.Animatable/... and /_content/BlazorAnime/chunks/...
// The repo keeps those files under two wwwroot folders, so stage the published layout.
const stage = fs.mkdtempSync(path.join(os.tmpdir(), "blazor-anime-"));
fs.cpSync(path.dirname(corePath), path.join(stage, "BlazorAnime"), { recursive: true });
fs.cpSync(path.dirname(satellitePath), path.join(stage, "BlazorAnime.Animatable"), { recursive: true });
const stagedSatellite = path.join(stage, "BlazorAnime.Animatable", "BlazorAnime.Animatable.lib.module.js");
test.after(() => fs.rmSync(stage, { recursive: true, force: true }));

let clock = 1_000_000;

function installDom() {
    function NodeList() {}
    function HTMLCollection() {}
    function Node() {}
    function Element() {}
    function SVGElement() {}
    function HTMLInputElement() {}

    Date.now = () => clock;
    Object.assign(globalThis, {
        NodeList,
        HTMLCollection,
        Node,
        Element,
        SVGElement,
        HTMLInputElement,
        window: globalThis,
        requestAnimationFrame: () => 0,
        cancelAnimationFrame: () => {},
        getComputedStyle: () => ({}),
        document: {
            hidden: false,
            documentElement: {},
            body: {},
            addEventListener() {},
            removeEventListener() {},
            querySelector() { return null; },
            querySelectorAll() { return []; }
        }
    });
}

async function loadAnimatable() {
    if (!globalThis.__blazorAnimeAnimatableLoaded) {
        installDom();
        await import(pathToFileURL(stagedSatellite).href);
        globalThis.__blazorAnimeAnimatableLoaded = true;
    }
    globalThis.AnimeJs.setSpeed(1);
    return globalThis.AnimeJsAnimatable;
}

test("satellite imports the core module and one engine", async () => {
    const source = fs.readFileSync(satellitePath, "utf8");
    assert.match(source, /\.\.\/BlazorAnime\//);
    const core = fs.readFileSync(corePath, "utf8");
    assert.doesNotMatch(core, /BlazorAnime\.Animatable\.lib\.module/);

    const api = await loadAnimatable();
    assert.equal(globalThis.AnimeJs.version(), "4.5.0");
    assert.equal(globalThis.AnimeJS.length, 1);

    globalThis.AnimeJs.setSpeed(4);
    assert.equal(globalThis.AnimeJS[0].engine.speed, 4);
    globalThis.AnimeJs.setSpeed(1);

    const created = api.create({ x: 0 }, { x: 500, ease: "linear" });
    assert.equal(Object.getPrototypeOf(created), Object.prototype);
    assert.equal(created.then, undefined);
    assert.equal(created.get("x").many, false);
    assert.equal(created.get("x").values[0], 0);
});

test("setter reaches the object on the shared clock", async () => {
    const api = await loadAnimatable();
    const target = { x: 0, y: 0 };
    const created = api.create(target, {
        x: 1000,
        y: 1000,
        ease: "linear"
    });

    created.set("x", 80);
    created.set("y", 40, 500, "linear");
    clock += 1000;
    globalThis.AnimeJS[0].engine.update();
    clock += 1000;
    globalThis.AnimeJS[0].engine.update();

    assert.ok(Math.abs(created.get("x").values[0] - 80) < 0.001, `x=${created.get("x").values[0]}`);
    assert.ok(Math.abs(created.get("y").values[0] - 40) < 0.001, `y=${created.get("y").values[0]}`);

    created.revert();
    assert.throws(() => created.get("x"), /no value/);
});

test("spring ease and stagger duration share the core engine", async () => {
    const api = await loadAnimatable();
    const rows = [{ n: 0 }, { n: 0 }, { n: 0 }];
    const created = api.create(rows, {
        n: {
            propType: "stagger",
            value: 20,
            options: {
                start: { name: "start", value: { propType: "setter", value: 100 } }
            }
        },
        ease: { propType: "easeFn", value: { fn: "spring" } }
    });
    created.set("n", 5);
    assert.equal(globalThis.AnimeJS.length, 1);
    created.pause();
});

function settle() {
    clock += 1000;
    globalThis.AnimeJS[0].engine.update();
    clock += 1000;
    globalThis.AnimeJS[0].engine.update();
}

test("snap and invert modify the value anime.js stores", async () => {
    const api = await loadAnimatable();
    const step = 0.25;
    const snappedTarget = { n: 0 };
    const snapped = api.create(snappedTarget, {
        n: 0,
        modifier: { propType: "modifier", fn: "snap", increment: step },
        duration: 0
    });
    snapped.set("n", 0.4, 0);
    settle();
    assert.equal(snappedTarget.n, 0.5);
    assert.equal(snapped.get("n").values[0], 0.5);

    const invertedTarget = { n: 0 };
    const inverted = api.create(invertedTarget, {
        n: 0,
        modifier: { propType: "modifier", fn: "invert" },
        duration: 0
    });
    inverted.set("n", 0.4, 0);
    settle();
    assert.ok(Math.abs(invertedTarget.n + 0.4) < 0.0001, `stored=${invertedTarget.n}`);
    assert.ok(Math.abs(inverted.get("n").values[0] + 0.4) < 0.0001, `get=${inverted.get("n").values[0]}`);

    const propertyTarget = { n: 0 };
    const propertyWins = api.create(propertyTarget, {
        n: { modifier: { propType: "modifier", fn: "snap", increment: step } },
        modifier: { propType: "modifier", fn: "invert" },
        duration: 0
    });
    propertyWins.set("n", 0.4, 0);
    settle();
    assert.equal(propertyTarget.n, 0.5);
    assert.equal(propertyWins.get("n").values[0], 0.5);
});

test("targets and animations are the resolved instance fields", async () => {
    const api = await loadAnimatable();
    const target = { x: 0, y: 0 };
    const created = api.create(target, { x: 100, y: 200 });
    assert.deepEqual(created.targets(), [target]);
    const listed = created.animations();
    assert.deepEqual(listed.names, ["x", "y"]);
    assert.equal(listed.animations.length, 2);
    assert.equal(listed.animations[0].then, undefined);
    assert.equal(typeof listed.animations[0].getDuration(), "number");
    listed.animations[0].pause();
    created.set("x", 12);
    settle();
    assert.equal(listed.animations[0].getCurrentValues().x, "12");
    created.revert();
    assert.deepEqual(created.targets(), []);
    assert.deepEqual(created.animations().names, []);
});

test("targets and animations are JS object references when DotNet is present", async () => {
    const api = await loadAnimatable();
    const registry = new Map();
    let nextId = 1;
    const previous = globalThis.DotNet;
    globalThis.DotNet = {
        createJSObjectReference(value) {
            const id = nextId++;
            registry.set(id, value);
            return { __jsObjectId: id };
        }
    };
    try {
        const target = { x: 1 };
        const created = api.create(target, { x: 50 });
        const [published] = created.targets();
        assert.equal(registry.get(published.__jsObjectId), target);
        const listed = created.animations();
        const handle = registry.get(listed.animations[0].__jsObjectId);
        assert.equal(handle.then, undefined);
        assert.equal(typeof handle.getDuration(), "number");
        handle.pause();
    } finally {
        if (previous === undefined) delete globalThis.DotNet;
        else globalThis.DotNet = previous;
    }
});
