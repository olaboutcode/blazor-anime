// Facade returned to Blazor. anime.js instances are thenable; a plain object is not.
export function installAnimatable(anime) {
    function normalizeLoop(value) {
        if (value === true || value === Infinity) return -1;
        if (typeof value === "number" && value < 0) return -1;
        if (value === false || value == null) return 0;
        return value;
    }

    function toState(anim) {
        const storedLoop = anim.__blazorLoop;
        const loop = storedLoop === undefined
            ? (anim.iterationCount === Infinity ? -1 : (anim.iterationCount ?? 1) - 1)
            : normalizeLoop(storedLoop);
        return {
            id: String(anim.id ?? ""),
            progress: anim.progress,
            began: !!anim.began,
            completed: !!anim.completed,
            paused: !!anim.paused,
            reversed: !!anim.reversed,
            backwards: !!anim.backwards,
            alternate: !!anim.__blazorAlternate,
            duration: anim.duration,
            delay: anim._delay ?? 0,
            loopDelay: anim._loopDelay ?? 0,
            currentTime: anim.currentTime,
            iterationProgress: anim.iterationProgress ?? 0,
            currentIteration: anim.currentIteration ?? 0,
            loop
        };
    }

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

    function isDomNode(value) {
        return typeof Node !== "undefined" && value instanceof Node;
    }

    function unwrap(value) {
        if (value == null) return value;
        if (typeof value !== "object") return value;
        if (isDomNode(value)) return value;
        if (Array.isArray(value)) return value.map(unwrap);
        if (value.propType === "easeFn") return resolveEase(value.value);
        if (value.propType === "modifier") return anime.round(value.decimals);
        if (value.propType === "setter") return unwrap(value.value);
        if (value.propType === "stagger") return unwrapStagger(value);
        if (value.propType === "callback") return callback(value);
        if (value.propType === "objectTarget") return unwrapObjectTarget(value);
        if (value.value && typeof value.value === "object" && value.value.propType) return unwrap(value.value);
        const out = {};
        for (const key of Object.keys(value)) out[key] = unwrap(value[key]);
        return out;
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
        return (anim) => {
            dotNetRef.invokeMethodAsync("Invoke", toState(anim)).catch(() => {});
        };
    }

    function unwrapObjectTarget(marker) {
        const payload = marker.value;
        if (Array.isArray(payload)) return payload.map((item) => item.target);
        return payload.target;
    }

    function requireFn(instance, name) {
        const fn = instance[name];
        if (typeof fn !== "function") {
            throw new Error("Animatable has no property '" + name + "'.");
        }
        return fn;
    }

    function facade(instance) {
        return {
            get(name) {
                const value = requireFn(instance, name)();
                if (value == null) {
                    throw new Error("Animatable property '" + name + "' has no value.");
                }
                if (Array.isArray(value)) {
                    return { many: true, values: value.map((item) => Number(item)) };
                }
                return { many: false, values: [Number(value)] };
            },
            set(name, value, duration, ease) {
                const fn = requireFn(instance, name);
                fn(
                    value,
                    duration == null ? undefined : duration,
                    ease == null ? undefined : resolveEase(ease));
            },
            revert() {
                instance.revert();
            },
            pause() {
                for (const name in instance.animations) instance.animations[name].pause();
                if (instance.callbacks) instance.callbacks.pause();
            }
        };
    }

    globalThis.AnimeJsAnimatable = {
        create(targets, parameters) {
            return facade(anime.createAnimatable(unwrap(targets), unwrap(parameters || {})));
        }
    };
}
