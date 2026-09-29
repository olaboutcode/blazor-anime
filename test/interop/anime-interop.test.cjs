const assert = require("node:assert/strict");
const path = require("node:path");
const test = require("node:test");
const { pathToFileURL } = require("node:url");

const moduleUrl = pathToFileURL(path.join(
    __dirname,
    "../../src/BlazorAnime/wwwroot/BlazorAnime.lib.module.js")).href;

async function loadAnime() {
    if (!globalThis.__blazorAnimeLoaded) {
        function NodeList() {}
        function HTMLCollection() {}
        function Node() {}
        function Element() {}
        function SVGElement() {}
        function HTMLInputElement() {}

        Object.assign(globalThis, {
            NodeList,
            HTMLCollection,
            Node,
            Element,
            SVGElement,
            HTMLInputElement,
            window: globalThis,
            Date,
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
        await import(moduleUrl);
        globalThis.__blazorAnimeLoaded = true;
    }

    const api = globalThis.AnimeJs;
    api.setSpeed(1);
    return api;
}

function prop(name, propType, value) {
    return { name, value: { propType, value } };
}

function setter(name, value) {
    return prop(name, "setter", value);
}

function easeFn(name, payload) {
    return { name, value: { propType: "easeFn", value: payload } };
}

test("object target seeks to the requested number", async () => {
    const api = await loadAnime();
    assert.equal(api.version(), "4.5.0");

    const target = api.createObject({ x: 0 });
    const animation = await api.createAnimation({
        targets: prop("targets", "objectTarget", target),
        x: setter("x", 100),
        duration: setter("duration", 100),
        autoplay: setter("autoplay", false),
        ease: setter("ease", "linear")
    });

    animation.seek(100);
    assert.equal(target.target.x, 100);
    assert.equal(animation.getProgress(), 1);
    assert.equal(animation.hasCompleted(), 1);
    assert.equal(animation.getCurrentValues().x, "100");
});

test("function values are collected once per target", async () => {
    const api = await loadAnime();
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
        ease: setter("ease", "linear")
    });

    animation.seek(animation.getDuration());
    assert.deepEqual(rows.map((row) => row.target.n), [5, 15, 25]);
});

test("property keyframes unwrap nested setters", async () => {
    const api = await loadAnime();
    const target = api.createObject({ x: 0 });
    const animation = await api.createAnimation({
        targets: prop("targets", "objectTarget", target),
        x: setter("x", [
            {
                to: setter("to", 40),
                duration: setter("duration", 100)
            },
            {
                to: setter("to", 10),
                duration: setter("duration", 100)
            }
        ]),
        ease: setter("ease", "linear"),
        autoplay: setter("autoplay", false)
    });

    animation.seek(100);
    assert.equal(target.target.x, 40);
    animation.seek(animation.getDuration());
    assert.equal(target.target.x, 10);
});

test("a sampled curve eases from the table instead of a named function", async () => {
    const api = await loadAnime();
    const target = api.createObject({ x: 0 });
    const animation = await api.createAnimation({
        targets: prop("targets", "objectTarget", target),
        x: setter("x", 100),
        duration: setter("duration", 100),
        autoplay: setter("autoplay", false),
        ease: easeFn("ease", { fn: "curve", value: [0, 0, 1] })
    });

    animation.seek(50);
    assert.ok(Math.abs(target.target.x) < 0.001, `midpoint was ${target.target.x}`);
    animation.seek(100);
    assert.equal(target.target.x, 100);
});

test("stagger accepts a sampled curve as its easing", async () => {
    const api = await loadAnime();
    const rows = [
        api.createObject({ n: 0 }),
        api.createObject({ n: 0 }),
        api.createObject({ n: 0 })
    ];
    const animation = await api.createAnimation({
        targets: prop("targets", "objectTarget", rows),
        n: setter("n", 1),
        duration: setter("duration", 10),
        autoplay: setter("autoplay", false),
        ease: setter("ease", "linear"),
        delay: prop("delay", "stagger", {
            value: 100,
            options: {
                ease: easeFn("ease", { fn: "curve", value: [0, 0, 1] })
            }
        })
    });

    animation.seek(50);
    assert.ok(Math.abs(rows[1].target.n - 1) < 0.001, `middle target was ${rows[1].target.n}`);
    assert.equal(rows[2].target.n, 0);
    animation.seek(animation.getDuration());
    assert.deepEqual(rows.map((row) => row.target.n), [1, 1, 1]);
});

test("target callbacks receive a snapshot of each target", async () => {
    const api = await loadAnime();
    const rows = [
        api.createObject({ n: 0, id: "a", tagName: "DIV", dataset: { x: "4" } }),
        api.createObject({ n: 0, id: "b", tagName: "SPAN", dataset: { x: "9" } })
    ];
    let seen = [];
    const animation = await api.createAnimation({
        targets: prop("targets", "objectTarget", rows),
        n: {
            name: "n",
            value: {
                propType: "callback",
                paramCount: 3,
                value: {
                    callback: "InvokeTargets",
                    dotNetRef: {
                        async invokeMethodAsync(name, infos) {
                            assert.equal(name, "InvokeTargets");
                            seen = infos;
                            return infos.map((info) => Number(info.dataset.x));
                        }
                    }
                }
            }
        },
        duration: setter("duration", 10),
        autoplay: setter("autoplay", false),
        ease: setter("ease", "linear")
    });

    animation.seek(10);
    assert.deepEqual(rows.map((row) => row.target.n), [4, 9]);
    assert.equal(seen[0].id, "a");
    assert.equal(seen[0].tagName, "div");
    assert.equal(seen[0].dataset.x, "4");
    assert.equal(seen[1].index, 1);
    assert.equal(seen[1].total, 2);
});

test("speed and random use the anime.js helpers", async () => {
    const api = await loadAnime();
    api.setSpeed(2);
    assert.equal(api.getSpeed(), 2);
    assert.equal(api.random(4, 4), 4);
    api.setSpeed(1);
});
