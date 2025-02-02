import type { Easing } from 'popmotion';
interface Image {
    img: string;
    name: string;
}
export interface Transition {
    linear: Easing;
    easeIn: Easing;
    easeOut: Easing;
    easeInOut: Easing;
    circIn: Easing;
    circOut: Easing;
    circInOut: Easing;
    backIn: Easing;
    backOut: Easing;
    backInOut: Easing;
    anticipate: Easing;
}
export interface ItemsProps {
    items: Array<Image>;
    transition: keyof Transition;
    duration: number;
    timeOut: number;
}
export interface PositionCoordinates {
    top: number;
    left: number;
    width: number;
    height: number;
}
export interface Positions {
    childElement: PositionCoordinates;
    parentElement: PositionCoordinates;
    stop?: () => void;
}
export interface ItemCachePosition {
    [key: string]: Positions;
}
export interface Coords {
    translateX: number;
    translateY: number;
    scaleX: number;
    scaleY: number;
    [key: string]: number;
}
export interface ChildCoordinates {
    top?: number;
    left?: number;
}
export {};
