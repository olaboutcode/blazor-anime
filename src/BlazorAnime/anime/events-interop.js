// onScroll returns a ScrollObserver. Autoplay links that object, so handle()
// returns it raw. The other methods return nothing: Blazor awaits every result,
// and refresh() and revert() return the observer.
const callbackNames = [
    "onEnter",
    "onEnterForward",
    "onEnterBackward",
    "onLeave",
    "onLeaveForward",
    "onLeaveBackward",
    "onUpdate",
    "onSyncComplete",
    "onResize"
];

export function installEvents(anime) {
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

    function snapshot(observer) {
        return {
            progress: observer.progress,
            isInView: !!observer.isInView,
            began: !!observer.began,
            completed: !!observer.completed,
            backward: !!observer.backward,
            velocity: observer.velocity,
            scroll: observer.scroll
        };
    }

    function create(container, target, settings) {
        const params = { ...(settings || {}) };
        if (container) params.container = container;
        if (target) params.target = target;
        if (params.sync != null) params.sync = resolveEase(params.sync);
        const holder = { current: null };
        for (const name of callbackNames) {
            const ref = params[name];
            if (!ref || typeof ref.invokeMethodAsync !== "function") continue;
            params[name] = () => {
                const observer = holder.current;
                if (!observer) return;
                ref.invokeMethodAsync("Run", snapshot(observer)).catch(() => {});
            };
        }
        const observer = anime.onScroll(params);
        holder.current = observer;
        return {
            handle: () => observer,
            link: (tickable) => {
                observer.link(tickable);
            },
            refresh: () => {
                observer.refresh();
            },
            revert: () => {
                observer.revert();
            },
            progress: () => observer.progress,
            isInView: () => !!observer.isInView,
            repeat: () => !!observer.repeat,
            horizontal: () => !!observer.horizontal
        };
    }

    globalThis.AnimeJsEvents = { create };
}
