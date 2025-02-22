import { FormContainer } from "./modules/formContainer";
import { Row } from "./modules/row";
import { Column } from "./modules/column";
import { TextField } from "./modules/textField";
import { Checkbox } from "./modules/checkbox";
import { TextArea } from "./modules/textArea";
import { Email } from "./modules/email";
import { NumberField } from "./modules/number";
import { Password } from "./modules/password";
import { RadioBox } from "./modules/radioBox";
import { Select } from "./modules/select";
import { DatePicker } from "./modules/datePicker";
import { FileInput } from "./modules/FileInput";
import { TagsField } from "./modules/tagsField";
import { GroupCheckbox } from "./modules/groupCheckbox";

export class FormBuilder {
  constructor(schema, parent) {
    this.parent = parent;
    this.schema = schema;
  }

  render() {
    let dataList = [];

    if (this.schema === undefined || this.schema == null) {
      console.error("schema is null.");
      return null;
    }

    if (this.parent === undefined || this.parent == null) {
      console.warn("form rendered on body.");
      this.parent = document.body;
    }

    if (this.schema.form === undefined) {
      console.error("schema syntax is invalid: form object not found.");
      return null;
    }

    if (
      this.schema.form.items === undefined ||
      this.schema.form.items.count === 0
    ) {
      console.warn("form is empty.");
      let result = {
        form: new FormContainer(
          this.schema.form.name,
          this.schema.form.version
        ),
        data: dataList,
      };
      return result;
    } else {
      let form = new FormContainer(
        this.schema.form.name,
        this.schema.form.version
      );
      let formElement = form.create();

      this.schema.form.items.forEach(function (item, index) {
        formElement.appendChild(createElement(item));
      });

      let result = {
        form: formElement,
        data: dataList,
      };
      return result;
    }

    function createElement(schema) {
      if (schema.isContaner) {
        return createContainer(schema);
      } else {
        return createField(schema);
      }
    }

    function createContainer(schema) {
      switch (schema.type) {
        case "Row":
          let row = new Row(schema.key, schema.uniqueId);
          let rowElement = row.create();

          schema.items.forEach(function (item, index) {
            rowElement.appendChild(createElement(item));
          });
          return rowElement;

        case "Column":
          let column = new Column(
            schema.key,
            {
              size: schema.size,
              width: schema.width,
            },
            schema.uniqueId
          );
          let columnElement = column.create();
          schema.items.forEach(function (item, index) {
            columnElement.appendChild(createElement(item));
          });
          return columnElement;

        default:
          let element = document.createElement("div");
          return element;
      }
    }

    function createField(schema) {
      dataList.push({
        key: schema.key,
        type: schema.type,
        validations: schema.validations,
      });

      const { key, type } = schema;

      switch (type) {
        case "TextField":
          let textField = new TextField(
            key,
            schema.label,
            schema.placeholder,
            schema.uniqueId
          );
          return textField.create();
        case "FileInput":
          let fileInput = new FileInput(
            key,
            schema.label,
            schema.placeholder,
            schema.uniqueId
          );
          return fileInput.create();
        case "Checkbox":
          let checkbox = new Checkbox(key, schema.label, schema.uniqueId);
          return checkbox.create();
        case "Email":
          let email = new Email(
            key,
            schema.label,
            schema.placeholder,
            schema.uniqueId
          );
          return email.create();
        case "Number":
          let number = new NumberField(
            key,
            schema.label,
            schema.placeholder,
            schema.uniqueId
          );
          return number.create();
        case "TagsField":
          let tags = new TagsField(
            key,
            schema.label,
            schema.placeholder,
            schema.uniqueId
          );
          return tags.create();
        case "Password":
          let password = new Password(
            key,
            schema.label,
            schema.placeholder,
            schema.uniqueId
          );
          return password.create();
        case "RadioBox":
          let radio = new RadioBox(
            key,
            schema.label,
            schema.values,
            schema.uniqueId
          );
          return radio.create();
        case "Select":
          let select = new Select(
            key,
            schema.label,
            schema.values,
            schema.uniqueId
          );
          return select.create();
        case "TextArea":
          let textarea = new TextArea(
            key,
            schema.label,
            schema.placeholder,
            schema.uniqueId
          );
          return textarea.create();
        case "Date":
          let datePicker = new DatePicker(
            key,
            schema.label,
            schema.placeholder,
            schema.uniqueId
          );
          return datePicker.create();
        case "GroupCheckbox": {
          console.log(key);
          let groupCheckbox = new GroupCheckbox(
            key,
            schema.label,
            schema.values,
            schema.uniqueId
          );
          return groupCheckbox.create();
        }
        default:
          let element = document.createElement("label");
          element.innerText = "default";
          return element;
      }
    }
  }

