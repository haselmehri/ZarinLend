import { BaseElement } from "./baseElement";

export class GroupCheckbox extends BaseElement {
  constructor(key, label, values = [], uniqueId = undefined) {
    super(label);
    this.type = "GroupCheckbox";
    this.typePrefix = "grp";
    this.values = values;
    this.base = this.init();
    this.key = key;
    this.fromSchema = false;

    if (uniqueId !== undefined) {
      this.uniqueId = uniqueId;
      this.fromSchema = true;
    }
  }

  create() {
    this.values.forEach((item, index) => {
      const { label, value } = item;

      let baseDiv = document.createElement("div");
      //   baseDiv.setAttribute("class", "custom-control custom-checkbox");
      if (this.fromSchema)
        baseDiv.setAttribute(
          "id",
          `${this.uniqueId}_${index + this.values.length + 1000}`
        );
      else
        baseDiv.setAttribute(
          "id",
          `${this.typePrefix}_${this.uniqueId}_${
            index + this.values.length + 1000
          }`
        );

      baseDiv.setAttribute("component-type", this.type);

      let datagroup = "";
      if (this.fromSchema) datagroup = this.uniqueId;
      else datagroup = `${this.typePrefix}_${this.uniqueId}`;
      console.log(this.key);
      let checkInput = document.createElement("input");
      let id = `${datagroup}_${value}`;

      checkInput.setAttribute("id", id);
      checkInput.setAttribute("type", "checkbox");
      checkInput.setAttribute("data-key", this.key);
      checkInput.setAttribute("data-value", value);
      checkInput.setAttribute("data-group", datagroup);

      let text = document.createElement("label");
      text.setAttribute("class", "checkbox-label");
      text.setAttribute("for", id);
      text.innerText = label;

      baseDiv.appendChild(checkInput);
      baseDiv.appendChild(text);
      this.base.appendChild(baseDiv);
    });
    return this.base;
  }

  print(data) {
    let datagroup = "";
    if (this.fromSchema) datagroup = this.uniqueId;
    else datagroup = `${this.typePrefix}_${this.uniqueId}`;

    this.values.forEach((item, index) => {
      const { label, value } = item;

      let baseDiv = document.createElement("div");
      //   baseDiv.setAttribute("class", "custom-control custom-checkbox");
      if (this.fromSchema)
        baseDiv.setAttribute(
          "id",
          `${this.uniqueId}_${index + this.values.length + 1000}`
        );
      else
        baseDiv.setAttribute(
          "id",
          `${this.typePrefix}_${this.uniqueId}_${
            index + this.values.length + 1000
          }`
        );

      baseDiv.setAttribute("component-type", this.type);

      let checkInput = document.createElement("input");
      let id = `${datagroup}_${value}`;

      checkInput.setAttribute("id", id);
      checkInput.setAttribute("type", "checkbox");
      checkInput.setAttribute("class", "custom-control-input");
      checkInput.setAttribute("data-key", this.key);
      checkInput.setAttribute("data-value", value);
      checkInput.setAttribute("disabled", "disabled");

      if (data.some((d) => d == value))
        checkInput.setAttribute("checked", "true");

      let text = document.createElement("label");
      if (this.fromSchema) text.setAttribute("for", this.uniqueId);
      else text.setAttribute("for", `${this.typePrefix}_${this.uniqueId}`);
      text.innerText = label;
      text.setAttribute("class", "custom-control-label");

      baseDiv.appendChild(checkInput);
      baseDiv.appendChild(text);
      this.base.appendChild(baseDiv);
    });
    return this.base;
  }
}
