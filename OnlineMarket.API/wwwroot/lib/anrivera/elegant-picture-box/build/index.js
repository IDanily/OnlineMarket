'use strict';

Object.defineProperty(exports, '__esModule', { value: true });

var React = require('react');
var popmotion = require('popmotion');

function _interopDefaultLegacy (e) { return e && typeof e === 'object' && 'default' in e ? e : { 'default': e }; }

function _interopNamespace(e) {
	if (e && e.__esModule) return e;
	var n = Object.create(null);
	if (e) {
		Object.keys(e).forEach(function (k) {
			if (k !== 'default') {
				var d = Object.getOwnPropertyDescriptor(e, k);
				Object.defineProperty(n, k, d.get ? d : {
					enumerable: true,
					get: function () { return e[k]; }
				});
			}
		});
	}
	n["default"] = e;
	return Object.freeze(n);
}

var React__namespace = /*#__PURE__*/_interopNamespace(React);
var React__default = /*#__PURE__*/_interopDefaultLegacy(React);

// @flow
const PictureBox = (props) => {
    return (React__namespace.createElement("div", null,
        React__namespace.createElement("span", null, "1")));
};

const defaultTimestep = (1 / 60) * 1000;
const getCurrentTime = typeof performance !== "undefined"
    ? () => performance.now()
    : () => Date.now();
const onNextFrame = typeof window !== "undefined"
    ? (callback) => window.requestAnimationFrame(callback)
    : (callback) => setTimeout(() => callback(getCurrentTime()), defaultTimestep);

function createRenderStep(runNextFrame) {
    let toRun = [];
    let toRunNextFrame = [];
    let numToRun = 0;
    let isProcessing = false;
    let flushNextFrame = false;
    const toKeepAlive = new WeakSet();
    const step = {
        schedule: (callback, keepAlive = false, immediate = false) => {
            const addToCurrentFrame = immediate && isProcessing;
            const buffer = addToCurrentFrame ? toRun : toRunNextFrame;
            if (keepAlive)
                toKeepAlive.add(callback);
            if (buffer.indexOf(callback) === -1) {
                buffer.push(callback);
                if (addToCurrentFrame && isProcessing)
                    numToRun = toRun.length;
            }
            return callback;
        },
        cancel: (callback) => {
            const index = toRunNextFrame.indexOf(callback);
            if (index !== -1)
                toRunNextFrame.splice(index, 1);
            toKeepAlive.delete(callback);
        },
        process: (frameData) => {
            if (isProcessing) {
                flushNextFrame = true;
                return;
            }
            isProcessing = true;
            [toRun, toRunNextFrame] = [toRunNextFrame, toRun];
            toRunNextFrame.length = 0;
            numToRun = toRun.length;
            if (numToRun) {
                for (let i = 0; i < numToRun; i++) {
                    const callback = toRun[i];
                    callback(frameData);
                    if (toKeepAlive.has(callback)) {
                        step.schedule(callback);
                        runNextFrame();
                    }
                }
            }
            isProcessing = false;
            if (flushNextFrame) {
                flushNextFrame = false;
                step.process(frameData);
            }
        },
    };
    return step;
}

const maxElapsed = 40;
let useDefaultElapsed = true;
let runNextFrame = false;
let isProcessing = false;
const frame = {
    delta: 0,
    timestamp: 0,
};
const stepsOrder = [
    "read",
    "update",
    "preRender",
    "render",
    "postRender",
];
const steps = stepsOrder.reduce((acc, key) => {
    acc[key] = createRenderStep(() => (runNextFrame = true));
    return acc;
}, {});
const sync = stepsOrder.reduce((acc, key) => {
    const step = steps[key];
    acc[key] = (process, keepAlive = false, immediate = false) => {
        if (!runNextFrame)
            startLoop();
        return step.schedule(process, keepAlive, immediate);
    };
    return acc;
}, {});
stepsOrder.reduce((acc, key) => {
    acc[key] = steps[key].cancel;
    return acc;
}, {});
stepsOrder.reduce((acc, key) => {
    acc[key] = () => steps[key].process(frame);
    return acc;
}, {});
const processStep = (stepId) => steps[stepId].process(frame);
const processFrame = (timestamp) => {
    runNextFrame = false;
    frame.delta = useDefaultElapsed
        ? defaultTimestep
        : Math.max(Math.min(timestamp - frame.timestamp, maxElapsed), 1);
    frame.timestamp = timestamp;
    isProcessing = true;
    stepsOrder.forEach(processStep);
    isProcessing = false;
    if (runNextFrame) {
        useDefaultElapsed = false;
        onNextFrame(processFrame);
    }
};
const startLoop = () => {
    runNextFrame = true;
    useDefaultElapsed = true;
    if (!isProcessing)
        onNextFrame(processFrame);
};

