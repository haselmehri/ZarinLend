(function(){function r(e,n,t){function o(i,f){if(!n[i]){if(!e[i]){var c="function"==typeof require&&require;if(!f&&c)return c(i,!0);if(u)return u(i,!0);var a=new Error("Cannot find module '"+i+"'");throw a.code="MODULE_NOT_FOUND",a}var p=n[i]={exports:{}};e[i][0].call(p.exports,function(r){var n=e[i][1][r];return o(n||r)},p,p.exports,r,e,n,t)}return n[i].exports}for(var u="function"==typeof require&&require,i=0;i<t.length;i++)o(t[i]);return o}return r})()({1:[function(require,module,exports){
"use strict";

var _formBuilder = require("./formBuilder");

$(document).ready(function () {
  var connectionType = document.getElementById("ConnectionType");
  var connectionSubject = document.getElementById("connectionSubject");
  var connectionTemplate = document.getElementById("connectionTemplate");
  var title = document.getElementById("templateTitle");
  var schema = JSON.parse(document.getElementById("schema").value);
  var data = JSON.parse(document.getElementById("data").value);
  var connectionTypeId = parseInt(document.getElementById("ConnectionTypeId").value);
  var connectionSubjectId = parseInt(document.getElementById("connectionSubjectId").value);
  var connectionTemplateId = parseInt(document.getElementById("connectionTemplateId").value);
  var formBase = document.getElementById("form-base");
  var connectionTypesUrl = "/Api/GetConnectionTypes";
  var connectionSubjectsUrl = "/Api/GetConnectionSubjects?typeId=" + connectionSubjectId;
  var connectionTemplatesUrl = "/Api/GetConnectionTemplates?subjectId=" + connectionTemplateId;
  $.getJSON(connectionTypesUrl, function (data) {
    data.forEach(function (item, index) {
      var option = document.createElement("option");
      option.text = item.name;
      option.value = item.id;
      if (item.id == connectionTypeId) option.selected = true;
      connectionType.appendChild(option);
    });
  });
  $.getJSON(connectionSubjectsUrl, function (data) {
    data.forEach(function (item, index) {
      var option = document.createElement("option");
      option.text = item.name;
      option.value = item.id;
      if (item.id == connectionSubjectId) option.selected = true;
      connectionSubject.appendChild(option);
    });
  });
  $.getJSON(connectionTemplatesUrl, function (data) {
    data.forEach(function (item, index) {
      var option = document.createElement("option");
      option.text = item.name;
      option.value = item.id;
      if (item.id == connectionTemplateId) option.selected = true;
      connectionTemplate.appendChild(option);
      title.innerText = item.name;
    });
  });
  var formbuilder = new _formBuilder.FormBuilder(schema, formBase);
  var print = formbuilder.print(data);
  formBase.innerHTML = "";
  formBase.appendChild(print.form);
});

},{"./formBuilder":2}],2:[function(require,module,exports){
"use strict";

Object.defineProperty(exports, "__esModule", {
  value: true
});
exports.FormBuilder = void 0;

var _formContainer = require("./modules/formContainer");

var _row = require("./modules/row");

var _column = require("./modules/column");

var _textField = require("./modules/textField");

var _checkbox = require("./modules/checkbox");

var _textArea = require("./modules/textArea");

var _email = require("./modules/email");

var _number = require("./modules/number");

var _password = require("./modules/password");

var _radioBox = require("./modules/radioBox");

var _select = require("./modules/select");

var _datePicker = require("./modules/datePicker");

var _FileInput = require("./modules/FileInput");

var _tagsField = require("./modules/tagsField");

var _groupCheckbox = require("./modules/groupCheckbox");

function _classCallCheck(instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError("Cannot call a class as a function"); } }

function _defineProperties(target, props) { for (var i = 0; i < props.length; i++) { var descriptor = props[i]; descriptor.enumerable = descriptor.enumerable || false; descriptor.configurable = true; if ("value" in descriptor) descriptor.writable = true; Object.defineProperty(target, descriptor.key, descriptor); } }

function _createClass(Constructor, protoProps, staticProps) { if (protoProps) _defineProperties(Constructor.prototype, protoProps); if (staticProps) _defineProperties(Constructor, staticProps); return Constructor; }

var FormBuilder = /*#__PURE__*/function () {
  function FormBuilder(schema, parent) {
    _classCallCheck(this, FormBuilder);

    this.parent = parent;
    this.schema = schema;
  }

  _createClass(FormBuilder, [{
    key: "render",
    value: function render() {
      var dataList = [];

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

      if (this.schema.form.items === undefined || this.schema.form.items.count === 0) {
        console.warn("form is empty.");
        var result = {
          form: new _formContainer.FormContainer(this.schema.form.name, this.schema.form.version),
          data: dataList
        };
        return result;
      } else {
        var form = new _formContainer.FormContainer(this.schema.form.name, this.schema.form.version);
        var formElement = form.create();
        this.schema.form.items.forEach(function (item, index) {
          formElement.appendChild(createElement(item));
        });
        var _result = {
          form: formElement,
          data: dataList
        };
        return _result;
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
            var row = new _row.Row(schema.key, schema.uniqueId);
            var rowElement = row.create();
            schema.items.forEach(function (item, index) {
              rowElement.appendChild(createElement(item));
            });
            return rowElement;

          case "Column":
            var column = new _column.Column(schema.key, {
              size: schema.size,
              width: schema.width
            }, schema.uniqueId);
            var columnElement = column.create();
            schema.items.forEach(function (item, index) {
              columnElement.appendChild(createElement(item));
            });
            return columnElement;

          default:
            var element = document.createElement("div");
            return element;
        }
      }

      function createField(schema) {
        dataList.push({
          key: schema.key,
          type: schema.type,
          validations: schema.validations
        });
        var key = schema.key,
            type = schema.type;

        switch (type) {
          case "TextField":
            var textField = new _textField.TextField(key, schema.label, schema.placeholder, schema.uniqueId);
            return textField.create();

          case "FileInput":
            var fileInput = new _FileInput.FileInput(key, schema.label, schema.placeholder, schema.uniqueId);
            return fileInput.create();

          case "Checkbox":
            var checkbox = new _checkbox.Checkbox(key, schema.label, schema.uniqueId);
            return checkbox.create();

          case "Email":
            var email = new _email.Email(key, schema.label, schema.placeholder, schema.uniqueId);
            return email.create();

          case "Number":
            var number = new _number.NumberField(key, schema.label, schema.placeholder, schema.uniqueId);
            return number.create();

          case "TagsField":
            var tags = new _tagsField.TagsField(key, schema.label, schema.placeholder, schema.uniqueId);
            return tags.create();

          case "Password":
            var password = new _password.Password(key, schema.label, schema.placeholder, schema.uniqueId);
            return password.create();

          case "RadioBox":
            var radio = new _radioBox.RadioBox(key, schema.label, schema.values, schema.uniqueId);
            return radio.create();

          case "Select":
            var select = new _select.Select(key, schema.label, schema.values, schema.uniqueId);
            return select.create();

          case "TextArea":
            var textarea = new _textArea.TextArea(key, schema.label, schema.placeholder, schema.uniqueId);
            return textarea.create();

          case "Date":
            var datePicker = new _datePicker.DatePicker(key, schema.label, schema.placeholder, schema.uniqueId);
            return datePicker.create();

          case "GroupCheckbox":
            {
              console.log(key);
              var groupCheckbox = new _groupCheckbox.GroupCheckbox(key, schema.label, schema.values, schema.uniqueId);
              return groupCheckbox.create();
            }

          default:
            var element = document.createElement("label");
            element.innerText = "default";
            return element;
        }
      }
    }
  }, {
    key: "print",
    value: function print(data) {
      var dataList = data;

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

      if (this.schema.form.items === undefined || this.schema.form.items.count === 0) {
        console.warn("form is empty.");
        var result = {
          form: new _formContainer.FormContainer(this.schema.form.name, this.schema.form.version),
          data: dataList
        };
        return result;
      } else {
        var form = new _formContainer.FormContainer(this.schema.form.name, this.schema.form.version);
        var formElement = form.create();
        this.schema.form.items.forEach(function (item, index) {
          formElement.appendChild(printElement(item, data));
        });
        var _result2 = {
          form: formElement,
          data: data
        };
        return _result2;
      }

      function printElement(schema, dt) {
        var dataValue = dt;

        if (schema.isContaner) {
          return printContainer(schema, dataValue);
        } else {
          return printField(schema, dataValue);
        }
      }

      function printContainer(schema, dataValue) {
        switch (schema.type) {
          case "Row":
            var row = new _row.Row(schema.key, schema.uniqueId);
            var rowElement = row.create();
            schema.items.forEach(function (item, index) {
              rowElement.appendChild(printElement(item, dataValue));
            });
            return rowElement;

          case "Column":
            var column = new _column.Column(schema.key, {
              size: schema.size,
              width: schema.width
            }, schema.uniqueId);
            var columnElement = column.create();
            schema.items.forEach(function (item, index) {
              columnElement.appendChild(printElement(item, dataValue));
            });
            return columnElement;

          default:
            var element = document.createElement("div");
            return element;
        }
      }

      function printField(schema, d) {
        var key = schema.key,
            type = schema.type;

        switch (type) {
          case "TextField":
            var textField = new _textField.TextField(key, schema.label, schema.placeholder, schema.uniqueId);
            return textField.print(d[key]);

          case "FileInput":
            var fileInput = new _FileInput.FileInput(key, schema.label, schema.placeholder, schema.uniqueId);
            return fileInput.print(d[key]);

          case "Checkbox":
            var checkbox = new _checkbox.Checkbox(key, schema.label, schema.uniqueId);
            return checkbox.print(d[key]);

          case "Email":
            var email = new _email.Email(key, schema.label, schema.placeholder, schema.uniqueId);
            return email.print(d[key]);

          case "Number":
            var number = new _number.NumberField(key, schema.label, schema.placeholder, schema.uniqueId);
            return number.print(d[key]);

          case "TagsField":
            var tags = new _tagsField.TagsField(key, schema.label, schema.placeholder, schema.uniqueId);
            return tags.print(d[key]);

          case "Password":
            var password = new _password.Password(key, schema.label, schema.placeholder, schema.uniqueId);
            return password.print(d[key]);

          case "RadioBox":
            var radio = new _radioBox.RadioBox(key, schema.label, schema.values, schema.uniqueId);
            return radio.print(d[key]);

          case "Select":
            var select = new _select.Select(key, schema.label, schema.values, schema.uniqueId);
            return select.print(d[key]);

          case "TextArea":
            var textarea = new _textArea.TextArea(key, schema.label, schema.placeholder, schema.uniqueId);
            return textarea.print(d[key]);

          case "Date":
            var datePicker = new _datePicker.DatePicker(key, schema.label, schema.placeholder, schema.uniqueId);
            return datePicker.print(d[key]);

          case "GroupCheckbox":
            var groupCheckbox = new _groupCheckbox.GroupCheckbox(key, schema.label, schema.values, schema.uniqueId);
            return groupCheckbox.print(d[key]);

          default:
            var element = document.createElement("label");
            element.innerText = "default";
            return element;
        }
      }
    }
  }]);

  return FormBuilder;
}();

exports.FormBuilder = FormBuilder;

},{"./modules/FileInput":3,"./modules/checkbox":6,"./modules/column":7,"./modules/datePicker":8,"./modules/email":9,"./modules/formContainer":10,"./modules/groupCheckbox":11,"./modules/number":12,"./modules/password":13,"./modules/radioBox":14,"./modules/row":15,"./modules/select":16,"./modules/tagsField":17,"./modules/textArea":18,"./modules/textField":19}],3:[function(require,module,exports){
"use strict";

Object.defineProperty(exports, "__esModule", {
  value: true
});
exports.FileInput = void 0;

var _baseElement = require("./baseElement");

function _typeof(obj) { "@babel/helpers - typeof"; if (typeof Symbol === "function" && typeof Symbol.iterator === "symbol") { _typeof = function _typeof(obj) { return typeof obj; }; } else { _typeof = function _typeof(obj) { return obj && typeof Symbol === "function" && obj.constructor === Symbol && obj !== Symbol.prototype ? "symbol" : typeof obj; }; } return _typeof(obj); }

function _classCallCheck(instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError("Cannot call a class as a function"); } }

function _defineProperties(target, props) { for (var i = 0; i < props.length; i++) { var descriptor = props[i]; descriptor.enumerable = descriptor.enumerable || false; descriptor.configurable = true; if ("value" in descriptor) descriptor.writable = true; Object.defineProperty(target, descriptor.key, descriptor); } }

function _createClass(Constructor, protoProps, staticProps) { if (protoProps) _defineProperties(Constructor.prototype, protoProps); if (staticProps) _defineProperties(Constructor, staticProps); return Constructor; }

function _inherits(subClass, superClass) { if (typeof superClass !== "function" && superClass !== null) { throw new TypeError("Super expression must either be null or a function"); } subClass.prototype = Object.create(superClass && superClass.prototype, { constructor: { value: subClass, writable: true, configurable: true } }); if (superClass) _setPrototypeOf(subClass, superClass); }

function _setPrototypeOf(o, p) { _setPrototypeOf = Object.setPrototypeOf || function _setPrototypeOf(o, p) { o.__proto__ = p; return o; }; return _setPrototypeOf(o, p); }

function _createSuper(Derived) { var hasNativeReflectConstruct = _isNativeReflectConstruct(); return function () { var Super = _getPrototypeOf(Derived), result; if (hasNativeReflectConstruct) { var NewTarget = _getPrototypeOf(this).constructor; result = Reflect.construct(Super, arguments, NewTarget); } else { result = Super.apply(this, arguments); } return _possibleConstructorReturn(this, result); }; }

function _possibleConstructorReturn(self, call) { if (call && (_typeof(call) === "object" || typeof call === "function")) { return call; } return _assertThisInitialized(self); }

function _assertThisInitialized(self) { if (self === void 0) { throw new ReferenceError("this hasn't been initialised - super() hasn't been called"); } return self; }

function _isNativeReflectConstruct() { if (typeof Reflect === "undefined" || !Reflect.construct) return false; if (Reflect.construct.sham) return false; if (typeof Proxy === "function") return true; try { Date.prototype.toString.call(Reflect.construct(Date, [], function () {})); return true; } catch (e) { return false; } }

function _getPrototypeOf(o) { _getPrototypeOf = Object.setPrototypeOf ? Object.getPrototypeOf : function _getPrototypeOf(o) { return o.__proto__ || Object.getPrototypeOf(o); }; return _getPrototypeOf(o); }

var FileInput = /*#__PURE__*/function (_BaseElement) {
  _inherits(FileInput, _BaseElement);

  var _super = _createSuper(FileInput);

  function FileInput(key, label, placeholder) {
    var _this;

    var uniqueId = arguments.length > 3 && arguments[3] !== undefined ? arguments[3] : undefined;

    _classCallCheck(this, FileInput);

    _this = _super.call(this, label);
    _this.key = key;
    _this.type = "FileInput";
    _this.typePrefix = "fln";
    _this.placeholder = placeholder;
    _this.fromSchema = false;

    if (uniqueId !== undefined) {
      _this.uniqueId = uniqueId;
      _this.fromSchema = true;
    }

    _this.base = _this.init();
    return _this;
  }

  _createClass(FileInput, [{
    key: "create",
    value: function create() {
      var container = document.createElement("div");
      container.setAttribute("class", "input-file-container");
      var input = document.createElement("input");
      var id = "";

      if (this.fromSchema) {
        input.setAttribute("id", this.uniqueId);
        id = this.uniqueId;
      } else {
        input.setAttribute("id", "".concat(this.typePrefix, "_").concat(this.uniqueId));
        id = "".concat(this.typePrefix, "_").concat(this.uniqueId);
      }

      input.setAttribute("type", "file");
      input.setAttribute("class", "input-file");
      input.setAttribute("data-key", this.key);
      var label = document.createElement("label");
      label.setAttribute("tabindex", "0");
      label.setAttribute("for", "0");
      label.setAttribute("class", "input-file-trigger");
      label.innerHTML = this.placeholder;
      container.appendChild(input);
      container.appendChild(label);
      this.base.appendChild(container);
      return this.base;
    }
  }, {
    key: "print",
    value: function print(data) {
      var element = document.createElement("p");
      if (this.fromSchema) element.setAttribute("id", this.uniqueId);else element.setAttribute("id", "".concat(this.typePrefix, "_").concat(this.uniqueId));
      element.setAttribute("class", "info-data");
      element.setAttribute("data-key", this.key);
      element.innerText = data;
      this.base.appendChild(element);
      return this.base;
    }
  }]);

  return FileInput;
}(_baseElement.BaseElement);

exports.FileInput = FileInput;

},{"./baseElement":5}],4:[function(require,module,exports){
"use strict";

Object.defineProperty(exports, "__esModule", {
  value: true
});
exports.BaseContainer = void 0;

function _classCallCheck(instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError("Cannot call a class as a function"); } }

function _defineProperties(target, props) { for (var i = 0; i < props.length; i++) { var descriptor = props[i]; descriptor.enumerable = descriptor.enumerable || false; descriptor.configurable = true; if ("value" in descriptor) descriptor.writable = true; Object.defineProperty(target, descriptor.key, descriptor); } }

function _createClass(Constructor, protoProps, staticProps) { if (protoProps) _defineProperties(Constructor.prototype, protoProps); if (staticProps) _defineProperties(Constructor, staticProps); return Constructor; }

var BaseContainer = /*#__PURE__*/function () {
  function BaseContainer() {
    _classCallCheck(this, BaseContainer);

    this.uniqueId = this.generateUniqueId();
  }

  _createClass(BaseContainer, [{
    key: "generateUniqueId",
    value: function generateUniqueId() {
      return 'xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
        var r = Math.random() * 16 | 0,
            v = c === 'x' ? r : r & 0x3 | 0x8;
        return v.toString(16);
      });
    }
  }]);

  return BaseContainer;
}();

