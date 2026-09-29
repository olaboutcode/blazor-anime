export function install(anime) {
    const api = {
        version() {
            return __ANIMEJS_VERSION__;
        },
        createAnimation(target, parameters) {
            const animation = anime.animate(target, {
                autoplay: false,
                ease: "linear",
                ...parameters
            });
            return {
                seek(time) {
                    animation.seek(time);
                }
            };
        },
        createTimer: anime.createTimer,
        createTimeline: anime.createTimeline,
        engine: anime.engine,
        stagger: anime.stagger,
        get: anime.get,
        set: anime.set,
        remove: anime.remove,
        random: anime.random,
        round: anime.round,
        createMotionPath: anime.createMotionPath,
        createDrawable: anime.createDrawable,
        morphTo: anime.morphTo,
        spring: anime.spring,
        cubicBezier: anime.cubicBezier,
        steps: anime.steps
    };

    globalThis.AnimeJs = api;
}
