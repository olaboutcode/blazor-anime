// Facade returned to Blazor. animate() returns a thenable Timeline, and Blazor awaits every result.
export function installLayout(anime) {
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

    function normalizeLoop(value) {
        if (value === true || value === Infinity || (typeof value === "number" && value < 0)) return -1;
        if (value === false || value == null) return 0;
        return value;
    }

    function toState(timeline) {
        const storedLoop = timeline.__blazorLoop;
        const loop = storedLoop === undefined
            ? (timeline.iterationCount === Infinity ? -1 : (timeline.iterationCount ?? 1) - 1)
            : normalizeLoop(storedLoop);
        const alternate = timeline.__blazorAlternate !== undefined
            ? !!timeline.__blazorAlternate
            : !!timeline._alternate;
        return {
            id: String(timeline.id ?? ""),
            progress: timeline.progress,
            began: !!timeline.began,
            completed: !!timeline.completed,
            paused: !!timeline.paused,
            reversed: !!timeline.reversed,
            backwards: !!timeline.backwards,
            alternate,
            duration: timeline.duration,
            delay: timeline._delay ?? 0,
            loopDelay: timeline._loopDelay ?? 0,
            currentTime: timeline.currentTime,
            iterationProgress: timeline.iterationProgress ?? 0,
            currentIteration: timeline.currentIteration ?? 0,
            loop
        };
    }

    function isDomNode(value) {
        return typeof Node !== "undefined" && value instanceof Node;
    }

    function unwrap(value) {
        if (value == null) return value;
        if (typeof value !== "object") return value;
        if (isDomNode(value)) return value;
        if (Array.isArray(value)) {
            let copy = null;
            for (let index = 0; index < value.length; index++) {
                const next = unwrap(value[index]);
                if (next === value[index]) continue;
                copy ??= value.slice();
                copy[index] = next;
            }
            return copy ?? value;
        }
        if (value.propType === "easeFn") return resolveEase(value.value);
        if (value.propType === "setter") return unwrap(value.value);
        if (value.propType === "stagger") return unwrapStagger(value);
        if (value.propType === "callback") return callback(value);
        if (value.value && typeof value.value === "object" && value.value.propType) return unwrap(value.value);
        let clone = null;
        for (const key of Object.keys(value)) {
            const current = value[key];
            const next = unwrap(current);
            if (next === current) continue;
            clone ??= { ...value };
            clone[key] = next;
        }
        return clone ?? value;
    }

    function unwrapStagger(marker) {
        const options = {};
        const raw = marker.options || {};
        for (const key of Object.keys(raw)) options[key] = unwrap(raw[key]);
        return Object.keys(options).length
            ? anime.stagger(marker.value, options)
            : anime.stagger(marker.value);
    }

    function callback(marker) {
        const dotNetRef = marker.dotNetRef;
        return (timeline) => {
            dotNetRef.invokeMethodAsync("Invoke", toState(timeline)).catch(() => {});
        };
    }

    function publish(value) {
        if (value == null) return null;
        const create = globalThis.DotNet && globalThis.DotNet.createJSObjectReference;
        return typeof create === "function" ? create(value) : value;
    }

    function publishList(values) {
        if (!values || typeof values.length !== "number") return [];
        const published = [];
        for (let index = 0; index < values.length; index++) {
            const item = publish(values[index]);
            if (item != null) published.push(item);
        }
        return published;
    }

    // The returned function must not return the timeline. A running timeline never finishes the Blazor await.
    function run(action) {
        return (...args) => {
            action(...args);
        };
    }

    function facade(instance) {
        return {
            record: run(() => instance.record()),
            animate: run((parameters) => instance.animate(unwrap(parameters || {}))),
            revert: run(() => instance.revert()),
            root: () => publish(instance.root),
            id: () => String(instance.id ?? ""),
            entering: () => publishList(instance.entering),
            leaving: () => publishList(instance.leaving),
            swapping: () => publishList(instance.swapping),
            animating: () => publishList(instance.animating)
        };
    }

    globalThis.AnimeJsLayout = {
        create(root, parameters) {
            return facade(anime.createLayout(unwrap(root), unwrap(parameters || {})));
        }
    };
}
