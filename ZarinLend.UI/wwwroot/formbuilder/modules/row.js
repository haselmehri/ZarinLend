import { BaseContainer } from "./baseContainer";

export class Row extends BaseContainer {
  constructor(key, uniqueId = undefined) {
    super();
    this.key = key;
    this.type = "Row";
    this.typePrefix = "rw";

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
    element.setAttribute("class", "row");
    element.setAttribute("data-key", this.key);
    return element;
  }
}
