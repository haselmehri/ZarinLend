import { BaseElement } from "./baseElement";

export class Select extends BaseElement {
  constructor(key, label, values = [], uniqueId = undefined) {
    super(label);
    this.key = key;
    this.type = "Select";
    this.typePrefix = "drp";
    this.base = this.init();
    this.values = values;

    this.fromSchema = false;

    if (uniqueId !== undefined) {
      this.uniqueId = uniqueId;
      this.fromSchema = true;
    }
  }

  create() {
    let element = document.createElement("select");

    if (this.fromSchema) element.setAttribute("id", this.uniqueId);
    else element.setAttribute("id", `${this.typePrefix}_${this.uniqueId}`);

    element.setAttribute("class", "form-control");
    element.setAttribute("data-key", this.key);

    this.values.forEach((item) => {
      const { label, value } = item;

      let child = document.createElement("option");
      child.setAttribute("value", value);
      child.innerText = label;
      element.appendChild(child);
    });

    this.base.appendChild(element);
    return this.base;
  }
  print(data) {
    let element = document.createElement("p");
    if (this.fromSchema) element.setAttribute("id", this.uniqueId);
    else element.setAttribute("id", `${this.typePrefix}_${this.uniqueId}`);
    element.setAttribute("class", "info-data");
    element.setAttribute("data-key", this.key);

    if (data == "") {
      element.innerHTML = "&nbsp;";
    } else {
      this.values.forEach((item) => {
        const { label, value } = item;
        if (value == data) {
          element.innerText = label;
        }
      });
    }

    this.base.appendChild(element);
    return this.base;
  }
}
