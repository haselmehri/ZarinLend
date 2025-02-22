import { BaseElement } from "./baseElement";

export class TagsField extends BaseElement {
  constructor(key, label, placeholder, uniqueId = undefined) {
    super(label);

    this.key = key;
    this.type = "TagsField";
    this.typePrefix = "tgs";
    this.placeholder = placeholder;

    this.fromSchema = false;

    if (uniqueId !== undefined) {
      this.uniqueId = uniqueId;
      this.fromSchema = true;
    }

    this.base = this.init();
  }

  create() {
    let element = document.createElement("input");

    if (this.fromSchema) element.setAttribute("id", this.uniqueId);
    else element.setAttribute("id", `${this.typePrefix}_${this.uniqueId}`);

    element.setAttribute("type", "text");
    element.setAttribute("class", "form-control tags");
    element.setAttribute("placeholder", `${this.placeholder}`);
    element.setAttribute("data-key", this.key);

    this.base.appendChild(element);
    return this.base;
  }

  print(data) {
    let element = document.createElement("p");
    if (this.fromSchema) element.setAttribute("id", this.uniqueId);
    else element.setAttribute("id", `${this.typePrefix}_${this.uniqueId}`);
    element.setAttribute("class", "info-data");
    element.setAttribute("data-key", this.key);
    element.innerText = data;

    this.base.appendChild(element);
    return this.base;
  }
}