function styleInject(css, ref) {
  if ( ref === void 0 ) ref = {};
  var insertAt = ref.insertAt;

  if (!css || typeof document === 'undefined') { return; }

  var head = document.head || document.getElementsByTagName('head')[0];
  var style = document.createElement('style');
  style.type = 'text/css';

  if (insertAt === 'top') {
    if (head.firstChild) {
      head.insertBefore(style, head.firstChild);
    } else {
      head.appendChild(style);
    }
  } else {
    head.appendChild(style);
  }

  if (style.styleSheet) {
    style.styleSheet.cssText = css;
  } else {
    style.appendChild(document.createTextNode(css));
  }
}

var css_248z = "/* 280px >= 639px */\n.grid {\n  display: grid;\n  grid-template-columns: repeat(auto-fit, minmax(6rem, 1fr));\n  grid-auto-rows: 6rem;\n  grid-gap: 8px;\n  grid-auto-flow: dense; }\n  .grid .card {\n    overflow: hidden;\n    position: relative; }\n    .grid .card .item {\n      position: relative;\n      width: 100%;\n      height: 100%; }\n      .grid .card .item img {\n        width: 100%;\n        height: 100%;\n        object-fit: cover;\n        object-position: 50% 50%;\n        cursor: pointer;\n        transition: transform 1s; }\n\n.card.zoom {\n  grid-column: span 2;\n  grid-row: span 2; }\n  .card.zoom .item img {\n    height: auto;\n    transform: scale(1.03); }\n\n/* 640px >= 767px */\n@media screen and (min-width: 640px) {\n  .grid {\n    grid-template-columns: repeat(auto-fit, minmax(9rem, 1fr));\n    grid-auto-rows: 9rem; }\n  .card.zoom {\n    grid-column: span 3;\n    grid-row: span 3; } }\n\n/* 768px >= 1279px */\n@media screen and (min-width: 768px) {\n  .grid {\n    grid-template-columns: repeat(auto-fit, minmax(12rem, 1fr));\n    grid-auto-rows: 12rem; }\n  .card.zoom {\n    grid-column: span 3;\n    grid-row: span 3; } }\n";
styleInject(css_248z);

