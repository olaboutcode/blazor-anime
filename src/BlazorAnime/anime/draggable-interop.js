// Facade returned to Blazor. A draggable is thenable through its timers; a plain object is not.
export function installDraggable(anime) {
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
        if (value.propType === "modifier") return resolveModifier(value);
        if (value.propType === "callback") return callback(value);
        if (value.propType === "objectTarget") return unwrapObjectTarget(value);
        if (value.value && typeof value.value === "object" && value.value.propType) return unwrap(value.value);
        // Leave data objects in place unless a nested marker has to be replaced.
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

    function resolveModifier(marker) {
        if (marker.fn === "snap") return anime.snap(marker.increment);
        if (marker.fn === "invert") return (amount) => -amount;
        return anime.round(marker.decimals);
    }

    function number(value) {
        const parsed = Number(value);
        return Number.isFinite(parsed) ? parsed : 0;
    }

    function snapshot(drag) {
        return {
            x: number(drag.x),
            y: number(drag.y),
            progressX: number(drag.progressX),
            progressY: number(drag.progressY),
            velocity: number(drag.velocity),
            angle: number(drag.angle),
            grabbed: !!drag.grabbed,
            dragged: !!drag.dragged,
            enabled: !!drag.enabled
        };
    }

    function callback(marker) {
        const dotNetRef = marker.dotNetRef;
        return (drag) => {
            dotNetRef.invokeMethodAsync("Invoke", snapshot(drag)).catch(() => {});
        };
    }

    function unwrapObjectTarget(marker) {
        const payload = marker.value;
        if (Array.isArray(payload)) return payload.map((item) => item.target);
        return payload.target;
    }

    // Blazor JSON-encodes a method result unless the value is already a JS object reference.
    function publish(value) {
        if (value == null) return null;
        const create = globalThis.DotNet && globalThis.DotNet.createJSObjectReference;
        return typeof create === "function" ? create(value) : value;
    }

    // The returned object must not be the draggable. Its timers are thenable, and Blazor awaits every result.
    function run(action) {
        return (...args) => {
            action(...args);
        };
    }

    function optional(value) {
        return value == null ? undefined : value;
    }

    function facade(instance) {
        return {
            disable: run(() => instance.disable()),
            enable: run(() => instance.enable()),
            setX: run((x, mute) => instance.setX(x, !!mute)),
            setY: run((y, mute) => instance.setY(y, !!mute)),
            stop: run(() => instance.stop()),
            reset: run(() => instance.reset()),
            refresh: run(() => instance.refresh()),
            revert: run(() => instance.revert()),
            scrollInView: run((duration, gap, ease) => instance.scrollInView(
                optional(duration),
                optional(gap),
                ease == null ? undefined : resolveEase(ease))),
            animateInView: run((duration, gap, ease) => instance.animateInView(
                optional(duration),
                optional(gap),
                ease == null ? undefined : resolveEase(ease))),
            x: () => number(instance.x),
            y: () => number(instance.y),
            progressX: () => number(instance.progressX),
            progressY: () => number(instance.progressY),
            velocity: () => number(instance.velocity),
            angle: () => number(instance.angle),
            destX: () => number(instance.destX),
            destY: () => number(instance.destY),
            deltaX: () => number(instance.deltaX),
            deltaY: () => number(instance.deltaY),
            enabled: () => !!instance.enabled,
            grabbed: () => !!instance.grabbed,
            dragged: () => !!instance.dragged,
            contained: () => !!instance.contained,
            xProp: () => String(instance.xProp ?? ""),
            yProp: () => String(instance.yProp ?? ""),
            target: () => publish(instance.$target),
            trigger: () => publish(instance.$trigger),
            container: () => publish(instance.$container),
            scrollContainer: () => publish(instance.$scrollContainer)
        };
    }

    globalThis.AnimeJsDraggable = {
        create(targets, parameters) {
            return facade(anime.createDraggable(unwrap(targets), unwrap(parameters || {})));
        }
    };
}
