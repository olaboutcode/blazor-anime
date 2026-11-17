// The scope and globals objects are the ones the engine reads while it creates instances.
// Entering a scope sets them, awaits .NET, then restores them. anime's own add() cannot
// wait for .NET, so media queries are listened to here instead of by createScope.
import { globals, scope as scopeState } from "./node_modules/animejs/dist/modules/core/globals.js";

export function installScope(anime) {
    function curveEasing(samples) {
        return (progress) => {
            const clamped = Math.min(1, Math.max(0, Number(progress) || 0));
            const scaled = clamped * (samples.length - 1);
            const index = Math.floor(scaled);
            const next = Math.min(index + 1, samples.length - 1);
            const fraction = scaled - index;
            return samples[index] + (samples[next] - samples[index]) * fraction;
        };
    }

    function resolveEase(payload) {
        if (payload == null || typeof payload === "string") return payload;
        if (payload.propType === "easeFn") payload = payload.value;
        if (!payload || typeof payload !== "object") return payload;
        if (payload.fn === "curve") return curveEasing(payload.value);
        if (payload.fn === "steps") return anime.steps(payload.steps);
        if (payload.fn === "cubicBezier") {
            return anime.cubicBezier(payload.x1, payload.y1, payload.x2, payload.y2);
        }
        if (payload.fn === "spring") {
            if (payload.bounce != null) {
                return anime.spring({ bounce: payload.bounce, duration: payload.duration });
            }
            if (payload.mass == null) return anime.spring({});
            return anime.spring({
                mass: payload.mass,
                stiffness: payload.stiffness,
                damping: payload.damping,
                velocity: payload.velocity
            });
        }
        return payload;
    }

    function resolveDefaults(defaults) {
        if (!defaults || typeof defaults !== "object") return null;
        const copy = { ...defaults };
        if (copy.ease != null) copy.ease = resolveEase(copy.ease);
        if (copy.playbackEase != null) copy.playbackEase = resolveEase(copy.playbackEase);
        return Object.keys(copy).length ? copy : null;
    }

    function match(query) {
        const fn = globalThis.matchMedia || globalThis.window?.matchMedia;
        if (typeof fn !== "function") throw new Error("matchMedia is not available.");
        return fn.call(globalThis.window || globalThis, query);
    }

    function syncMatches(record) {
        const matches = record.instance.matches;
        for (const name of Object.keys(matches)) delete matches[name];
        for (const [name, list] of Object.entries(record.queries)) matches[name] = !!list.matches;
        return { ...matches };
    }

    function enter(record) {
        const previous = {
            current: scopeState.current,
            root: scopeState.root,
            defaults: globals.defaults
        };
        const instance = record.instance;
        scopeState.current = instance;
        scopeState.root = instance.root;
        globals.defaults = instance.defaults;
        syncMatches(record);
        return previous;
    }

    function leave(previous) {
        scopeState.current = previous.current;
        scopeState.root = previous.root;
        globals.defaults = previous.defaults;
    }

    function enqueue(record, job) {
        const previous = record.tail || Promise.resolve();
        const current = previous.then(job, job);
        record.tail = current.then(() => undefined, () => undefined);
        return current.then(() => undefined);
    }

    async function runCleanups(list) {
        const items = list.splice(0);
        for (const item of items) {
            try {
                await item.invokeMethodAsync("Run", {});
            } catch {
                // The page may already be gone. Keep reverting the rest.
            } finally {
                item.dispose?.();
            }
        }
    }

    async function executeConstructor(record, dotnetRef, once) {
        const instance = record.instance;
        instance.once = !!once;
        const previous = enter(record);
        try {
            const cleanups = await dotnetRef.invokeMethodAsync("Run", syncMatches(record));
            const bucket = once ? record.onceCleanups : record.cleanups;
            if (Array.isArray(cleanups)) {
                for (const cleanup of cleanups) {
                    if (cleanup) bucket.push(cleanup);
                }
            }
        } finally {
            instance.once = false;
            leave(previous);
        }
    }

    async function refresh(record) {
        if (record.disposed) return;
        const revertibles = record.instance.revertibles;
        let index = revertibles.length;
        while (index--) revertibles[index].revert();
        revertibles.length = 0;
        await runCleanups(record.cleanups);
        for (const constructor of record.constructors) {
            await executeConstructor(record, constructor, false);
        }
    }

    async function revert(record) {
        if (record.disposed) return;
        record.disposed = true;
        for (const list of Object.values(record.queries)) {
            list.removeEventListener("change", record.onChange);
        }
        record.instance.revert();
        await runCleanups(record.cleanups);
        await runCleanups(record.onceCleanups);
        for (const constructor of record.constructors) constructor.dispose?.();
        for (const constructor of record.onceConstructors) constructor.dispose?.();
        for (const method of Object.values(record.methods)) method.dispose?.();
        record.constructors = [];
        record.onceConstructors = [];
        record.methods = {};
        record.queries = {};
        record.state = {};
    }

    function rootReference(root) {
        if (globalThis.DotNet?.createJSObjectReference) return globalThis.DotNet.createJSObjectReference(root);
        return root;
    }

    function create(root, defaults, mediaQueries) {
        const params = {};
        if (root) params.root = root;
        const resolved = resolveDefaults(defaults);
        if (resolved) params.defaults = resolved;
        const instance = anime.createScope(params);
        const record = {
            instance,
            constructors: [],
            onceConstructors: [],
            methods: {},
            cleanups: [],
            onceCleanups: [],
            queries: {},
            state: {},
            disposed: false,
            tail: null,
            onChange: null
        };
        record.onChange = () => {
            if (!record.disposed) enqueue(record, () => refresh(record));
        };
        if (mediaQueries && typeof mediaQueries === "object") {
            for (const [name, query] of Object.entries(mediaQueries)) {
                const list = match(String(query));
                record.queries[name] = list;
                list.addEventListener("change", record.onChange);
            }
        }
        syncMatches(record);
        return {
            root: () => rootReference(instance.root),
            matches: () => syncMatches(record),
            registered: () => instance.revertibles.length + instance.revertiblesOnce.length,
            stateGet: (key) => record.state[key] ?? 0,
            stateSet: (key, value) => {
                record.state[key] = value | 0;
            },
            add: (ref) => enqueue(record, async () => {
                if (record.disposed) return;
                record.constructors.push(ref);
                await executeConstructor(record, ref, false);
            }),
            addOnce: (ref) => enqueue(record, async () => {
                if (record.disposed) return;
                record.onceConstructors.push(ref);
                await executeConstructor(record, ref, true);
            }),
            addMethod: (name, ref) => {
                const previous = record.methods[name];
                if (previous && previous !== ref) previous.dispose?.();
                record.methods[name] = ref;
            },
            invoke: (name, call) => enqueue(record, async () => {
                if (record.disposed) return;
                const method = record.methods[name];
                if (!method) return;
                const previous = enter(record);
                try {
                    await method.invokeMethodAsync("Run", call || {});
                } finally {
                    leave(previous);
                }
            }),
            refresh: () => enqueue(record, () => refresh(record)),
            revert: () => enqueue(record, () => revert(record))
        };
    }

    globalThis.AnimeJsScope = { create };
}
