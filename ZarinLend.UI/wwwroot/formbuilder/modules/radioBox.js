export class RadioBox {
  constructor(key, label, values = [], uniqueId = undefined) {
    this.key = key;
    this.type = "RadioBox";
    this.typePrefix = "rdo";
    this.values = values;
    this.label = label;

    this.fromSchema = false;
    if (uniqueId !== undefined) {
      this.uniqueId = uniqueId;
      this.fromSchema = true;
    } else this.uniqueId = `rdg_${this.generateUniqueId()}`;

    this.base = this.init();
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
    baseDiv.setAttribute("class", "form-group");
    baseDiv.setAttribute("id", this.uniqueId);
    baseDiv.setAttribute("component-type", this.type);
    baseDiv.setAttribute("data-key", this.key);

    let labelElement = document.createElement("label");
    labelElement.setAttribute("id", `lbl_${this.uniqueId}`);
    labelElement.innerText = this.label;

    baseDiv.appendChild(labelElement);
    return baseDiv;
  }

  create() {
    this.values.forEach((item) => {
      const { label: labelTxt, value } = item;

      let baseDiv = document.createElement("div");
      baseDiv.setAttribute(
        "class",
        "custom-control custom-radio custom-control-inline"
      );

      const uniqueId = `${this.typePrefix}_${this.generateUniqueId()}`;

      let label = document.createElement("label");
      label.innerHTML = labelTxt;
      label.setAttribute("class", "custom-control-label");
      label.setAttribute("for", uniqueId);

      let radioInput = document.createElement("input");
      radioInput.setAttribute("id", uniqueId);
      radioInput.setAttribute("type", "radio");
      radioInput.setAttribute("name", this.key);
      radioInput.setAttribute("value", value);
      radioInput.setAttribute("class", "custom-control-input");

      baseDiv.appendChild(radioInput);
      baseDiv.appendChild(label);

      this.base.appendChild(baseDiv);
    });

    return this.base;
  }

  print(data) {
    this.values.forEach((item) => {
      const { label: labelTxt, value } = item;

      let baseDiv = document.createElement("div");
      baseDiv.setAttribute(
        "class",
        "custom-control custom-radio custom-control-inline"
      );

      const uniqueId = `${this.typePrefix}_${this.generateUniqueId()}`;

      let label = document.createElement("label");
      label.innerHTML = labelTxt;
      label.setAttribute("class", "custom-control-label");
      label.setAttribute("for", uniqueId);

      let radioInput = document.createElement("input");
      radioInput.setAttribute("id", uniqueId);
      radioInput.setAttribute("type", "radio");
      radioInput.setAttribute("name", `rdo-${this.key}`);
      radioInput.setAttribute("value", value);
      radioInput.setAttribute("class", "custom-control-input");
      radioInput.setAttribute("disabled", "disabled");

      if (value == data) {
        radioInput.setAttribute("checked", "true");
      }

      baseDiv.appendChild(radioInput);
      baseDiv.appendChild(label);

      this.base.appendChild(baseDiv);
    });

    return this.base;
  }
}
