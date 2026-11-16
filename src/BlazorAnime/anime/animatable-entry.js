import { createAnimatable } from "animejs/animatable";
import { cubicBezier } from "animejs/easings/cubic-bezier";
import { spring } from "animejs/easings/spring";
import { steps } from "animejs/easings/steps";
import { get, remove, round, snap, stagger } from "animejs/utils";
// Side-effect import of the core entry. esbuild emits this as an import of
// BlazorAnime.lib.module.js, so the satellite and the core share one engine.
import "./entry.js";
import { installAnimatable } from "./animatable-interop.js";

installAnimatable({
    createAnimatable,
    cubicBezier,
    get,
    remove,
    round,
    snap,
    spring,
    stagger,
    steps
});
