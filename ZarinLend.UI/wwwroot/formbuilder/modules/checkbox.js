export class Checkbox {
  constructor(key, labelValue, uniqueId = undefined) {
    this.key = key;
    this.type = "Checkbox";
    this.typePrefix = "chk";
    this.base = this.init();
    this.labelValue = labelValue;

    this.fromSchema = false;
    if (uniqueId !== undefined) {
      this.uniqueId = uniqueId;
      this.fromSchema = true;
    } else this.uniqueId = this.generateUniqueId();
  }

  generateUniqueId() {
    return "xxxxxxxxxxxx".replace(/[xy]/g, function (c) {
      var r = (Math.random() * 16) | 0,
        v = c === "x" ? r : (r & 0x3) | 0x8;
      return v.toString(16);
    });
  }

  init() {
    let baseDiv = document.createElement("div");
    baseDiv.setAttribute("class", "custom-control custom-checkbox");
    baseDiv.setAttribute("id", this.uniqueId);
    baseDiv.setAttribute("component-type", this.type);

    return baseDiv;
  }

  create() {
    let checkInput = document.createElement("input");

    if (this.fromSchema) checkInput.setAttribute("id", this.uniqueId);
    else checkInput.setAttribute("id", `${this.typePrefix}_${this.uniqueId}`);

    checkInput.setAttribute("type", "checkbox");
    checkInput.setAttribute("class", "custom-control-input");
    checkInput.setAttribute("data-key", this.key);

    let label = document.createElement("label");
    if (this.fromSchema) label.setAttribute("for", this.uniqueId);
    else label.setAttribute("for", `${this.typePrefix}_${this.uniqueId}`);
    label.innerText = this.labelValue;
    label.setAttribute("class", "custom-control-label");

    this.base.appendChild(checkInput);
    this.base.appendChild(label);
    return this.base;
  }

  print(data) {
    let checkInput = document.createElement("input");

    if (this.fromSchema) checkInput.setAttribute("id", this.uniqueId);
    else checkInput.setAttribute("id", `${this.typePrefix}_${this.uniqueId}`);

    checkInput.setAttribute("type", "checkbox");
    checkInput.setAttribute("class", "custom-control-input");
    checkInput.setAttribute("data-key", this.key);
    checkInput.setAttribute("disabled", "disabled");
    if (data == true) checkInput.setAttribute("checked", "true");

    let label = document.createElement("label");
    if (this.fromSchema) label.setAttribute("for", this.uniqueId);
    else label.setAttribute("for", `${this.typePrefix}_${this.uniqueId}`);
    label.innerText = this.labelValue;
    label.setAttribute("class", "custom-control-label");

    this.base.appendChild(checkInput);
    this.base.appendChild(label);
    return this.base;
  }
}
