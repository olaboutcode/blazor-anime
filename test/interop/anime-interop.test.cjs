const assert = require("node:assert/strict");
const fs = require("node:fs");
const path = require("node:path");
const test = require("node:test");
const vm = require("node:vm");

function loadAnime() {
    function NodeList() {}
    function HTMLCollection() {}
    function Node() {}
    function Element() {}
    function SVGElement() {}
    function HTMLInputElement() {}

    const sandbox = {
        NodeList,
        HTMLCollection,
        Node,
        Element,
        SVGElement,
        HTMLInputElement,
        console,
        setTimeout,
        clearTimeout,
        requestAnimationFrame: (callback) => setTimeout(() => callback(Date.now()), 16),
        cancelAnimationFrame: (id) => clearTimeout(id),
        document: {
            hidden: false,
            addEventListener() {},
            querySelector() { return null; },
            querySelectorAll() { return []; }
        }
    };
    sandbox.window = sandbox;
    sandbox.globalThis = sandbox;
    vm.createContext(sandbox);

    const scriptPath = path.join(__dirname, "../../src/BlazorAnime/wwwroot/blazor.anime.interop.js");
    vm.runInContext(fs.readFileSync(scriptPath, "utf8"), sandbox);
    return sandbox.AnimeJs;
}

function prop(name, propType, value) {
    return { name, value: { propType, value } };
}

function setter(name, value) {
    return prop(name, "setter", value);
}

test("object target seeks to the requested number", async () => {
    const api = loadAnime();
    assert.equal(api.version(), "3.2.2");

    const target = api.createObject({ x: 0 });
    const animation = await api.createAnimation({
        targets: prop("targets", "objectTarget", target),
        x: setter("x", 100),
        duration: setter("duration", 100),
        autoplay: setter("autoplay", false),
        easing: setter("easing", "linear")
    });

    animation.seek(100);
    assert.equal(target.target.x, 100);
    assert.equal(animation.getProgress(), 100);
    assert.equal(animation.hasCompleted(), 1);
    assert.equal(animation.getCurrentValues().x, "100");
});

test("function values are collected once per target", async () => {
    const api = loadAnime();
    const rows = [
        api.createObject({ n: 0 }),
        api.createObject({ n: 0 }),
        api.createObject({ n: 0 })
    ];

    const animation = await api.createAnimation({
        targets: prop("targets", "objectTarget", rows),
        n: {
            name: "n",
            value: {
                propType: "callback",
                paramCount: 2,
                value: {
                    callback: "InvokeAll",
                    dotNetRef: {
                        async invokeMethodAsync(name, total) {
                            assert.equal(name, "InvokeAll");
                            assert.equal(total, 3);
                            return [5, 15, 25];
                        }
                    }
                }
            }
        },
        delay: prop("delay", "stagger", {
            value: 10,
            options: {
                from: setter("from", "center"),
                grid: setter("grid", [3, 1])
            }
        }),
        duration: setter("duration", 20),
        autoplay: setter("autoplay", false),
        easing: setter("easing", "linear")
    });

    animation.seek(animation.getDuration());
    assert.deepEqual(rows.map((row) => row.target.n), [5, 15, 25]);
});

test("property keyframes unwrap nested setters", async () => {
    const api = loadAnime();
    const target = api.createObject({ x: 0 });
    const animation = await api.createAnimation({
        targets: prop("targets", "objectTarget", target),
        x: setter("x", [
            {
                value: setter("value", 40),
                duration: setter("duration", 100)
            },
            {
                value: setter("value", 10),
                duration: setter("duration", 100)
            }
        ]),
        easing: setter("easing", "linear"),
        autoplay: setter("autoplay", false)
    });

    animation.seek(100);
    assert.equal(target.target.x, 40);
    animation.seek(animation.getDuration());
    assert.equal(target.target.x, 10);
});

test("timeline offset waits until the previous child plus the gap", async () => {
    const api = loadAnime();
    const first = api.createObject({ v: 0 });
    const second = api.createObject({ v: 0 });
    const timeline = await api.createTimeline({
        autoplay: setter("autoplay", false),
        easing: setter("easing", "linear")
    });

    await timeline.add({
        targets: prop("targets", "objectTarget", first),
        v: setter("v", 1),
        duration: setter("duration", 100)
    });
    await timeline.add({
        targets: prop("targets", "objectTarget", second),
        v: setter("v", 1),
        duration: setter("duration", 100)
    }, "+=50");

    assert.equal(timeline.getDuration(), 250);
    timeline.seek(100);
    assert.equal(first.target.v, 1);
    assert.equal(second.target.v, 0);
    timeline.finish();
    assert.equal(second.target.v, 1);
});

test("speed and random use the anime.js helpers", () => {
    const api = loadAnime();
    api.setSpeed(2);
    assert.equal(api.getSpeed(), 2);
    assert.equal(api.random(4, 4), 4);
});
