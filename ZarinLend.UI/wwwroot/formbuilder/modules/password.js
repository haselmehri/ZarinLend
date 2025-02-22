import { BaseElement } from "./baseElement";

export class Password extends BaseElement {
  constructor(key, label, placeholder, uniqueId = undefined) {
    super(key, label);
    this.type = "Password";
    this.typePrefix = "pas";
    this.placeholder = placeholder;
    this.base = this.init();

    this.fromSchema = false;

    if (uniqueId !== undefined) {
      this.uniqueId = uniqueId;
      this.fromSchema = true;
    }
  }

  create() {
    let element = document.createElement("input");

    if (this.fromSchema) element.setAttribute("id", this.uniqueId);
    else element.setAttribute("id", `${this.typePrefix}_${this.uniqueId}`);

    element.setAttribute("type", "password");
    element.setAttribute("class", "form-control");
    element.setAttribute("placeholder", `${this.placeholder}`);
    element.setAttribute("data-key", this.key);

    this.base.appendChild(element);
    return this.base;
  }

  Print(data) {
    let element = document.createElement("input");

    if (this.fromSchema) element.setAttribute("id", this.uniqueId);
    else element.setAttribute("id", `${this.typePrefix}_${this.uniqueId}`);

    element.setAttribute("type", "password");
    element.setAttribute("class", "form-control");
    element.setAttribute("placeholder", `${this.placeholder}`);
    element.setAttribute("data-key", this.key);
    element.value = data;

    this.base.appendChild(element);
    return this.base;
  }
}
