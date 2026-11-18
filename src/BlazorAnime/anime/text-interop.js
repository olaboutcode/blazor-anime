// splitText and scrambleText. refresh() and revert() return the splitter.
// Drop that return so Blazor does not await it. Element handles are { target }
// so core Targets can unwrap them.
export function installText(anime) {
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

    function publish(element) {
        const handle = { target: element };
        const create = globalThis.DotNet && globalThis.DotNet.createJSObjectReference;
        return typeof create === "function" ? create(handle) : handle;
    }

    function readList(list) {
        return (list || []).map(publish);
    }

    function facade(split) {
        return {
            lines: () => readList(split.lines),
            words: () => readList(split.words),
            chars: () => readList(split.chars),
            refresh: () => {
                split.refresh();
            },
            revert: () => {
                split.revert();
            }
        };
    }

    function targetOf(target) {
        if (typeof target === "string") {
            const found = document.querySelector(target);
            if (!found) throw new Error("splitText did not find a target.");
            return found;
        }
        if (target && target.nodeType === 1) return target;
        throw new Error("splitText did not find a target.");
    }

    function copySettings(input) {
        const settings = {};
        if (!input || typeof input !== "object") return settings;
        for (const key of ["lines", "words", "chars", "accessible", "includeSpaces", "debug"]) {
            if (Object.prototype.hasOwnProperty.call(input, key)) settings[key] = input[key];
        }
        return settings;
    }

    globalThis.AnimeJsText = {
        create: async (target, settings) => {
            const element = targetOf(target);
            const split = anime.splitText(element, copySettings(settings));
            if (!split.$target) throw new Error("splitText did not find a target.");
            if (split.lineTemplate && document.fonts && document.fonts.ready) {
                await document.fonts.ready;
            }
            return facade(split);
        },
        scramble: (params) => {
            const options = { ...(params || {}) };
            if (Object.prototype.hasOwnProperty.call(options, "ease")) {
                options.ease = resolveEase(options.ease);
            }
            const changed = options.onChange;
            if (changed && typeof changed.invokeMethodAsync === "function") {
                options.onChange = (text, progress) => {
                    changed.invokeMethodAsync("Run", String(text ?? ""), Number(progress) || 0).catch(() => {});
                };
            }
            return anime.scrambleText(options);
        }
    };
}
