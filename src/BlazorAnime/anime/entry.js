// Subpath imports keep the package barrel out of the shared chunk. The barrel
// re-exports createAnimatable, and a shared barrel would ship it in the core file.
import { animate } from "animejs/animation";
import { createTimer } from "animejs/timer";
import { createTimeline } from "animejs/timeline";
import { engine } from "animejs/engine";
import { cubicBezier } from "animejs/easings/cubic-bezier";
import { spring } from "animejs/easings/spring";
import { steps } from "animejs/easings/steps";
import { get, random, remove, round, set, stagger } from "animejs/utils";
import { createDrawable, createMotionPath, morphTo } from "animejs/svg";
import { install } from "./interop.js";

install({
    animate,
    createDrawable,
    createMotionPath,
    createTimeline,
    createTimer,
    cubicBezier,
    engine,
    get,
    morphTo,
    random,
    remove,
    round,
    set,
    spring,
    stagger,
    steps
});