exports.BaseContainer = BaseContainer;

},{}],5:[function(require,module,exports){
"use strict";

Object.defineProperty(exports, "__esModule", {
  value: true
});
exports.BaseElement = void 0;

function _classCallCheck(instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError("Cannot call a class as a function"); } }

function _defineProperties(target, props) { for (var i = 0; i < props.length; i++) { var descriptor = props[i]; descriptor.enumerable = descriptor.enumerable || false; descriptor.configurable = true; if ("value" in descriptor) descriptor.writable = true; Object.defineProperty(target, descriptor.key, descriptor); } }

function _createClass(Constructor, protoProps, staticProps) { if (protoProps) _defineProperties(Constructor.prototype, protoProps); if (staticProps) _defineProperties(Constructor, staticProps); return Constructor; }

var BaseElement = /*#__PURE__*/function () {
  function BaseElement(label) {
    _classCallCheck(this, BaseElement);

    this.label = label;
    this.uniqueId = this.generateUniqueId();
  }

  _createClass(BaseElement, [{
    key: "generateUniqueId",
    value: function generateUniqueId() {
      return 'xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
        var r = Math.random() * 16 | 0,
            v = c === 'x' ? r : r & 0x3 | 0x8;
        return v.toString(16);
      });
    }
  }, {
    key: "init",
    value: function init() {
      var baseDiv = document.createElement('div');
      baseDiv.setAttribute('class', 'form-group');
      baseDiv.setAttribute('id', this.uniqueId);
      baseDiv.setAttribute('component-type', this.type);
      var label = document.createElement('label');
      label.setAttribute('id', "lbl_".concat(this.uniqueId));
      label.innerText = this.label;
      baseDiv.appendChild(label);
      return baseDiv;
    }
  }]);

  return BaseElement;
}();