  print(data) {
    let dataList = data;

    if (this.schema === undefined || this.schema == null) {
      console.error("schema is null.");
      return null;
    }

    if (this.parent === undefined || this.parent == null) {
      console.warn("form rendered on body.");
      this.parent = document.body;
    }

    if (this.schema.form === undefined) {
      console.error("schema syntax is invalid: form object not found.");
      return null;
    }

    if (
      this.schema.form.items === undefined ||
      this.schema.form.items.count === 0
    ) {
      console.warn("form is empty.");
      let result = {
        form: new FormContainer(
          this.schema.form.name,
          this.schema.form.version
        ),
        data: dataList,
      };
      return result;
    } else {
      let form = new FormContainer(
        this.schema.form.name,
        this.schema.form.version
      );
      let formElement = form.create();

      this.schema.form.items.forEach(function (item, index) {
        formElement.appendChild(printElement(item, data));
      });

      let result = {
        form: formElement,
        data: data,
      };
      return result;
    }

    function printElement(schema, dt) {
      let dataValue = dt;
      if (schema.isContaner) {
        return printContainer(schema, dataValue);
      } else {
        return printField(schema, dataValue);
      }
    }

    function printContainer(schema, dataValue) {
      switch (schema.type) {
        case "Row":
          let row = new Row(schema.key, schema.uniqueId);
          let rowElement = row.create();

          schema.items.forEach(function (item, index) {
            rowElement.appendChild(printElement(item, dataValue));
          });
          return rowElement;

        case "Column":
          let column = new Column(
            schema.key,
            {
              size: schema.size,
              width: schema.width,
            },
            schema.uniqueId
          );
          let columnElement = column.create();
          schema.items.forEach(function (item, index) {
            columnElement.appendChild(printElement(item, dataValue));
          });
          return columnElement;

        default:
          let element = document.createElement("div");
          return element;
      }
    }

    function printField(schema, d) {
      const { key, type } = schema;

      switch (type) {
        case "TextField":
          let textField = new TextField(
            key,
            schema.label,
            schema.placeholder,
            schema.uniqueId
          );
          return textField.print(d[key]);
        case "FileInput":
          let fileInput = new FileInput(
            key,
            schema.label,
            schema.placeholder,
            schema.uniqueId
          );
          return fileInput.print(d[key]);
        case "Checkbox":
          let checkbox = new Checkbox(key, schema.label, schema.uniqueId);
          return checkbox.print(d[key]);
        case "Email":
          let email = new Email(
            key,
            schema.label,
            schema.placeholder,
            schema.uniqueId
          );
          return email.print(d[key]);
        case "Number":
          let number = new NumberField(
            key,
            schema.label,
            schema.placeholder,
            schema.uniqueId
          );
          return number.print(d[key]);

        case "TagsField":
          let tags = new TagsField(
            key,
            schema.label,
            schema.placeholder,
            schema.uniqueId
          );
          return tags.print(d[key]);
        case "Password":
          let password = new Password(
            key,
            schema.label,
            schema.placeholder,
            schema.uniqueId
          );
          return password.print(d[key]);
        case "RadioBox":
          let radio = new RadioBox(
            key,
            schema.label,
            schema.values,
            schema.uniqueId
          );
          return radio.print(d[key]);
        case "Select":
          let select = new Select(
            key,
            schema.label,
            schema.values,
            schema.uniqueId
          );
          return select.print(d[key]);
        case "TextArea":
          let textarea = new TextArea(
            key,
            schema.label,
            schema.placeholder,
            schema.uniqueId
          );
          return textarea.print(d[key]);
        case "Date":
          let datePicker = new DatePicker(
            key,
            schema.label,
            schema.placeholder,
            schema.uniqueId
          );
          return datePicker.print(d[key]);
        case "GroupCheckbox":
          let groupCheckbox = new GroupCheckbox(
            key,
            schema.label,
            schema.values,
            schema.uniqueId
          );
          return groupCheckbox.print(d[key]);
        default:
          let element = document.createElement("label");
          element.innerText = "default";
          return element;
      }
    }
  }
}
