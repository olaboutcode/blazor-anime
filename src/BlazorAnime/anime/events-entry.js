import { onScroll } from "animejs/events";
import { cubicBezier } from "animejs/easings/cubic-bezier";
import { spring } from "animejs/easings/spring";
import { steps } from "animejs/easings/steps";
// Side-effect import of the core entry. esbuild emits this as an import of
// BlazorAnime.lib.module.js, so the satellite and the core share one engine.
import "./entry.js";
import { installEvents } from "./events-interop.js";

installEvents({
    onScroll,
    cubicBezier,
    spring,
    steps
});
