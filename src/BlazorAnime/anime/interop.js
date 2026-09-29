const VALUE_FN = "__blazorAnimeValueFn";

const playbackNames = new Set([
    "autoplay",
    "loop",
    "alternate",
    "reversed",
    "onBegin",
    "onUpdate",
    "onRender",
    "onLoop",
    "onComplete",
    "onPause",
    "onBeforeUpdate",
    "playbackRate",
    "frameRate",
    "playbackEase"
]);

const reservedNames = new Set([
    ...playbackNames,
    "targets",
    "duration",
    "delay",
    "ease",
    "loopDelay",
    "modifier",
    "composition",
    "id",
    "keyframes"
]);

export function install(anime) {
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

    function isDomNode(value) {
        return typeof Node !== "undefined" && value instanceof Node;
    }

    function isPlain(value) {
        if (!value || typeof value !== "object") return false;
        const proto = Object.getPrototypeOf(value);
        return proto === Object.prototype || proto === null;
    }

    function isMarker(value) {
        return isPlain(value) && value[VALUE_FN] === true;
    }

    function resolveTargets(targets) {
        if (targets == null) return [];
        if (typeof targets === "string") {
            try {
                return Array.from(document.querySelectorAll(targets));
            } catch {
                return [];
            }
        }
        if (isDomNode(targets)) return [targets];
        if (typeof NodeList !== "undefined" && (targets instanceof NodeList || targets instanceof HTMLCollection)) {
            return Array.from(targets);
        }
        if (Array.isArray(targets)) {
            const result = [];
            const seen = new Set();
            for (const item of targets) {
                for (const resolved of resolveTargets(item)) {
                    if (resolved && typeof resolved === "object") {
                        if (seen.has(resolved)) continue;
                        seen.add(resolved);
                    }
                    result.push(resolved);
                }
            }
            return result;
        }
        return [targets];
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
        if (!payload || typeof payload !== "object") return payload;
        if (payload.fn === "curve") return curveEasing(payload.value);
        if (payload.fn === "steps") return anime.steps(payload.steps);
        if (payload.fn === "cubicBezier") {
            return anime.cubicBezier(payload.x1, payload.y1, payload.x2, payload.y2);
        }
        if (payload.fn === "round") return anime.round(payload.decimals);
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

    function isWrappedProp(value) {
        return !!value && typeof value === "object" && !!value.value && !!value.value.propType;
    }

    function isPropBag(value) {
        if (!isPlain(value) || isWrappedProp(value)) return false;
        const keys = Object.keys(value);
        return keys.length > 0 && keys.every((key) => isWrappedProp(value[key]));
    }

    function isPercentageMap(value) {
        if (!isPlain(value)) return false;
        const keys = Object.keys(value);
        return keys.length > 0 && keys.every((key) => key.endsWith("%") && isPropBag(value[key]));
    }

    function transformSetter(setterValue) {
        if (Array.isArray(setterValue)) {
            return setterValue.map((item) => {
                if (item && typeof item === "object" && !isDomNode(item) && !isMarker(item)) {
                    return transformProps(item);
                }
                return item;
            });
        }
        if (isPercentageMap(setterValue)) {
            const frames = {};
            for (const key of Object.keys(setterValue)) {
                frames[key] = transformProps(setterValue[key]);
            }
            return frames;
        }
        if (setterValue && typeof setterValue === "object" && !isDomNode(setterValue)) {
            return transformProps(setterValue);
        }
        return setterValue;
    }

    function transformProps(props) {
        if (!props || typeof props !== "object") return props;
        const finalProps = {};
        for (const key in props) {
            if (!Object.prototype.hasOwnProperty.call(props, key)) continue;
            const prop = props[key];
            if (!prop || !prop.value) continue;
            const name = prop.name;
            const propType = prop.value.propType;
            const payload = prop.value.value;

            if (propType === "setter") {
                finalProps[name] = transformSetter(payload);
            } else if (propType === "svgSetter") {
                finalProps[name] = payload;
            } else if (propType === "objectTarget") {
                finalProps[name] = Array.isArray(payload)
                    ? payload.map((item) => item.target)
                    : payload.target;
            } else if (propType === "stagger") {
                const value = Array.isArray(payload.value)
                    ? [payload.value[0], payload.value[1]]
                    : payload.value;
                const options = payload.options ? transformProps(payload.options) : {};
                finalProps[name] = Object.keys(options).length === 0
                    ? anime.stagger(value)
                    : anime.stagger(value, options);
            } else if (propType === "easeFn") {
                finalProps[name] = resolveEase(payload);
            } else if (propType === "callback") {
                const dotNetRef = payload && payload.dotNetRef;
                const callbackName = payload && payload.callback;
                const paramCount = prop.value.paramCount;
                if (!callbackName || !dotNetRef) continue;
                if (paramCount >= 2) {
                    finalProps[name] = {
                        [VALUE_FN]: true,
                        dotNetRef,
                        callbackName,
                        withTarget: paramCount >= 3
                    };
                } else {
                    finalProps[name] = (anim) => {
                        dotNetRef.invokeMethodAsync(callbackName, toState(anim)).catch(() => {});
                    };
                }
            }
        }
        return finalProps;
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

    async function makeValueFn(marker, targets, slots) {
        const total = targets.length;
        if (total === 0) return () => undefined;
        const argument = marker.withTarget
            ? targets.map((element, index) => describeTarget(element, index, total))
            : total;
        const slot = {
            marker,
            targets,
            values: await marker.dotNetRef.invokeMethodAsync(marker.callbackName, argument)
        };
        slots.push(slot);
        return (_element, index) => slot.values[index];
    }

    async function materialize(node, targets, slots) {
        if (Array.isArray(node)) {
            for (let i = 0; i < node.length; i++) {
                if (isMarker(node[i])) node[i] = await makeValueFn(node[i], targets, slots);
                else await materialize(node[i], targets, slots);
            }
            return;
        }
        if (!isPlain(node) || isDomNode(node)) return;
        for (const key of Object.keys(node)) {
            const value = node[key];
            if (isMarker(value)) node[key] = await makeValueFn(value, targets, slots);
            else await materialize(value, targets, slots);
        }
    }

    async function transformPropsAsync(props, slots) {
        const transformed = transformProps(props);
        await materialize(transformed, resolveTargets(transformed.targets), slots);
        return transformed;
    }

    async function refreshValues(slots) {
        for (const slot of slots) {
            const total = slot.targets.length;
            const argument = slot.marker.withTarget
                ? slot.targets.map((element, index) => describeTarget(element, index, total))
                : total;
            slot.values = await slot.marker.dotNetRef.invokeMethodAsync(slot.marker.callbackName, argument);
        }
    }

    function animatedNames(params) {
        return Object.keys(params).filter((name) => !reservedNames.has(name));
    }

    function readCurrent(target, name) {
        if (target && typeof target.nodeType === "number") return anime.get(target, name);
        if (target && Object.prototype.hasOwnProperty.call(target, name)) return target[name];
        return undefined;
    }

    function attachInstanceApi(instance, params, extra, slots) {
        const names = animatedNames(params);
        const alternate = !!params.alternate;
        const loop = params.loop;
        const valueSlots = slots || [];
        return {
            play: () => instance.play(),
            pause: () => instance.pause(),
            restart: () => instance.restart(),
            reverse: () => instance.reverse(),
            resume: () => instance.resume(),
            reset: () => instance.reset(),
            seek: (time) => instance.seek(time),
            complete: () => instance.complete(),
            cancel: () => instance.cancel(),
            revert: () => instance.revert(),
            alternate: () => instance.alternate(),
            stretch: (duration) => instance.stretch(duration),
            refresh: async () => {
                await refreshValues(valueSlots);
                instance.refresh();
            },
            remove: (targets) => anime.remove(targets, instance),
            getProgress: () => instance.progress,
            setProgress: (progress) => {
                const clamped = Math.min(1, Math.max(0, Number(progress) || 0));
                instance.seek(clamped * (instance.duration || 0));
            },
            hasBegun: () => instance.began ? 1 : 0,
            hasCompleted: () => instance.completed ? 1 : 0,
            isPaused: () => instance.paused ? 1 : 0,
            isReversed: () => instance.reversed ? 1 : 0,
            isBackwards: () => instance.backwards ? 1 : 0,
            isAlternate: () => alternate ? 1 : 0,
            getId: () => String(instance.id ?? ""),
            getLoop: () => normalizeLoop(loop),
            getDuration: () => instance.duration,
            getDelay: () => instance._delay ?? 0,
            getLoopDelay: () => instance._loopDelay ?? 0,
            getCurrentTime: () => instance.currentTime,
            getIterationProgress: () => instance.iterationProgress ?? 0,
            getCurrentIteration: () => instance.currentIteration ?? 0,
            getTargetCount: () => (instance.targets || []).length,
            getCurrentValues: () => {
                const values = {};
                const target = (instance.targets || [])[0];
                for (const name of names) {
                    const current = readCurrent(target, name);
                    values[name] = current == null ? "" : String(current);
                }
                return values;
            },
            whenFinished: () => instance.then(),
            ...extra
        };
    }

    globalThis.AnimeJs = {
        createAnimation: async (props) => {
            const slots = [];
            const params = await transformPropsAsync(props, slots);
            const targets = params.targets;
            delete params.targets;
            const animation = anime.animate(targets, params);
            return attachInstanceApi(animation, params, null, slots);
        },
        createTimeline: async (props) => {
            const slots = [];
            const params = await transformPropsAsync(props, slots);
            const playback = {};
            const defaults = {};
            for (const key of Object.keys(params)) {
                if (playbackNames.has(key)) playback[key] = params[key];
                else if (key === "composition" && typeof params[key] !== "string") playback.composition = params[key];
                else defaults[key] = params[key];
            }
            const timeline = anime.createTimeline({ ...playback, defaults });
            const add = timeline.add.bind(timeline);
            return attachInstanceApi(timeline, params, {
                defaults: timeline.defaults,
                add: async (childProps, position) => {
                    const child = await transformPropsAsync(childProps, slots);
                    const targets = child.targets;
                    delete child.targets;
                    add(targets, child, position);
                }
            }, slots);
        },
        get: (target, propName, unit) =>
            unit !== undefined && unit !== null
                ? anime.get(target, propName, unit)
                : anime.get(target, propName),
        set: async (targets, props) => {
            // utils.set returns a thenable animation. Leave it so this call resolves immediately.
            anime.set(targets, await transformPropsAsync(props, []));
        },
        remove: (targets) => {
            anime.remove(targets);
        },
        random: (min, max) => anime.random(min, max),
        getSpeed: () => anime.engine.speed,
        setSpeed: (value) => {
            anime.engine.speed = value;
        },
        version: () => __ANIMEJS_VERSION__,
        pauseOnDocumentHidden: (value) => {
            anime.engine.pauseOnDocumentHidden = value;
        },
        getPauseOnDocumentHidden: () => anime.engine.pauseOnDocumentHidden,
        createMotionPath: (path, offset = 0) => {
            const motion = anime.createMotionPath(path, offset);
            if (!motion) {
                throw new Error("CreateMotionPath did not find a path, polygon, or polyline");
            }
            return {
                getTranslateX: () => motion.translateX,
                getTranslateY: () => motion.translateY,
                getRotate: () => motion.rotate
            };
        },
        createDrawable: (target) => ({
            target: anime.createDrawable(target)
        }),
        morphTo: (shape, precision = 0.33) => anime.morphTo(shape, precision),
        createObject: (values) => {
            const target = Object.assign({}, values);
            return {
                target,
                readNumber: (name) => {
                    const value = target[name];
                    return typeof value === "number" ? value : Number(value);
                },
                readString: (name) => {
                    const value = target[name];
                    return value == null ? "" : String(value);
                }
            };
        }
    };
}
