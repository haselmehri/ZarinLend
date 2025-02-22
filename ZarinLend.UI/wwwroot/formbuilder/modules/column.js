import { BaseContainer } from "./baseContainer";

export class Column extends BaseContainer {
  constructor(key, options, uniqueId = undefined) {
    super();
    this.key = key;
    this.type = "Column";
    this.typePrefix = "col";
    this.options = options;

    this.fromSchema = false;

    if (uniqueId !== undefined) {
      this.uniqueId = uniqueId;
      this.fromSchema = true;
    }
  }
  create() {
    let element = document.createElement("div");

    if (this.fromSchema) element.setAttribute("id", this.uniqueId);
    else element.setAttribute("id", `${this.typePrefix}_${this.uniqueId}`);

    if (this.options === undefined) {
      element.setAttribute("class", "col");
    } else {
      element.setAttribute(
        "class",
        `col-${this.options.size}-${this.options.width}`
      );
    }

    element.setAttribute("data-key", this.key);
    return element;
  }
}
