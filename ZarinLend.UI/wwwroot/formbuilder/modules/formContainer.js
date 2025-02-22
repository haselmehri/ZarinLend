import { BaseContainer } from "./baseContainer";

export class FormContainer extends BaseContainer {
  constructor(name, version, uniqueId = undefined) {
    super();

    this.name = name;
    this.version = version;
    this.type = "form";
    this.typePrefix = "frm";

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

    element.setAttribute("class", "form-body");
    element.setAttribute("data-name", this.name);
    element.setAttribute("data-version", this.version);

    return element;
  }
}
