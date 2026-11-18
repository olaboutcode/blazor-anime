import { waapi } from "animejs/waapi";
import { cubicBezier } from "animejs/easings/cubic-bezier";
import { spring } from "animejs/easings/spring";
import { steps } from "animejs/easings/steps";
import { stagger } from "animejs/utils";
// Side-effect import of the core entry. esbuild emits this as an import of
// BlazorAnime.lib.module.js, so the satellite and the core share one engine.
import "./entry.js";
import { installWaapi } from "./waapi-interop.js";

installWaapi({
    waapi,
    cubicBezier,
    spring,
    steps,
    stagger
});