const popmotionEasing = {
    anticipate: popmotion.anticipate,
    backIn: popmotion.backIn,
    backInOut: popmotion.backInOut,
    backOut: popmotion.backOut,
    circIn: popmotion.circIn,
    circInOut: popmotion.circInOut,
    circOut: popmotion.circOut,
    easeIn: popmotion.easeIn,
    easeInOut: popmotion.easeInOut,
    easeOut: popmotion.easeOut,
    linear: popmotion.linear,
};
const DATASET_KEY = 'elegantPictureBoxId';
const itemCachePosition = {};
const toArray = (arrLike) => {
    if (!arrLike)
        return [];
    return Array.prototype.slice.call(arrLike);
};
const getCurrentPositionChildElement = (gridBoundingClientRect, el) => {
    const { top, left, width, height } = el.getBoundingClientRect();
    const rect = { top, left, width, height };
    rect.top -= gridBoundingClientRect.top;
    rect.left -= gridBoundingClientRect.left;
    // if an element is display:none it will return top: 0 and left:0
    // rather than saying it's still in the containing element
    // so we need to use Math.max to make sure the coordinates stay
    // within the container
    rect.top = Math.max(rect.top, 0);
    rect.left = Math.max(rect.left, 0);
    return rect;
};
const applyTransform = (el, { translateX, translateY, scaleX, scaleY }, { immediate } = {}) => {
    const isFinished = translateX === 0 && translateY === 0 && scaleX === 1 && scaleY === 1;
    const styleEl = () => {
        el.style.transform = isFinished
            ? ''
            : `translateX(${translateX}px) translateY(${translateY}px) scaleX(${scaleX}) scaleY(${scaleY})`;
    };
    if (immediate)
        styleEl();
    else
        sync.render(styleEl);
    const firstChild = el.children[0];
    if (firstChild) {
        const styleChild = () => {
            firstChild.style.transform = isFinished ? '' : `scaleX(${1 / scaleX}) scaleY(${1 / scaleY})`;
        };
        if (immediate)
            styleChild();
        else
            sync.render(styleChild);
    }
};
const registerPositions = (gridBoundingClientRect, elements) => {
    const childrenElements = toArray(elements);
    childrenElements.forEach((el) => {
        if (typeof el.getBoundingClientRect !== 'function')
            return;
        //agregar propiedad data-aniamted-grid-id con un ID
        if (!el.dataset[DATASET_KEY])
            el.dataset[DATASET_KEY] = `${Math.random()}`;
        const animatedGridId = el.dataset[DATASET_KEY];
        if (!itemCachePosition[animatedGridId])
            itemCachePosition[animatedGridId] = {};
        const currentPositionChildElement = getCurrentPositionChildElement(gridBoundingClientRect, el);
        itemCachePosition[animatedGridId].childElement = currentPositionChildElement;
        itemCachePosition[animatedGridId].parentElement = gridBoundingClientRect;
    });
};
const stopCurrentTransitions = (container) => {
    const childrenElements = toArray(container.children);
    childrenElements.filter((el) => {
        const position = itemCachePosition[el.dataset[DATASET_KEY]];
        if (position && position.stop) {
            position.stop();
            delete position.stop;
            return true;
        }
    });
    childrenElements.forEach((el) => {
        el.style.transform = '';
        const firstChild = el.children[0];
        if (firstChild)
            firstChild.style.transform = '';
    });
    return childrenElements;
};
const getNewPositions = (gridBoundingClientRect, childrenElements) => {
    const positionGridChildren = childrenElements.map((el) => ({
        childCoords: {},
        el,
        currentPositionChildElement: getCurrentPositionChildElement(gridBoundingClientRect, el),
    }));
    positionGridChildren.filter(({ el, currentPositionChildElement }) => {
        const position = itemCachePosition[el.dataset[DATASET_KEY]];
        if (!position) {
            registerPositions(currentPositionChildElement, [el]);
            return false;
        }
        else if (gridBoundingClientRect.top === position.childElement.top &&
            gridBoundingClientRect.left === position.childElement.left &&
            gridBoundingClientRect.width === position.childElement.width &&
            gridBoundingClientRect.height === position.childElement.height) {
            // if it hasn't moved, dont animate it
            return false;
        }
        return true;
    });
    positionGridChildren.forEach(({ el }) => {
        if (toArray(el.children).length > 1) {
            throw new Error('Make sure every grid item has a single container element surrounding its children');
        }
    });
    if (!positionGridChildren.length)
        return;
    positionGridChildren
        // do this measurement first so as not to cause layout thrashing
        .map((data) => {
        const firstChild = data.el.children[0];
        if (firstChild)
            data.childCoords = getCurrentPositionChildElement(gridBoundingClientRect, firstChild);
        return data;
    });
    return positionGridChildren;
};
const startAnimation = (gridBoundingClientRect, positionGridChildren, transition, duration, timeOut) => {
    positionGridChildren.forEach(({ el, currentPositionChildElement: { top, left, width, height }, childCoords: { top: childTop, left: childLeft }, }, i) => {
        const firstChild = el.children[0];
        const position = itemCachePosition[el.dataset[DATASET_KEY]];
        const coords = {
            scaleX: position.childElement.width / width,
            scaleY: position.childElement.height / height,
            translateX: position.childElement.left - left,
            translateY: position.childElement.top - top,
        };
        el.style.transformOrigin = '0 0';
        if (firstChild) {
            firstChild.style.transformOrigin = '0 0';
            firstChild.style.transition = '10s';
        }
        applyTransform(el, coords, { immediate: true });
        if (!popmotionEasing[transition])
            throw new Error(`${transition} is not a valid easing name`);
        const init = () => {
            const animation = popmotion.animate({
                from: coords,
                to: { translateX: 0, translateY: 0, scaleX: 1, scaleY: 1 },
                duration: duration,
                ease: popmotionEasing[transition],
                onUpdate: (transforms) => {
                    applyTransform(el, transforms);
                    sync.postRender(() => registerPositions(gridBoundingClientRect, [el]));
                },
            });
            position.stop = () => animation.stop;
        };
        const timeoutId = setTimeout(() => {
            sync.update(init);
        }, timeOut * i);
        position.stop = () => clearTimeout(timeoutId);
    });
};
const PicturesGrid = ({ items, transition, duration, timeOut }) => {
    const gridRef = React.useRef(null);
    React.useEffect(() => {
        const grid = gridRef.current;
        const gridsItemPosition = grid.getBoundingClientRect();
        registerPositions(gridsItemPosition, grid.children);
        const childrenElement = stopCurrentTransitions(grid);
        grid.addEventListener('click', (ev) => {
            let target = ev.target;
            if (target.tagName === 'IMG') {
                const elParent = target.parentElement['parentElement'];
                //target.parentElement.classList.toggle('zoom');
                elParent.classList.toggle('zoom');
                const newPositions = getNewPositions(gridsItemPosition, childrenElement);
                startAnimation(gridsItemPosition, newPositions, transition, duration, timeOut);
                return;
            }
        });
    });
    return (React__default["default"].createElement("div", { ref: gridRef, className: 'grid' }, items.map((item, index) => (React__default["default"].createElement("div", { className: 'card', key: index },
        React__default["default"].createElement("div", { className: 'item' },
            React__default["default"].createElement("img", { src: item.img, alt: item.name })))))));
};

exports.PictureBox = PictureBox;
exports.PicturesGrid = PicturesGrid;
//# sourceMappingURL=index.js.map
