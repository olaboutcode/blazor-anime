import { createDraggable } from "animejs/draggable";
import { cubicBezier } from "animejs/easings/cubic-bezier";
import { spring } from "animejs/easings/spring";
import { steps } from "animejs/easings/steps";
import { round, snap } from "animejs/utils";
// Side-effect import of the core entry. esbuild emits this as an import of
// BlazorAnime.lib.module.js, so the satellite and the core share one engine.
import "./entry.js";
import { installDraggable } from "./draggable-interop.js";

installDraggable({
    createDraggable,
    cubicBezier,
    round,
    snap,
    spring,
    steps
});
