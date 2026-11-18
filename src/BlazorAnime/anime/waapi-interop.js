// Facade returned to Blazor. WAAPIAnimation is thenable, and Blazor awaits every
// result, so playback methods return nothing and handle() is not the animation.
export function installWaapi(anime) {
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
        if (payload == null || typeof payload === "string" || typeof payload === "number" || typeof payload === "boolean") {
            return payload;
        }
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

    function unwrapStagger(marker) {
        const options = {};
        const raw = marker.options || {};
        for (const key of Object.keys(raw)) options[key] = resolveValue(raw[key]);
        return Object.keys(options).length
            ? anime.stagger(marker.value, options)
            : anime.stagger(marker.value);
    }

    function resolveValue(value) {
        if (value == null || typeof value === "function" || typeof value !== "object") return value;
        if (typeof value.nodeType === "number") return value;
        if (typeof value.link === "function" || typeof value.invokeMethodAsync === "function") return value;
        if (value.propType === "easeFn") return resolveEase(value);
        if (value.propType === "stagger") return unwrapStagger(value);
        if (value.propType === "setter") return resolveValue(value.value);
        if (value.value && typeof value.value === "object" && value.value.propType) return resolveValue(value.value);
        if (Array.isArray(value)) return value.map(resolveValue);
        const copy = {};
        let changed = false;
        for (const key of Object.keys(value)) {
            const next = resolveValue(value[key]);
            copy[key] = next;
            if (next !== value[key]) changed = true;
        }
        return changed ? copy : value;
    }

    function isDomNode(value) {
        return typeof Node !== "undefined" && value instanceof Node;
    }

    function collectTargets(targets) {
        if (targets == null) return [];
        if (typeof targets === "string") {
            try {
                return Array.from(document.querySelectorAll(targets));
            } catch {
                return [];
            }
        }
        if (isDomNode(targets)) return [targets];
        if (Array.isArray(targets)) {
            const result = [];
            for (const item of targets) result.push(...collectTargets(item));
            return result;
        }
        return [targets];
    }

    function describeTarget(element, index, total) {
        const dataset = {};
        const source = element && element.dataset;
        if (source) {
            for (const key of Object.keys(source)) {
                const value = source[key];
                dataset[key] = value == null ? "" : String(value);
            }
        }
        const tag = element && element.tagName ? String(element.tagName) : "";
        return {
            index,
            total,
            id: element && element.id ? String(element.id) : "",
            tagName: tag.toLowerCase(),
            dataset
        };
    }

    async function callbackFunction(marker, targets) {
        const total = targets.length;
        if (total === 0) return () => undefined;
        const argument = marker.paramCount >= 3
            ? targets.map((element, index) => describeTarget(element, index, total))
            : total;
        const values = await marker.dotNetRef.invokeMethodAsync(marker.callback, argument);
        return (_element, index) => values[index];
    }

    async function materializeValue(value, targets) {
        if (Array.isArray(value)) {
            const next = [];
            for (const item of value) next.push(await materializeValue(item, targets));
            return next;
        }
        if (!value || typeof value !== "object" || typeof value.nodeType === "number") return value;
        if (value.propType === "callback" && value.dotNetRef) return callbackFunction(value, targets);
        if (value.propType === "easeFn" || value.propType === "stagger") return value;
        if (typeof value.link === "function" || typeof value.invokeMethodAsync === "function") return value;
        const copy = {};
        for (const key of Object.keys(value)) copy[key] = await materializeValue(value[key], targets);
        return copy;
    }

    function finite(value) {
        const number = Number(value);
        return Number.isFinite(number) ? number : 0;
    }

    function snapshot(animation) {
        return {
            currentTime: finite(animation.currentTime),
            progress: finite(animation.progress),
            duration: finite(animation.duration),
            completed: !!animation.completed,
            paused: !!animation.paused,
            reversed: !!animation.reversed
        };
    }

    function run(action) {
        return (...args) => {
            action(...args);
        };
    }

    function publish(value) {
        const create = globalThis.DotNet && globalThis.DotNet.createJSObjectReference;
        return typeof create === "function" ? create(value) : value;
    }

    // timeline.sync reads pause, duration, persist, and currentTime.
    // The raw animation is thenable, so this wrapper is what Blazor holds.
    function linkHandle(instance) {
        return {
            pause() {
                instance.pause();
            },
            get duration() {
                return instance.duration;
            },
            get persist() {
                return instance.persist;
            },
            set persist(value) {
                instance.persist = value;
            },
            get currentTime() {
                return instance.currentTime;
            },
            set currentTime(value) {
                instance.currentTime = value;
            }
        };
    }

    function facade(instance) {
        return {
            handle: () => linkHandle(instance),
            play: run(() => instance.play()),
            pause: run(() => instance.pause()),
            resume: run(() => instance.resume()),
            reverse: run(() => instance.reverse()),
            alternate: run(() => instance.alternate()),
            restart: run(() => instance.restart()),
            seek: run((time) => instance.seek(time)),
            complete: run(() => instance.complete()),
            cancel: run(() => instance.cancel()),
            revert: run(() => instance.revert()),
            commitStyles: run(() => instance.commitStyles()),
            whenFinished: () => instance.then(),
            duration: () => finite(instance.duration),
            currentTime: () => finite(instance.currentTime),
            progress: () => finite(instance.progress),
            speed: () => finite(instance.speed),
            setSpeed: run((speed) => {
                instance.speed = Number(speed);
            }),
            paused: () => !!instance.paused,
            completed: () => !!instance.completed,
            reversed: () => !!instance.reversed,
            persist: () => !!instance.persist,
            targets: () => (instance.targets || []).map(publish)
        };
    }

    function easingFunction(payload) {
        const resolved = resolveEase(payload);
        if (typeof resolved === "function") return resolved;
        if (resolved && typeof resolved.ease === "function") return resolved.ease;
        throw new Error("ConvertEase needs a JavaScript easing function.");
    }

    globalThis.AnimeJsWaapi = {
        create: async (targets, settings) => {
            const params = await materializeValue({ ...(settings || {}) }, collectTargets(targets));
            const completed = params.onComplete;
            if (completed && typeof completed.invokeMethodAsync === "function") {
                params.onComplete = (animation) => {
                    completed.invokeMethodAsync("Run", snapshot(animation)).catch(() => {});
                };
            }
            for (const key of Object.keys(params)) params[key] = resolveValue(params[key]);
            return facade(anime.waapi.animate(targets, params));
        },
        convertEase: (payload) => anime.waapi.convertEase(easingFunction(payload))
    };
}