exports.BaseElement = BaseElement;

},{}],6:[function(require,module,exports){
"use strict";

Object.defineProperty(exports, "__esModule", {
  value: true
});
exports.Checkbox = void 0;

function _classCallCheck(instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError("Cannot call a class as a function"); } }

function _defineProperties(target, props) { for (var i = 0; i < props.length; i++) { var descriptor = props[i]; descriptor.enumerable = descriptor.enumerable || false; descriptor.configurable = true; if ("value" in descriptor) descriptor.writable = true; Object.defineProperty(target, descriptor.key, descriptor); } }

function _createClass(Constructor, protoProps, staticProps) { if (protoProps) _defineProperties(Constructor.prototype, protoProps); if (staticProps) _defineProperties(Constructor, staticProps); return Constructor; }

var Checkbox = /*#__PURE__*/function () {
  function Checkbox(key, labelValue) {
    var uniqueId = arguments.length > 2 && arguments[2] !== undefined ? arguments[2] : undefined;

    _classCallCheck(this, Checkbox);

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

  _createClass(Checkbox, [{
    key: "generateUniqueId",
    value: function generateUniqueId() {
      return "xxxxxxxxxxxx".replace(/[xy]/g, function (c) {
        var r = Math.random() * 16 | 0,
            v = c === "x" ? r : r & 0x3 | 0x8;
        return v.toString(16);
      });
    }
  }, {
    key: "init",
    value: function init() {
      var baseDiv = document.createElement("div");
      baseDiv.setAttribute("class", "custom-control custom-checkbox");
      baseDiv.setAttribute("id", this.uniqueId);
      baseDiv.setAttribute("component-type", this.type);
      return baseDiv;
    }
  }, {
    key: "create",
    value: function create() {
      var checkInput = document.createElement("input");
      if (this.fromSchema) checkInput.setAttribute("id", this.uniqueId);else checkInput.setAttribute("id", "".concat(this.typePrefix, "_").concat(this.uniqueId));
      checkInput.setAttribute("type", "checkbox");
      checkInput.setAttribute("class", "custom-control-input");
      checkInput.setAttribute("data-key", this.key);
      var label = document.createElement("label");
      if (this.fromSchema) label.setAttribute("for", this.uniqueId);else label.setAttribute("for", "".concat(this.typePrefix, "_").concat(this.uniqueId));
      label.innerText = this.labelValue;
      label.setAttribute("class", "custom-control-label");
      this.base.appendChild(checkInput);
      this.base.appendChild(label);
      return this.base;
    }
  }, {
    key: "print",
    value: function print(data) {
      var checkInput = document.createElement("input");
      if (this.fromSchema) checkInput.setAttribute("id", this.uniqueId);else checkInput.setAttribute("id", "".concat(this.typePrefix, "_").concat(this.uniqueId));
      checkInput.setAttribute("type", "checkbox");
      checkInput.setAttribute("class", "custom-control-input");
      checkInput.setAttribute("data-key", this.key);
      checkInput.setAttribute("disabled", "disabled");
      if (data == true) checkInput.setAttribute("checked", "true");
      var label = document.createElement("label");
      if (this.fromSchema) label.setAttribute("for", this.uniqueId);else label.setAttribute("for", "".concat(this.typePrefix, "_").concat(this.uniqueId));
      label.innerText = this.labelValue;
      label.setAttribute("class", "custom-control-label");
      this.base.appendChild(checkInput);
      this.base.appendChild(label);
      return this.base;
    }
  }]);

  return Checkbox;
}();

exports.Checkbox = Checkbox;

},{}],7:[function(require,module,exports){
"use strict";

Object.defineProperty(exports, "__esModule", {
  value: true
});
exports.Column = void 0;

var _baseContainer = require("./baseContainer");

function _typeof(obj) { "@babel/helpers - typeof"; if (typeof Symbol === "function" && typeof Symbol.iterator === "symbol") { _typeof = function _typeof(obj) { return typeof obj; }; } else { _typeof = function _typeof(obj) { return obj && typeof Symbol === "function" && obj.constructor === Symbol && obj !== Symbol.prototype ? "symbol" : typeof obj; }; } return _typeof(obj); }

function _classCallCheck(instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError("Cannot call a class as a function"); } }

function _defineProperties(target, props) { for (var i = 0; i < props.length; i++) { var descriptor = props[i]; descriptor.enumerable = descriptor.enumerable || false; descriptor.configurable = true; if ("value" in descriptor) descriptor.writable = true; Object.defineProperty(target, descriptor.key, descriptor); } }

function _createClass(Constructor, protoProps, staticProps) { if (protoProps) _defineProperties(Constructor.prototype, protoProps); if (staticProps) _defineProperties(Constructor, staticProps); return Constructor; }

function _inherits(subClass, superClass) { if (typeof superClass !== "function" && superClass !== null) { throw new TypeError("Super expression must either be null or a function"); } subClass.prototype = Object.create(superClass && superClass.prototype, { constructor: { value: subClass, writable: true, configurable: true } }); if (superClass) _setPrototypeOf(subClass, superClass); }

function _setPrototypeOf(o, p) { _setPrototypeOf = Object.setPrototypeOf || function _setPrototypeOf(o, p) { o.__proto__ = p; return o; }; return _setPrototypeOf(o, p); }

function _createSuper(Derived) { var hasNativeReflectConstruct = _isNativeReflectConstruct(); return function () { var Super = _getPrototypeOf(Derived), result; if (hasNativeReflectConstruct) { var NewTarget = _getPrototypeOf(this).constructor; result = Reflect.construct(Super, arguments, NewTarget); } else { result = Super.apply(this, arguments); } return _possibleConstructorReturn(this, result); }; }

function _possibleConstructorReturn(self, call) { if (call && (_typeof(call) === "object" || typeof call === "function")) { return call; } return _assertThisInitialized(self); }

function _assertThisInitialized(self) { if (self === void 0) { throw new ReferenceError("this hasn't been initialised - super() hasn't been called"); } return self; }

function _isNativeReflectConstruct() { if (typeof Reflect === "undefined" || !Reflect.construct) return false; if (Reflect.construct.sham) return false; if (typeof Proxy === "function") return true; try { Date.prototype.toString.call(Reflect.construct(Date, [], function () {})); return true; } catch (e) { return false; } }

function _getPrototypeOf(o) { _getPrototypeOf = Object.setPrototypeOf ? Object.getPrototypeOf : function _getPrototypeOf(o) { return o.__proto__ || Object.getPrototypeOf(o); }; return _getPrototypeOf(o); }

var Column = /*#__PURE__*/function (_BaseContainer) {
  _inherits(Column, _BaseContainer);

  var _super = _createSuper(Column);

  function Column(key, options) {
    var _this;

    var uniqueId = arguments.length > 2 && arguments[2] !== undefined ? arguments[2] : undefined;

    _classCallCheck(this, Column);

    _this = _super.call(this);
    _this.key = key;
    _this.type = "Column";
    _this.typePrefix = "col";
    _this.options = options;
    _this.fromSchema = false;

    if (uniqueId !== undefined) {
      _this.uniqueId = uniqueId;
      _this.fromSchema = true;
    }

    return _this;
  }

  _createClass(Column, [{
    key: "create",
    value: function create() {
      var element = document.createElement("div");
      if (this.fromSchema) element.setAttribute("id", this.uniqueId);else element.setAttribute("id", "".concat(this.typePrefix, "_").concat(this.uniqueId));

      if (this.options === undefined) {
        element.setAttribute("class", "col");
      } else {
        element.setAttribute("class", "col-".concat(this.options.size, "-").concat(this.options.width));
      }

      element.setAttribute("data-key", this.key);
      return element;
    }
  }]);

  return Column;
}(_baseContainer.BaseContainer);

exports.Column = Column;

},{"./baseContainer":4}],8:[function(require,module,exports){
"use strict";

Object.defineProperty(exports, "__esModule", {
  value: true
});
exports.DatePicker = void 0;

var _baseElement = require("./baseElement");

function _typeof(obj) { "@babel/helpers - typeof"; if (typeof Symbol === "function" && typeof Symbol.iterator === "symbol") { _typeof = function _typeof(obj) { return typeof obj; }; } else { _typeof = function _typeof(obj) { return obj && typeof Symbol === "function" && obj.constructor === Symbol && obj !== Symbol.prototype ? "symbol" : typeof obj; }; } return _typeof(obj); }

function _classCallCheck(instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError("Cannot call a class as a function"); } }

function _defineProperties(target, props) { for (var i = 0; i < props.length; i++) { var descriptor = props[i]; descriptor.enumerable = descriptor.enumerable || false; descriptor.configurable = true; if ("value" in descriptor) descriptor.writable = true; Object.defineProperty(target, descriptor.key, descriptor); } }

function _createClass(Constructor, protoProps, staticProps) { if (protoProps) _defineProperties(Constructor.prototype, protoProps); if (staticProps) _defineProperties(Constructor, staticProps); return Constructor; }

function _inherits(subClass, superClass) { if (typeof superClass !== "function" && superClass !== null) { throw new TypeError("Super expression must either be null or a function"); } subClass.prototype = Object.create(superClass && superClass.prototype, { constructor: { value: subClass, writable: true, configurable: true } }); if (superClass) _setPrototypeOf(subClass, superClass); }

function _setPrototypeOf(o, p) { _setPrototypeOf = Object.setPrototypeOf || function _setPrototypeOf(o, p) { o.__proto__ = p; return o; }; return _setPrototypeOf(o, p); }

function _createSuper(Derived) { var hasNativeReflectConstruct = _isNativeReflectConstruct(); return function () { var Super = _getPrototypeOf(Derived), result; if (hasNativeReflectConstruct) { var NewTarget = _getPrototypeOf(this).constructor; result = Reflect.construct(Super, arguments, NewTarget); } else { result = Super.apply(this, arguments); } return _possibleConstructorReturn(this, result); }; }

function _possibleConstructorReturn(self, call) { if (call && (_typeof(call) === "object" || typeof call === "function")) { return call; } return _assertThisInitialized(self); }

function _assertThisInitialized(self) { if (self === void 0) { throw new ReferenceError("this hasn't been initialised - super() hasn't been called"); } return self; }

function _isNativeReflectConstruct() { if (typeof Reflect === "undefined" || !Reflect.construct) return false; if (Reflect.construct.sham) return false; if (typeof Proxy === "function") return true; try { Date.prototype.toString.call(Reflect.construct(Date, [], function () {})); return true; } catch (e) { return false; } }

function _getPrototypeOf(o) { _getPrototypeOf = Object.setPrototypeOf ? Object.getPrototypeOf : function _getPrototypeOf(o) { return o.__proto__ || Object.getPrototypeOf(o); }; return _getPrototypeOf(o); }

var DatePicker = /*#__PURE__*/function (_BaseElement) {
  _inherits(DatePicker, _BaseElement);

  var _super = _createSuper(DatePicker);

  function DatePicker(key, label) {
    var _this;

    var uniqueId = arguments.length > 2 && arguments[2] !== undefined ? arguments[2] : undefined;

    _classCallCheck(this, DatePicker);

    _this = _super.call(this, label);
    _this.key = key;
    _this.type = "Date";
    _this.typePrefix = "dt";
    _this.fromSchema = false;

    if (uniqueId !== undefined) {
      _this.uniqueId = uniqueId;
      _this.fromSchema = true;
    }

    _this.base = _this.init();
    return _this;
  }

  _createClass(DatePicker, [{
    key: "create",
    value: function create() {
      var element = document.createElement("input");
      if (this.fromSchema) element.setAttribute("id", this.uniqueId);else element.setAttribute("id", "".concat(this.typePrefix, "_").concat(this.uniqueId));
      element.setAttribute("type", "text");
      element.setAttribute("class", "form-control datepicker");
      element.setAttribute("data-key", this.key);
      this.base.appendChild(element);
      return this.base;
    }
  }, {
    key: "print",
    value: function print(data) {
      var element = document.createElement("p");
      if (this.fromSchema) element.setAttribute("id", this.uniqueId);else element.setAttribute("id", "".concat(this.typePrefix, "_").concat(this.uniqueId));
      element.setAttribute("class", "info-data");
      element.setAttribute("data-key", this.key);

      if (data == "") {
        element.innerHTML = "&nbsp;";
      } else element.innerText = data;

      this.base.appendChild(element);
      return this.base;
    }
  }]);

  return DatePicker;
}(_baseElement.BaseElement);

exports.DatePicker = DatePicker;

},{"./baseElement":5}],9:[function(require,module,exports){
"use strict";

Object.defineProperty(exports, "__esModule", {
  value: true
});
exports.Email = void 0;

var _baseElement = require("./baseElement");

function _typeof(obj) { "@babel/helpers - typeof"; if (typeof Symbol === "function" && typeof Symbol.iterator === "symbol") { _typeof = function _typeof(obj) { return typeof obj; }; } else { _typeof = function _typeof(obj) { return obj && typeof Symbol === "function" && obj.constructor === Symbol && obj !== Symbol.prototype ? "symbol" : typeof obj; }; } return _typeof(obj); }

function _classCallCheck(instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError("Cannot call a class as a function"); } }

function _defineProperties(target, props) { for (var i = 0; i < props.length; i++) { var descriptor = props[i]; descriptor.enumerable = descriptor.enumerable || false; descriptor.configurable = true; if ("value" in descriptor) descriptor.writable = true; Object.defineProperty(target, descriptor.key, descriptor); } }

function _createClass(Constructor, protoProps, staticProps) { if (protoProps) _defineProperties(Constructor.prototype, protoProps); if (staticProps) _defineProperties(Constructor, staticProps); return Constructor; }

function _inherits(subClass, superClass) { if (typeof superClass !== "function" && superClass !== null) { throw new TypeError("Super expression must either be null or a function"); } subClass.prototype = Object.create(superClass && superClass.prototype, { constructor: { value: subClass, writable: true, configurable: true } }); if (superClass) _setPrototypeOf(subClass, superClass); }

function _setPrototypeOf(o, p) { _setPrototypeOf = Object.setPrototypeOf || function _setPrototypeOf(o, p) { o.__proto__ = p; return o; }; return _setPrototypeOf(o, p); }

function _createSuper(Derived) { var hasNativeReflectConstruct = _isNativeReflectConstruct(); return function () { var Super = _getPrototypeOf(Derived), result; if (hasNativeReflectConstruct) { var NewTarget = _getPrototypeOf(this).constructor; result = Reflect.construct(Super, arguments, NewTarget); } else { result = Super.apply(this, arguments); } return _possibleConstructorReturn(this, result); }; }

function _possibleConstructorReturn(self, call) { if (call && (_typeof(call) === "object" || typeof call === "function")) { return call; } return _assertThisInitialized(self); }

function _assertThisInitialized(self) { if (self === void 0) { throw new ReferenceError("this hasn't been initialised - super() hasn't been called"); } return self; }

function _isNativeReflectConstruct() { if (typeof Reflect === "undefined" || !Reflect.construct) return false; if (Reflect.construct.sham) return false; if (typeof Proxy === "function") return true; try { Date.prototype.toString.call(Reflect.construct(Date, [], function () {})); return true; } catch (e) { return false; } }

function _getPrototypeOf(o) { _getPrototypeOf = Object.setPrototypeOf ? Object.getPrototypeOf : function _getPrototypeOf(o) { return o.__proto__ || Object.getPrototypeOf(o); }; return _getPrototypeOf(o); }

var Email = /*#__PURE__*/function (_BaseElement) {
  _inherits(Email, _BaseElement);

  var _super = _createSuper(Email);

  function Email(key, label, placeholder) {
    var _this;

    var uniqueId = arguments.length > 3 && arguments[3] !== undefined ? arguments[3] : undefined;

    _classCallCheck(this, Email);

    _this = _super.call(this, key, label);
    _this.type = "Email";
    _this.typePrefix = "eml";
    _this.placeholder = placeholder;
    _this.base = _this.init();
    _this.fromSchema = false;

    if (uniqueId !== undefined) {
      _this.uniqueId = uniqueId;
      _this.fromSchema = true;
    }

    return _this;
  }

  _createClass(Email, [{
    key: "create",
    value: function create() {
      var element = document.createElement("input");
      if (this.fromSchema) element.setAttribute("id", this.uniqueId);else element.setAttribute("id", "".concat(this.typePrefix, "_").concat(this.uniqueId));
      element.setAttribute("type", "email");
      element.setAttribute("class", "form-control");
      element.setAttribute("placeholder", "".concat(this.placeholder));
      element.setAttribute("data-key", this.key);
      this.base.appendChild(element);
      return this.base;
    }
  }, {
    key: "print",
    value: function print(data) {
      var element = document.createElement("p");
      if (this.fromSchema) element.setAttribute("id", this.uniqueId);else element.setAttribute("id", "".concat(this.typePrefix, "_").concat(this.uniqueId));
      element.setAttribute("class", "info-data");
      element.setAttribute("data-key", this.key);

      if (data == "") {
        element.innerHTML = "&nbsp;";
      } else element.innerText = data;

      this.base.appendChild(element);
      return this.base;
    }
  }]);

  return Email;
}(_baseElement.BaseElement);

exports.Email = Email;

},{"./baseElement":5}],10:[function(require,module,exports){
"use strict";

Object.defineProperty(exports, "__esModule", {
  value: true
});
exports.FormContainer = void 0;

var _baseContainer = require("./baseContainer");

function _typeof(obj) { "@babel/helpers - typeof"; if (typeof Symbol === "function" && typeof Symbol.iterator === "symbol") { _typeof = function _typeof(obj) { return typeof obj; }; } else { _typeof = function _typeof(obj) { return obj && typeof Symbol === "function" && obj.constructor === Symbol && obj !== Symbol.prototype ? "symbol" : typeof obj; }; } return _typeof(obj); }

function _classCallCheck(instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError("Cannot call a class as a function"); } }

function _defineProperties(target, props) { for (var i = 0; i < props.length; i++) { var descriptor = props[i]; descriptor.enumerable = descriptor.enumerable || false; descriptor.configurable = true; if ("value" in descriptor) descriptor.writable = true; Object.defineProperty(target, descriptor.key, descriptor); } }

function _createClass(Constructor, protoProps, staticProps) { if (protoProps) _defineProperties(Constructor.prototype, protoProps); if (staticProps) _defineProperties(Constructor, staticProps); return Constructor; }

function _inherits(subClass, superClass) { if (typeof superClass !== "function" && superClass !== null) { throw new TypeError("Super expression must either be null or a function"); } subClass.prototype = Object.create(superClass && superClass.prototype, { constructor: { value: subClass, writable: true, configurable: true } }); if (superClass) _setPrototypeOf(subClass, superClass); }

function _setPrototypeOf(o, p) { _setPrototypeOf = Object.setPrototypeOf || function _setPrototypeOf(o, p) { o.__proto__ = p; return o; }; return _setPrototypeOf(o, p); }

function _createSuper(Derived) { var hasNativeReflectConstruct = _isNativeReflectConstruct(); return function () { var Super = _getPrototypeOf(Derived), result; if (hasNativeReflectConstruct) { var NewTarget = _getPrototypeOf(this).constructor; result = Reflect.construct(Super, arguments, NewTarget); } else { result = Super.apply(this, arguments); } return _possibleConstructorReturn(this, result); }; }

function _possibleConstructorReturn(self, call) { if (call && (_typeof(call) === "object" || typeof call === "function")) { return call; } return _assertThisInitialized(self); }

function _assertThisInitialized(self) { if (self === void 0) { throw new ReferenceError("this hasn't been initialised - super() hasn't been called"); } return self; }

function _isNativeReflectConstruct() { if (typeof Reflect === "undefined" || !Reflect.construct) return false; if (Reflect.construct.sham) return false; if (typeof Proxy === "function") return true; try { Date.prototype.toString.call(Reflect.construct(Date, [], function () {})); return true; } catch (e) { return false; } }

function _getPrototypeOf(o) { _getPrototypeOf = Object.setPrototypeOf ? Object.getPrototypeOf : function _getPrototypeOf(o) { return o.__proto__ || Object.getPrototypeOf(o); }; return _getPrototypeOf(o); }

var FormContainer = /*#__PURE__*/function (_BaseContainer) {
  _inherits(FormContainer, _BaseContainer);

  var _super = _createSuper(FormContainer);

  function FormContainer(name, version) {
    var _this;

    var uniqueId = arguments.length > 2 && arguments[2] !== undefined ? arguments[2] : undefined;

    _classCallCheck(this, FormContainer);

    _this = _super.call(this);
    _this.name = name;
    _this.version = version;
    _this.type = "form";
    _this.typePrefix = "frm";
    _this.fromSchema = false;

    if (uniqueId !== undefined) {
      _this.uniqueId = uniqueId;
      _this.fromSchema = true;
    }

    return _this;
  }

  _createClass(FormContainer, [{
    key: "create",
    value: function create() {
      var element = document.createElement("div");
      if (this.fromSchema) element.setAttribute("id", this.uniqueId);else element.setAttribute("id", "".concat(this.typePrefix, "_").concat(this.uniqueId));
      element.setAttribute("class", "form-body");
      element.setAttribute("data-name", this.name);
      element.setAttribute("data-version", this.version);
      return element;
    }
  }]);

  return FormContainer;
}(_baseContainer.BaseContainer);

exports.FormContainer = FormContainer;

},{"./baseContainer":4}],11:[function(require,module,exports){
"use strict";

Object.defineProperty(exports, "__esModule", {
  value: true
});
exports.GroupCheckbox = void 0;

var _baseElement = require("./baseElement");

function _typeof(obj) { "@babel/helpers - typeof"; if (typeof Symbol === "function" && typeof Symbol.iterator === "symbol") { _typeof = function _typeof(obj) { return typeof obj; }; } else { _typeof = function _typeof(obj) { return obj && typeof Symbol === "function" && obj.constructor === Symbol && obj !== Symbol.prototype ? "symbol" : typeof obj; }; } return _typeof(obj); }

function _classCallCheck(instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError("Cannot call a class as a function"); } }

function _defineProperties(target, props) { for (var i = 0; i < props.length; i++) { var descriptor = props[i]; descriptor.enumerable = descriptor.enumerable || false; descriptor.configurable = true; if ("value" in descriptor) descriptor.writable = true; Object.defineProperty(target, descriptor.key, descriptor); } }

function _createClass(Constructor, protoProps, staticProps) { if (protoProps) _defineProperties(Constructor.prototype, protoProps); if (staticProps) _defineProperties(Constructor, staticProps); return Constructor; }

function _inherits(subClass, superClass) { if (typeof superClass !== "function" && superClass !== null) { throw new TypeError("Super expression must either be null or a function"); } subClass.prototype = Object.create(superClass && superClass.prototype, { constructor: { value: subClass, writable: true, configurable: true } }); if (superClass) _setPrototypeOf(subClass, superClass); }

function _setPrototypeOf(o, p) { _setPrototypeOf = Object.setPrototypeOf || function _setPrototypeOf(o, p) { o.__proto__ = p; return o; }; return _setPrototypeOf(o, p); }

function _createSuper(Derived) { var hasNativeReflectConstruct = _isNativeReflectConstruct(); return function () { var Super = _getPrototypeOf(Derived), result; if (hasNativeReflectConstruct) { var NewTarget = _getPrototypeOf(this).constructor; result = Reflect.construct(Super, arguments, NewTarget); } else { result = Super.apply(this, arguments); } return _possibleConstructorReturn(this, result); }; }

function _possibleConstructorReturn(self, call) { if (call && (_typeof(call) === "object" || typeof call === "function")) { return call; } return _assertThisInitialized(self); }

function _assertThisInitialized(self) { if (self === void 0) { throw new ReferenceError("this hasn't been initialised - super() hasn't been called"); } return self; }

function _isNativeReflectConstruct() { if (typeof Reflect === "undefined" || !Reflect.construct) return false; if (Reflect.construct.sham) return false; if (typeof Proxy === "function") return true; try { Date.prototype.toString.call(Reflect.construct(Date, [], function () {})); return true; } catch (e) { return false; } }

function _getPrototypeOf(o) { _getPrototypeOf = Object.setPrototypeOf ? Object.getPrototypeOf : function _getPrototypeOf(o) { return o.__proto__ || Object.getPrototypeOf(o); }; return _getPrototypeOf(o); }

var GroupCheckbox = /*#__PURE__*/function (_BaseElement) {
  _inherits(GroupCheckbox, _BaseElement);

  var _super = _createSuper(GroupCheckbox);

  function GroupCheckbox(key, label) {
    var _this;

    var values = arguments.length > 2 && arguments[2] !== undefined ? arguments[2] : [];
    var uniqueId = arguments.length > 3 && arguments[3] !== undefined ? arguments[3] : undefined;

    _classCallCheck(this, GroupCheckbox);

    _this = _super.call(this, label);
    _this.type = "GroupCheckbox";
    _this.typePrefix = "grp";
    _this.values = values;
    _this.base = _this.init();
    _this.key = key;
    _this.fromSchema = false;

    if (uniqueId !== undefined) {
      _this.uniqueId = uniqueId;
      _this.fromSchema = true;
    }

    return _this;
  }

  _createClass(GroupCheckbox, [{
    key: "create",
    value: function create() {
      var _this2 = this;

      this.values.forEach(function (item, index) {
        var label = item.label,
            value = item.value;
        var baseDiv = document.createElement("div"); //   baseDiv.setAttribute("class", "custom-control custom-checkbox");

        if (_this2.fromSchema) baseDiv.setAttribute("id", "".concat(_this2.uniqueId, "_").concat(index + _this2.values.length + 1000));else baseDiv.setAttribute("id", "".concat(_this2.typePrefix, "_").concat(_this2.uniqueId, "_").concat(index + _this2.values.length + 1000));
        baseDiv.setAttribute("component-type", _this2.type);
        var datagroup = "";
        if (_this2.fromSchema) datagroup = _this2.uniqueId;else datagroup = "".concat(_this2.typePrefix, "_").concat(_this2.uniqueId);
        console.log(_this2.key);
        var checkInput = document.createElement("input");
        var id = "".concat(datagroup, "_").concat(value);
        checkInput.setAttribute("id", id);
        checkInput.setAttribute("type", "checkbox");
        checkInput.setAttribute("data-key", _this2.key);
        checkInput.setAttribute("data-value", value);
        checkInput.setAttribute("data-group", datagroup);
        var text = document.createElement("label");
        text.setAttribute("class", "checkbox-label");
        text.setAttribute("for", id);
        text.innerText = label;
        baseDiv.appendChild(checkInput);
        baseDiv.appendChild(text);

        _this2.base.appendChild(baseDiv);
      });
      return this.base;
    }
  }, {
    key: "print",
    value: function print(data) {
      var _this3 = this;

      var datagroup = "";
      if (this.fromSchema) datagroup = this.uniqueId;else datagroup = "".concat(this.typePrefix, "_").concat(this.uniqueId);
      this.values.forEach(function (item) {
        var label = item.label,
            value = item.value;
        var baseDiv = document.createElement("div"); //   baseDiv.setAttribute("class", "custom-control custom-checkbox");

        if (_this3.fromSchema) baseDiv.setAttribute("id", "".concat(_this3.uniqueId, "_").concat(index + _this3.values.length + 1000));else baseDiv.setAttribute("id", "".concat(_this3.typePrefix, "_").concat(_this3.uniqueId, "_").concat(index + _this3.values.length + 1000));
        baseDiv.setAttribute("component-type", _this3.type);
        var checkInput = document.createElement("input");
        var id = "".concat(datagroup, "_").concat(value);
        checkInput.setAttribute("id", id);
        checkInput.setAttribute("type", "checkbox");
        checkInput.setAttribute("class", "custom-control-input");
        checkInput.setAttribute("data-key", _this3.key);
        checkInput.setAttribute("data-value", value);
        checkInput.setAttribute("disabled", "disabled");
        if (data.some(function (d) {
          return d == value;
        })) checkInput.setAttribute("checked", "true");
        var text = document.createElement("label");
        if (_this3.fromSchema) text.setAttribute("for", _this3.uniqueId);else text.setAttribute("for", "".concat(_this3.typePrefix, "_").concat(_this3.uniqueId));
        text.innerText = label;
        text.setAttribute("class", "custom-control-label");
        baseDiv.appendChild(checkInput);
        baseDiv.appendChild(text);

        _this3.base.appendChild(baseDiv);
      });
      return this.base;
    }
  }]);

  return GroupCheckbox;
}(_baseElement.BaseElement);

exports.GroupCheckbox = GroupCheckbox;

},{"./baseElement":5}],12:[function(require,module,exports){
"use strict";

Object.defineProperty(exports, "__esModule", {
  value: true
});
exports.Number = void 0;

var _baseElement = require("./baseElement");

function _typeof(obj) { "@babel/helpers - typeof"; if (typeof Symbol === "function" && typeof Symbol.iterator === "symbol") { _typeof = function _typeof(obj) { return typeof obj; }; } else { _typeof = function _typeof(obj) { return obj && typeof Symbol === "function" && obj.constructor === Symbol && obj !== Symbol.prototype ? "symbol" : typeof obj; }; } return _typeof(obj); }

function _classCallCheck(instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError("Cannot call a class as a function"); } }

function _defineProperties(target, props) { for (var i = 0; i < props.length; i++) { var descriptor = props[i]; descriptor.enumerable = descriptor.enumerable || false; descriptor.configurable = true; if ("value" in descriptor) descriptor.writable = true; Object.defineProperty(target, descriptor.key, descriptor); } }

function _createClass(Constructor, protoProps, staticProps) { if (protoProps) _defineProperties(Constructor.prototype, protoProps); if (staticProps) _defineProperties(Constructor, staticProps); return Constructor; }

function _inherits(subClass, superClass) { if (typeof superClass !== "function" && superClass !== null) { throw new TypeError("Super expression must either be null or a function"); } subClass.prototype = Object.create(superClass && superClass.prototype, { constructor: { value: subClass, writable: true, configurable: true } }); if (superClass) _setPrototypeOf(subClass, superClass); }

function _setPrototypeOf(o, p) { _setPrototypeOf = Object.setPrototypeOf || function _setPrototypeOf(o, p) { o.__proto__ = p; return o; }; return _setPrototypeOf(o, p); }

function _createSuper(Derived) { var hasNativeReflectConstruct = _isNativeReflectConstruct(); return function () { var Super = _getPrototypeOf(Derived), result; if (hasNativeReflectConstruct) { var NewTarget = _getPrototypeOf(this).constructor; result = Reflect.construct(Super, arguments, NewTarget); } else { result = Super.apply(this, arguments); } return _possibleConstructorReturn(this, result); }; }

function _possibleConstructorReturn(self, call) { if (call && (_typeof(call) === "object" || typeof call === "function")) { return call; } return _assertThisInitialized(self); }

function _assertThisInitialized(self) { if (self === void 0) { throw new ReferenceError("this hasn't been initialised - super() hasn't been called"); } return self; }

function _isNativeReflectConstruct() { if (typeof Reflect === "undefined" || !Reflect.construct) return false; if (Reflect.construct.sham) return false; if (typeof Proxy === "function") return true; try { Date.prototype.toString.call(Reflect.construct(Date, [], function () {})); return true; } catch (e) { return false; } }

function _getPrototypeOf(o) { _getPrototypeOf = Object.setPrototypeOf ? Object.getPrototypeOf : function _getPrototypeOf(o) { return o.__proto__ || Object.getPrototypeOf(o); }; return _getPrototypeOf(o); }

var Number = /*#__PURE__*/function (_BaseElement) {
  _inherits(Number, _BaseElement);

  var _super = _createSuper(Number);

  function Number(key, label, placeholder) {
    var _this;

    var uniqueId = arguments.length > 3 && arguments[3] !== undefined ? arguments[3] : undefined;

    _classCallCheck(this, Number);

    _this = _super.call(this, label);
    _this.key = key;
    _this.type = "Number";
    _this.typePrefix = "num";
    _this.base = _this.init();
    _this.placeholder = placeholder;
    _this.fromSchema = false;

    if (uniqueId !== undefined) {
      _this.uniqueId = uniqueId;
      _this.fromSchema = true;
    }

    return _this;
  }

  _createClass(Number, [{
    key: "create",
    value: function create() {
      var element = document.createElement("input");
      if (this.fromSchema) element.setAttribute("id", this.uniqueId);else element.setAttribute("id", "".concat(this.typePrefix, "_").concat(this.uniqueId));
      element.setAttribute("type", "number");
      element.setAttribute("class", "form-control");
      element.setAttribute("placeholder", "".concat(this.placeholder));
      element.setAttribute("data-key", this.key);
      this.base.appendChild(element);
      return this.base;
    }
  }, {
    key: "print",
    value: function print(data) {
      var element = document.createElement("p");
      if (this.fromSchema) element.setAttribute("id", this.uniqueId);else element.setAttribute("id", "".concat(this.typePrefix, "_").concat(this.uniqueId));
      element.setAttribute("class", "info-data");
      element.setAttribute("data-key", this.key);

      if (data == "") {
        element.innerHTML = "&nbsp;";
      } else element.innerText = data;

      this.base.appendChild(element);
      return this.base;
    }
  }]);

  return Number;
}(_baseElement.BaseElement);

exports.Number = Number;

},{"./baseElement":5}],13:[function(require,module,exports){
"use strict";

Object.defineProperty(exports, "__esModule", {
  value: true
});
exports.Password = void 0;

var _baseElement = require("./baseElement");

function _typeof(obj) { "@babel/helpers - typeof"; if (typeof Symbol === "function" && typeof Symbol.iterator === "symbol") { _typeof = function _typeof(obj) { return typeof obj; }; } else { _typeof = function _typeof(obj) { return obj && typeof Symbol === "function" && obj.constructor === Symbol && obj !== Symbol.prototype ? "symbol" : typeof obj; }; } return _typeof(obj); }

function _classCallCheck(instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError("Cannot call a class as a function"); } }

function _defineProperties(target, props) { for (var i = 0; i < props.length; i++) { var descriptor = props[i]; descriptor.enumerable = descriptor.enumerable || false; descriptor.configurable = true; if ("value" in descriptor) descriptor.writable = true; Object.defineProperty(target, descriptor.key, descriptor); } }

function _createClass(Constructor, protoProps, staticProps) { if (protoProps) _defineProperties(Constructor.prototype, protoProps); if (staticProps) _defineProperties(Constructor, staticProps); return Constructor; }

function _inherits(subClass, superClass) { if (typeof superClass !== "function" && superClass !== null) { throw new TypeError("Super expression must either be null or a function"); } subClass.prototype = Object.create(superClass && superClass.prototype, { constructor: { value: subClass, writable: true, configurable: true } }); if (superClass) _setPrototypeOf(subClass, superClass); }

function _setPrototypeOf(o, p) { _setPrototypeOf = Object.setPrototypeOf || function _setPrototypeOf(o, p) { o.__proto__ = p; return o; }; return _setPrototypeOf(o, p); }

function _createSuper(Derived) { var hasNativeReflectConstruct = _isNativeReflectConstruct(); return function () { var Super = _getPrototypeOf(Derived), result; if (hasNativeReflectConstruct) { var NewTarget = _getPrototypeOf(this).constructor; result = Reflect.construct(Super, arguments, NewTarget); } else { result = Super.apply(this, arguments); } return _possibleConstructorReturn(this, result); }; }

function _possibleConstructorReturn(self, call) { if (call && (_typeof(call) === "object" || typeof call === "function")) { return call; } return _assertThisInitialized(self); }

function _assertThisInitialized(self) { if (self === void 0) { throw new ReferenceError("this hasn't been initialised - super() hasn't been called"); } return self; }

function _isNativeReflectConstruct() { if (typeof Reflect === "undefined" || !Reflect.construct) return false; if (Reflect.construct.sham) return false; if (typeof Proxy === "function") return true; try { Date.prototype.toString.call(Reflect.construct(Date, [], function () {})); return true; } catch (e) { return false; } }

function _getPrototypeOf(o) { _getPrototypeOf = Object.setPrototypeOf ? Object.getPrototypeOf : function _getPrototypeOf(o) { return o.__proto__ || Object.getPrototypeOf(o); }; return _getPrototypeOf(o); }

var Password = /*#__PURE__*/function (_BaseElement) {
  _inherits(Password, _BaseElement);

  var _super = _createSuper(Password);

  function Password(key, label, placeholder) {
    var _this;

    var uniqueId = arguments.length > 3 && arguments[3] !== undefined ? arguments[3] : undefined;

    _classCallCheck(this, Password);

    _this = _super.call(this, key, label);
    _this.type = "Password";
    _this.typePrefix = "pas";
    _this.placeholder = placeholder;
    _this.base = _this.init();
    _this.fromSchema = false;

    if (uniqueId !== undefined) {
      _this.uniqueId = uniqueId;
      _this.fromSchema = true;
    }

    return _this;
  }

  _createClass(Password, [{
    key: "create",
    value: function create() {
      var element = document.createElement("input");
      if (this.fromSchema) element.setAttribute("id", this.uniqueId);else element.setAttribute("id", "".concat(this.typePrefix, "_").concat(this.uniqueId));
      element.setAttribute("type", "password");
      element.setAttribute("class", "form-control");
      element.setAttribute("placeholder", "".concat(this.placeholder));
      element.setAttribute("data-key", this.key);
      this.base.appendChild(element);
      return this.base;
    }
  }, {
    key: "Print",
    value: function Print(data) {
      var element = document.createElement("input");
      if (this.fromSchema) element.setAttribute("id", this.uniqueId);else element.setAttribute("id", "".concat(this.typePrefix, "_").concat(this.uniqueId));
      element.setAttribute("type", "password");
      element.setAttribute("class", "form-control");
      element.setAttribute("placeholder", "".concat(this.placeholder));
      element.setAttribute("data-key", this.key);
      element.value = data;
      this.base.appendChild(element);
      return this.base;
    }
  }]);

  return Password;
}(_baseElement.BaseElement);

exports.Password = Password;

},{"./baseElement":5}],14:[function(require,module,exports){
"use strict";

Object.defineProperty(exports, "__esModule", {
  value: true
});
exports.RadioBox = void 0;

function _classCallCheck(instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError("Cannot call a class as a function"); } }

function _defineProperties(target, props) { for (var i = 0; i < props.length; i++) { var descriptor = props[i]; descriptor.enumerable = descriptor.enumerable || false; descriptor.configurable = true; if ("value" in descriptor) descriptor.writable = true; Object.defineProperty(target, descriptor.key, descriptor); } }

function _createClass(Constructor, protoProps, staticProps) { if (protoProps) _defineProperties(Constructor.prototype, protoProps); if (staticProps) _defineProperties(Constructor, staticProps); return Constructor; }

var RadioBox = /*#__PURE__*/function () {
  function RadioBox(key, label) {
    var values = arguments.length > 2 && arguments[2] !== undefined ? arguments[2] : [];
    var uniqueId = arguments.length > 3 && arguments[3] !== undefined ? arguments[3] : undefined;

    _classCallCheck(this, RadioBox);

    this.key = key;
    this.type = "RadioBox";
    this.typePrefix = "rdo";
    this.values = values;
    this.label = label;
    this.fromSchema = false;

    if (uniqueId !== undefined) {
      this.uniqueId = uniqueId;
      this.fromSchema = true;
    } else this.uniqueId = "rdg_".concat(this.generateUniqueId());

    this.base = this.init();
  }

  _createClass(RadioBox, [{
    key: "generateUniqueId",
    value: function generateUniqueId() {
      return "xxxxxxxxxxxx".replace(/[xy]/g, function (c) {
        var r = Math.random() * 16 | 0,
            v = c === "x" ? r : r & 0x3 | 0x8;
        return v.toString(16);
      });
    }
  }, {
    key: "init",
    value: function init() {
      var baseDiv = document.createElement("div");
      baseDiv.setAttribute("class", "form-group");
      baseDiv.setAttribute("id", this.uniqueId);
      baseDiv.setAttribute("component-type", this.type);
      baseDiv.setAttribute("data-key", this.key);
      var labelElement = document.createElement("label");
      labelElement.setAttribute("id", "lbl_".concat(this.uniqueId));
      labelElement.innerText = this.label;
      baseDiv.appendChild(labelElement);
      return baseDiv;
    }
  }, {
    key: "create",
    value: function create() {
      var _this = this;

      this.values.forEach(function (item) {
        var labelTxt = item.label,
            value = item.value;
        var baseDiv = document.createElement("div");
        baseDiv.setAttribute("class", "custom-control custom-radio custom-control-inline");
        var uniqueId = "".concat(_this.typePrefix, "_").concat(_this.generateUniqueId());
        var label = document.createElement("label");
        label.innerHTML = labelTxt;
        label.setAttribute("class", "custom-control-label");
        label.setAttribute("for", uniqueId);
        var radioInput = document.createElement("input");
        radioInput.setAttribute("id", uniqueId);
        radioInput.setAttribute("type", "radio");
        radioInput.setAttribute("name", _this.key);
        radioInput.setAttribute("value", value);
        radioInput.setAttribute("class", "custom-control-input");
        baseDiv.appendChild(radioInput);
        baseDiv.appendChild(label);

        _this.base.appendChild(baseDiv);
      });
      return this.base;
    }
  }, {
    key: "print",
    value: function print(data) {
      var _this2 = this;

      this.values.forEach(function (item) {
        var labelTxt = item.label,
            value = item.value;
        var baseDiv = document.createElement("div");
        baseDiv.setAttribute("class", "custom-control custom-radio custom-control-inline");
        var uniqueId = "".concat(_this2.typePrefix, "_").concat(_this2.generateUniqueId());
        var label = document.createElement("label");
        label.innerHTML = labelTxt;
        label.setAttribute("class", "custom-control-label");
        label.setAttribute("for", uniqueId);
        var radioInput = document.createElement("input");
        radioInput.setAttribute("id", uniqueId);
        radioInput.setAttribute("type", "radio");
        radioInput.setAttribute("name", "rdo-".concat(_this2.key));
        radioInput.setAttribute("value", value);
        radioInput.setAttribute("class", "custom-control-input");
        radioInput.setAttribute("disabled", "disabled");

        if (value == data) {
          radioInput.setAttribute("checked", "true");
        }

        baseDiv.appendChild(radioInput);
        baseDiv.appendChild(label);

        _this2.base.appendChild(baseDiv);
      });
      return this.base;
    }
  }]);

  return RadioBox;
}();

exports.RadioBox = RadioBox;

},{}],15:[function(require,module,exports){
"use strict";

Object.defineProperty(exports, "__esModule", {
  value: true
});
exports.Row = void 0;

var _baseContainer = require("./baseContainer");

function _typeof(obj) { "@babel/helpers - typeof"; if (typeof Symbol === "function" && typeof Symbol.iterator === "symbol") { _typeof = function _typeof(obj) { return typeof obj; }; } else { _typeof = function _typeof(obj) { return obj && typeof Symbol === "function" && obj.constructor === Symbol && obj !== Symbol.prototype ? "symbol" : typeof obj; }; } return _typeof(obj); }

function _classCallCheck(instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError("Cannot call a class as a function"); } }

function _defineProperties(target, props) { for (var i = 0; i < props.length; i++) { var descriptor = props[i]; descriptor.enumerable = descriptor.enumerable || false; descriptor.configurable = true; if ("value" in descriptor) descriptor.writable = true; Object.defineProperty(target, descriptor.key, descriptor); } }

function _createClass(Constructor, protoProps, staticProps) { if (protoProps) _defineProperties(Constructor.prototype, protoProps); if (staticProps) _defineProperties(Constructor, staticProps); return Constructor; }

function _inherits(subClass, superClass) { if (typeof superClass !== "function" && superClass !== null) { throw new TypeError("Super expression must either be null or a function"); } subClass.prototype = Object.create(superClass && superClass.prototype, { constructor: { value: subClass, writable: true, configurable: true } }); if (superClass) _setPrototypeOf(subClass, superClass); }

function _setPrototypeOf(o, p) { _setPrototypeOf = Object.setPrototypeOf || function _setPrototypeOf(o, p) { o.__proto__ = p; return o; }; return _setPrototypeOf(o, p); }

function _createSuper(Derived) { var hasNativeReflectConstruct = _isNativeReflectConstruct(); return function () { var Super = _getPrototypeOf(Derived), result; if (hasNativeReflectConstruct) { var NewTarget = _getPrototypeOf(this).constructor; result = Reflect.construct(Super, arguments, NewTarget); } else { result = Super.apply(this, arguments); } return _possibleConstructorReturn(this, result); }; }

function _possibleConstructorReturn(self, call) { if (call && (_typeof(call) === "object" || typeof call === "function")) { return call; } return _assertThisInitialized(self); }

function _assertThisInitialized(self) { if (self === void 0) { throw new ReferenceError("this hasn't been initialised - super() hasn't been called"); } return self; }

function _isNativeReflectConstruct() { if (typeof Reflect === "undefined" || !Reflect.construct) return false; if (Reflect.construct.sham) return false; if (typeof Proxy === "function") return true; try { Date.prototype.toString.call(Reflect.construct(Date, [], function () {})); return true; } catch (e) { return false; } }

function _getPrototypeOf(o) { _getPrototypeOf = Object.setPrototypeOf ? Object.getPrototypeOf : function _getPrototypeOf(o) { return o.__proto__ || Object.getPrototypeOf(o); }; return _getPrototypeOf(o); }

var Row = /*#__PURE__*/function (_BaseContainer) {
  _inherits(Row, _BaseContainer);

  var _super = _createSuper(Row);

  function Row(key) {
    var _this;

    var uniqueId = arguments.length > 1 && arguments[1] !== undefined ? arguments[1] : undefined;

    _classCallCheck(this, Row);

    _this = _super.call(this);
    _this.key = key;
    _this.type = "Row";
    _this.typePrefix = "rw";
    _this.fromSchema = false;

    if (uniqueId !== undefined) {
      _this.uniqueId = uniqueId;
      _this.fromSchema = true;
    }

    return _this;
  }

  _createClass(Row, [{
    key: "create",
    value: function create() {
      var element = document.createElement("div");
      if (this.fromSchema) element.setAttribute("id", this.uniqueId);else element.setAttribute("id", "".concat(this.typePrefix, "_").concat(this.uniqueId));
      element.setAttribute("class", "row");
      element.setAttribute("data-key", this.key);
      return element;
    }
  }]);

  return Row;
}(_baseContainer.BaseContainer);

exports.Row = Row;

},{"./baseContainer":4}],16:[function(require,module,exports){
"use strict";

Object.defineProperty(exports, "__esModule", {
  value: true
});
exports.Select = void 0;

var _baseElement = require("./baseElement");

function _typeof(obj) { "@babel/helpers - typeof"; if (typeof Symbol === "function" && typeof Symbol.iterator === "symbol") { _typeof = function _typeof(obj) { return typeof obj; }; } else { _typeof = function _typeof(obj) { return obj && typeof Symbol === "function" && obj.constructor === Symbol && obj !== Symbol.prototype ? "symbol" : typeof obj; }; } return _typeof(obj); }

function _classCallCheck(instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError("Cannot call a class as a function"); } }

function _defineProperties(target, props) { for (var i = 0; i < props.length; i++) { var descriptor = props[i]; descriptor.enumerable = descriptor.enumerable || false; descriptor.configurable = true; if ("value" in descriptor) descriptor.writable = true; Object.defineProperty(target, descriptor.key, descriptor); } }

function _createClass(Constructor, protoProps, staticProps) { if (protoProps) _defineProperties(Constructor.prototype, protoProps); if (staticProps) _defineProperties(Constructor, staticProps); return Constructor; }

function _inherits(subClass, superClass) { if (typeof superClass !== "function" && superClass !== null) { throw new TypeError("Super expression must either be null or a function"); } subClass.prototype = Object.create(superClass && superClass.prototype, { constructor: { value: subClass, writable: true, configurable: true } }); if (superClass) _setPrototypeOf(subClass, superClass); }

function _setPrototypeOf(o, p) { _setPrototypeOf = Object.setPrototypeOf || function _setPrototypeOf(o, p) { o.__proto__ = p; return o; }; return _setPrototypeOf(o, p); }

function _createSuper(Derived) { var hasNativeReflectConstruct = _isNativeReflectConstruct(); return function () { var Super = _getPrototypeOf(Derived), result; if (hasNativeReflectConstruct) { var NewTarget = _getPrototypeOf(this).constructor; result = Reflect.construct(Super, arguments, NewTarget); } else { result = Super.apply(this, arguments); } return _possibleConstructorReturn(this, result); }; }

function _possibleConstructorReturn(self, call) { if (call && (_typeof(call) === "object" || typeof call === "function")) { return call; } return _assertThisInitialized(self); }

function _assertThisInitialized(self) { if (self === void 0) { throw new ReferenceError("this hasn't been initialised - super() hasn't been called"); } return self; }

function _isNativeReflectConstruct() { if (typeof Reflect === "undefined" || !Reflect.construct) return false; if (Reflect.construct.sham) return false; if (typeof Proxy === "function") return true; try { Date.prototype.toString.call(Reflect.construct(Date, [], function () {})); return true; } catch (e) { return false; } }

function _getPrototypeOf(o) { _getPrototypeOf = Object.setPrototypeOf ? Object.getPrototypeOf : function _getPrototypeOf(o) { return o.__proto__ || Object.getPrototypeOf(o); }; return _getPrototypeOf(o); }

var Select = /*#__PURE__*/function (_BaseElement) {
  _inherits(Select, _BaseElement);

  var _super = _createSuper(Select);

  function Select(key, label) {
    var _this;

    var values = arguments.length > 2 && arguments[2] !== undefined ? arguments[2] : [];
    var uniqueId = arguments.length > 3 && arguments[3] !== undefined ? arguments[3] : undefined;

    _classCallCheck(this, Select);

    _this = _super.call(this, label);
    _this.key = key;
    _this.type = "Select";
    _this.typePrefix = "drp";
    _this.base = _this.init();
    _this.values = values;
    _this.fromSchema = false;

    if (uniqueId !== undefined) {
      _this.uniqueId = uniqueId;
      _this.fromSchema = true;
    }

    return _this;
  }

  _createClass(Select, [{
    key: "create",
    value: function create() {
      var element = document.createElement("select");
      if (this.fromSchema) element.setAttribute("id", this.uniqueId);else element.setAttribute("id", "".concat(this.typePrefix, "_").concat(this.uniqueId));
      element.setAttribute("class", "form-control");
      element.setAttribute("data-key", this.key);
      this.values.forEach(function (item) {
        var label = item.label,
            value = item.value;
        var child = document.createElement("option");
        child.setAttribute("value", value);
        child.innerText = label;
        element.appendChild(child);
      });
      this.base.appendChild(element);
      return this.base;
    }
  }, {
    key: "print",
    value: function print(data) {
      var element = document.createElement("p");
      if (this.fromSchema) element.setAttribute("id", this.uniqueId);else element.setAttribute("id", "".concat(this.typePrefix, "_").concat(this.uniqueId));
      element.setAttribute("class", "info-data");
      element.setAttribute("data-key", this.key);

      if (data == "") {
        element.innerHTML = "&nbsp;";
      } else {
        this.values.forEach(function (item) {
          var label = item.label,
              value = item.value;

          if (value == data) {
            element.innerText = label;
          }
        });
      }

      this.base.appendChild(element);
      return this.base;
    }
  }]);

  return Select;
}(_baseElement.BaseElement);

exports.Select = Select;

},{"./baseElement":5}],17:[function(require,module,exports){
"use strict";

Object.defineProperty(exports, "__esModule", {
  value: true
});
exports.TagsField = void 0;

var _baseElement = require("./baseElement");

function _typeof(obj) { "@babel/helpers - typeof"; if (typeof Symbol === "function" && typeof Symbol.iterator === "symbol") { _typeof = function _typeof(obj) { return typeof obj; }; } else { _typeof = function _typeof(obj) { return obj && typeof Symbol === "function" && obj.constructor === Symbol && obj !== Symbol.prototype ? "symbol" : typeof obj; }; } return _typeof(obj); }

function _classCallCheck(instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError("Cannot call a class as a function"); } }

function _defineProperties(target, props) { for (var i = 0; i < props.length; i++) { var descriptor = props[i]; descriptor.enumerable = descriptor.enumerable || false; descriptor.configurable = true; if ("value" in descriptor) descriptor.writable = true; Object.defineProperty(target, descriptor.key, descriptor); } }

function _createClass(Constructor, protoProps, staticProps) { if (protoProps) _defineProperties(Constructor.prototype, protoProps); if (staticProps) _defineProperties(Constructor, staticProps); return Constructor; }

function _inherits(subClass, superClass) { if (typeof superClass !== "function" && superClass !== null) { throw new TypeError("Super expression must either be null or a function"); } subClass.prototype = Object.create(superClass && superClass.prototype, { constructor: { value: subClass, writable: true, configurable: true } }); if (superClass) _setPrototypeOf(subClass, superClass); }

function _setPrototypeOf(o, p) { _setPrototypeOf = Object.setPrototypeOf || function _setPrototypeOf(o, p) { o.__proto__ = p; return o; }; return _setPrototypeOf(o, p); }

function _createSuper(Derived) { var hasNativeReflectConstruct = _isNativeReflectConstruct(); return function () { var Super = _getPrototypeOf(Derived), result; if (hasNativeReflectConstruct) { var NewTarget = _getPrototypeOf(this).constructor; result = Reflect.construct(Super, arguments, NewTarget); } else { result = Super.apply(this, arguments); } return _possibleConstructorReturn(this, result); }; }

function _possibleConstructorReturn(self, call) { if (call && (_typeof(call) === "object" || typeof call === "function")) { return call; } return _assertThisInitialized(self); }

function _assertThisInitialized(self) { if (self === void 0) { throw new ReferenceError("this hasn't been initialised - super() hasn't been called"); } return self; }

function _isNativeReflectConstruct() { if (typeof Reflect === "undefined" || !Reflect.construct) return false; if (Reflect.construct.sham) return false; if (typeof Proxy === "function") return true; try { Date.prototype.toString.call(Reflect.construct(Date, [], function () {})); return true; } catch (e) { return false; } }

function _getPrototypeOf(o) { _getPrototypeOf = Object.setPrototypeOf ? Object.getPrototypeOf : function _getPrototypeOf(o) { return o.__proto__ || Object.getPrototypeOf(o); }; return _getPrototypeOf(o); }

var TagsField = /*#__PURE__*/function (_BaseElement) {
  _inherits(TagsField, _BaseElement);

  var _super = _createSuper(TagsField);

  function TagsField(key, label, placeholder) {
    var _this;

    var uniqueId = arguments.length > 3 && arguments[3] !== undefined ? arguments[3] : undefined;

    _classCallCheck(this, TagsField);

    _this = _super.call(this, label);
    _this.key = key;
    _this.type = "TagsField";
    _this.typePrefix = "tgs";
    _this.placeholder = placeholder;
    _this.fromSchema = false;

    if (uniqueId !== undefined) {
      _this.uniqueId = uniqueId;
      _this.fromSchema = true;
    }

    _this.base = _this.init();
    return _this;
  }

  _createClass(TagsField, [{
    key: "create",
    value: function create() {
      var element = document.createElement("input");
      if (this.fromSchema) element.setAttribute("id", this.uniqueId);else element.setAttribute("id", "".concat(this.typePrefix, "_").concat(this.uniqueId));
      element.setAttribute("type", "text");
      element.setAttribute("class", "form-control tags");
      element.setAttribute("placeholder", "".concat(this.placeholder));
      element.setAttribute("data-key", this.key);
      this.base.appendChild(element);
      return this.base;
    }
  }, {
    key: "print",
    value: function print(data) {
      var element = document.createElement("p");
      if (this.fromSchema) element.setAttribute("id", this.uniqueId);else element.setAttribute("id", "".concat(this.typePrefix, "_").concat(this.uniqueId));
      element.setAttribute("class", "info-data");
      element.setAttribute("data-key", this.key);
      element.innerText = data;
      this.base.appendChild(element);
      return this.base;
    }
  }]);

  return TagsField;
}(_baseElement.BaseElement);

exports.TagsField = TagsField;

},{"./baseElement":5}],18:[function(require,module,exports){
"use strict";

Object.defineProperty(exports, "__esModule", {
  value: true
});
exports.TextArea = void 0;

var _baseElement = require("./baseElement");

function _typeof(obj) { "@babel/helpers - typeof"; if (typeof Symbol === "function" && typeof Symbol.iterator === "symbol") { _typeof = function _typeof(obj) { return typeof obj; }; } else { _typeof = function _typeof(obj) { return obj && typeof Symbol === "function" && obj.constructor === Symbol && obj !== Symbol.prototype ? "symbol" : typeof obj; }; } return _typeof(obj); }

function _classCallCheck(instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError("Cannot call a class as a function"); } }

function _defineProperties(target, props) { for (var i = 0; i < props.length; i++) { var descriptor = props[i]; descriptor.enumerable = descriptor.enumerable || false; descriptor.configurable = true; if ("value" in descriptor) descriptor.writable = true; Object.defineProperty(target, descriptor.key, descriptor); } }

function _createClass(Constructor, protoProps, staticProps) { if (protoProps) _defineProperties(Constructor.prototype, protoProps); if (staticProps) _defineProperties(Constructor, staticProps); return Constructor; }

function _inherits(subClass, superClass) { if (typeof superClass !== "function" && superClass !== null) { throw new TypeError("Super expression must either be null or a function"); } subClass.prototype = Object.create(superClass && superClass.prototype, { constructor: { value: subClass, writable: true, configurable: true } }); if (superClass) _setPrototypeOf(subClass, superClass); }

function _setPrototypeOf(o, p) { _setPrototypeOf = Object.setPrototypeOf || function _setPrototypeOf(o, p) { o.__proto__ = p; return o; }; return _setPrototypeOf(o, p); }

function _createSuper(Derived) { var hasNativeReflectConstruct = _isNativeReflectConstruct(); return function () { var Super = _getPrototypeOf(Derived), result; if (hasNativeReflectConstruct) { var NewTarget = _getPrototypeOf(this).constructor; result = Reflect.construct(Super, arguments, NewTarget); } else { result = Super.apply(this, arguments); } return _possibleConstructorReturn(this, result); }; }

function _possibleConstructorReturn(self, call) { if (call && (_typeof(call) === "object" || typeof call === "function")) { return call; } return _assertThisInitialized(self); }

function _assertThisInitialized(self) { if (self === void 0) { throw new ReferenceError("this hasn't been initialised - super() hasn't been called"); } return self; }

function _isNativeReflectConstruct() { if (typeof Reflect === "undefined" || !Reflect.construct) return false; if (Reflect.construct.sham) return false; if (typeof Proxy === "function") return true; try { Date.prototype.toString.call(Reflect.construct(Date, [], function () {})); return true; } catch (e) { return false; } }

function _getPrototypeOf(o) { _getPrototypeOf = Object.setPrototypeOf ? Object.getPrototypeOf : function _getPrototypeOf(o) { return o.__proto__ || Object.getPrototypeOf(o); }; return _getPrototypeOf(o); }

var TextArea = /*#__PURE__*/function (_BaseElement) {
  _inherits(TextArea, _BaseElement);

  var _super = _createSuper(TextArea);

  function TextArea(key, label, placeholder) {
    var _this;

    var uniqueId = arguments.length > 3 && arguments[3] !== undefined ? arguments[3] : undefined;

    _classCallCheck(this, TextArea);

    _this = _super.call(this, label);
    _this.key = key;
    _this.type = "TextArea";
    _this.typePrefix = "ter";
    _this.base = _this.init();
    _this.placeholder = placeholder;
    _this.fromSchema = false;

    if (uniqueId !== undefined) {
      _this.uniqueId = uniqueId;
      _this.fromSchema = true;
    }

    return _this;
  }

  _createClass(TextArea, [{
    key: "create",
    value: function create() {
      var element = document.createElement("textarea");
      if (this.fromSchema) element.setAttribute("id", this.uniqueId);else element.setAttribute("id", "".concat(this.typePrefix, "_").concat(this.uniqueId));
      element.setAttribute("class", "form-control");
      element.setAttribute("placeholder", "".concat(this.placeholder));
      element.setAttribute("data-key", this.key);
      this.base.appendChild(element);
      return this.base;
    }
  }, {
    key: "print",
    value: function print(data) {
      var element = document.createElement("p");
      if (this.fromSchema) element.setAttribute("id", this.uniqueId);else element.setAttribute("id", "".concat(this.typePrefix, "_").concat(this.uniqueId));
      element.setAttribute("class", "info-data");
      element.setAttribute("data-key", this.key);

      if (data == "") {
        element.innerHTML = "&nbsp;";
      } else element.innerText = data;

      this.base.appendChild(element);
      return this.base;
    }
  }]);

  return TextArea;
}(_baseElement.BaseElement);

exports.TextArea = TextArea;

},{"./baseElement":5}],19:[function(require,module,exports){
"use strict";

Object.defineProperty(exports, "__esModule", {
  value: true
});
exports.TextField = void 0;

var _baseElement = require("./baseElement");

function _typeof(obj) { "@babel/helpers - typeof"; if (typeof Symbol === "function" && typeof Symbol.iterator === "symbol") { _typeof = function _typeof(obj) { return typeof obj; }; } else { _typeof = function _typeof(obj) { return obj && typeof Symbol === "function" && obj.constructor === Symbol && obj !== Symbol.prototype ? "symbol" : typeof obj; }; } return _typeof(obj); }

function _classCallCheck(instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError("Cannot call a class as a function"); } }

function _defineProperties(target, props) { for (var i = 0; i < props.length; i++) { var descriptor = props[i]; descriptor.enumerable = descriptor.enumerable || false; descriptor.configurable = true; if ("value" in descriptor) descriptor.writable = true; Object.defineProperty(target, descriptor.key, descriptor); } }

function _createClass(Constructor, protoProps, staticProps) { if (protoProps) _defineProperties(Constructor.prototype, protoProps); if (staticProps) _defineProperties(Constructor, staticProps); return Constructor; }

function _inherits(subClass, superClass) { if (typeof superClass !== "function" && superClass !== null) { throw new TypeError("Super expression must either be null or a function"); } subClass.prototype = Object.create(superClass && superClass.prototype, { constructor: { value: subClass, writable: true, configurable: true } }); if (superClass) _setPrototypeOf(subClass, superClass); }

function _setPrototypeOf(o, p) { _setPrototypeOf = Object.setPrototypeOf || function _setPrototypeOf(o, p) { o.__proto__ = p; return o; }; return _setPrototypeOf(o, p); }

function _createSuper(Derived) { var hasNativeReflectConstruct = _isNativeReflectConstruct(); return function () { var Super = _getPrototypeOf(Derived), result; if (hasNativeReflectConstruct) { var NewTarget = _getPrototypeOf(this).constructor; result = Reflect.construct(Super, arguments, NewTarget); } else { result = Super.apply(this, arguments); } return _possibleConstructorReturn(this, result); }; }

function _possibleConstructorReturn(self, call) { if (call && (_typeof(call) === "object" || typeof call === "function")) { return call; } return _assertThisInitialized(self); }

function _assertThisInitialized(self) { if (self === void 0) { throw new ReferenceError("this hasn't been initialised - super() hasn't been called"); } return self; }

function _isNativeReflectConstruct() { if (typeof Reflect === "undefined" || !Reflect.construct) return false; if (Reflect.construct.sham) return false; if (typeof Proxy === "function") return true; try { Date.prototype.toString.call(Reflect.construct(Date, [], function () {})); return true; } catch (e) { return false; } }

function _getPrototypeOf(o) { _getPrototypeOf = Object.setPrototypeOf ? Object.getPrototypeOf : function _getPrototypeOf(o) { return o.__proto__ || Object.getPrototypeOf(o); }; return _getPrototypeOf(o); }

var TextField = /*#__PURE__*/function (_BaseElement) {
  _inherits(TextField, _BaseElement);

  var _super = _createSuper(TextField);

  function TextField(key, label, placeholder) {
    var _this;

    var uniqueId = arguments.length > 3 && arguments[3] !== undefined ? arguments[3] : undefined;

    _classCallCheck(this, TextField);

    _this = _super.call(this, label);
    _this.key = key;
    _this.type = "TextField";
    _this.typePrefix = "txt";
    _this.placeholder = placeholder;
    _this.fromSchema = false;

    if (uniqueId !== undefined) {
      _this.uniqueId = uniqueId;
      _this.fromSchema = true;
    }

    _this.base = _this.init();
    return _this;
  }

  _createClass(TextField, [{
    key: "create",
    value: function create() {
      var element = document.createElement("input");
      if (this.fromSchema) element.setAttribute("id", this.uniqueId);else element.setAttribute("id", "".concat(this.typePrefix, "_").concat(this.uniqueId));
      element.setAttribute("type", "text");
      element.setAttribute("class", "form-control");
      element.setAttribute("placeholder", "".concat(this.placeholder));
      element.setAttribute("data-key", this.key);
      this.base.appendChild(element);
      return this.base;
    }
  }, {
    key: "print",
    value: function print(data) {
      var element = document.createElement("p");
      if (this.fromSchema) element.setAttribute("id", this.uniqueId);else element.setAttribute("id", "".concat(this.typePrefix, "_").concat(this.uniqueId));
      element.setAttribute("class", "info-data");
      element.setAttribute("data-key", this.key);

      if (data == "") {
        element.innerHTML = "&nbsp;";
      } else element.innerText = data;

      this.base.appendChild(element);
      return this.base;
    }
  }]);

  return TextField;
}(_baseElement.BaseElement);

exports.TextField = TextField;

},{"./baseElement":5}]},{},[1]);
