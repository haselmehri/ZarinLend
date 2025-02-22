import { BaseElement } from "./baseElement";

export class FileInput extends BaseElement {
  constructor(key, label, placeholder, uniqueId = undefined) {
    super(label);

    this.key = key;
    this.type = "FileInput";
    this.typePrefix = "fln";
    this.placeholder = placeholder;

    this.fromSchema = false;

    if (uniqueId !== undefined) {
      this.uniqueId = uniqueId;
      this.fromSchema = true;
    }

    this.base = this.init();
  }

  create() {
    let container = document.createElement("div");
    container.setAttribute("class", "input-file-container");

    let input = document.createElement("input");

    let id = "";
    if (this.fromSchema) {
      input.setAttribute("id", this.uniqueId);
      id = this.uniqueId;
    } else {
      input.setAttribute("id", `${this.typePrefix}_${this.uniqueId}`);
      id = `${this.typePrefix}_${this.uniqueId}`;
    }

    input.setAttribute("type", "file");
    input.setAttribute("class", "input-file");
    input.setAttribute("data-key", this.key);

    let label = document.createElement("label");

    label.setAttribute("tabindex", "0");
    label.setAttribute("for", "0");
    label.setAttribute("class", "input-file-trigger");
    label.innerHTML = this.placeholder;

    container.appendChild(input);
    container.appendChild(label);
    this.base.appendChild(container);
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
