import { defineBoot } from "#q-app/wrappers";
import { type ComponentConstructor, QBtn, type QBtnProps } from "quasar";

const setComponentDefaults = <T>(component: ComponentConstructor<T>, defaults: Partial<T>): void => {
  (Object.keys(defaults) as (keyof typeof defaults)[]).forEach((prop: keyof typeof defaults) => {
    component.props[prop] =
      Array.isArray(component.props[prop]) === true || typeof component.props[prop] === "function"
        ? {
            type: component.props[prop],
            default: defaults[prop],
          }
        : {
            ...component.props[prop],
            default: defaults[prop],
          };
  });
};

export default defineBoot(() => {
  setComponentDefaults<QBtnProps>(QBtn, {
    noCaps: true,
  });
});
