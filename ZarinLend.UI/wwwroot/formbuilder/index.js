import { FormBuilder } from "./formBuilder";

$(function () {
  var connectionType = document.getElementById("ConnectionType");
  var connectionSubject = document.getElementById("connectionSubject");
  var connectionTemplate = document.getElementById("connectionTemplate");

  var opt = document.createElement("option");
  opt.text = "انتخاب کنید";
  opt.value = 0;
  connectionType.appendChild(opt);

  var fsub = document.createElement("option");
  fsub.text = "انتخاب کنید";
  fsub.value = 0;
  connectionSubject.appendChild(fsub);

  var tmp = document.createElement("option");
  tmp.text = "انتخاب کنید";
  tmp.value = 0;
  connectionTemplate.appendChild(tmp);

  var formBase = document.getElementById("form-base");
  var dataBag = {};
  var schema = {};

  var connectionTypesUrl = "/Api/GetConnectionTypes";
  var connectionSubjects = "/Api/GetConnectionSubjects?typeId=";
  var connectionTemplates = "/Api/GetConnectionTemplates?subjectId=";
  var templateUrl = "/Api/GetTemplate/";

  $.getJSON(connectionTypesUrl, function (data) {
    data.forEach(function (item, index) {
      let option = document.createElement("option");
      option.text = item.name;
      option.value = item.id;
      connectionType.appendChild(option);
    });
  });

  connectionType.addEventListener("change", function () {
    connectionSubject.innerHTML = "";
    connectionTemplate.innerHTML = "";

    let first = document.createElement("option");
    first.text = "انتخاب کنید";
    first.value = 0;
    connectionTemplate.appendChild(first);

    if (this.value !== 0) {
      var tmpConnectionSubjects = connectionSubjects + this.value;
      $.getJSON(tmpConnectionSubjects, function (data) {
        var first = document.createElement("option");
        first.text = "انتخاب کنید";
        first.value = 0;
        connectionSubject.appendChild(first);

        data.forEach(function (item, index) {
          let subject = document.createElement("option");
          subject.text = item.name;
          subject.value = item.id;
          connectionSubject.appendChild(subject);
        });
      });
    }
  });

  connectionSubject.addEventListener("change", function () {
    connectionTemplate.innerHTML = "";

    if (this.value !== 0) {
      var tmpConnectionTemplates = connectionTemplates + this.value;
      $.getJSON(tmpConnectionTemplates, function (data) {
        let first = document.createElement("option");
        first.text = "انتخاب کنید";
        first.value = 0;
        connectionTemplate.appendChild(first);

        data.forEach(function (item, index) {
          let subject = document.createElement("option");
          subject.text = item.name;
          subject.value = item.id;
          connectionTemplate.appendChild(subject);
        });
      });
    }
  });

  connectionTemplate.addEventListener("change", function () {
    if (this.value !== 0) {
      var tmpTemplateUrl = templateUrl + this.value;
      let title = document.getElementById("templateTitle");

      $.getJSON(tmpTemplateUrl, function (data) {
        if (data === undefined || data === null) console.warn("data is null.");
        else {
          title.innerText = data.name;
          schema = JSON.parse(data.structure);

          formBase.innerHTML = "";
          let builder = new FormBuilder(schema, formBase);
          let form = builder.render();

          formBase.appendChild(form.form);
          dataBag = form.data;

          bindeDate();
          bindTag();
        }
      });
    }
  });

  function bindeDate() {
    $(".datepicker").persianDatepicker({
      format: "YYYY/MM/DD",
    });
  }

  function bindTag() {
    $(".tags").tagManager();
  }

  function validateData(key, value, validations) {
    if (validations.length == 0) return true;

    let validate = true;
    let vMessage = "";

    for (let item of validations) {
      const { type, message } = item;
      console.log(type);
      switch (type) {
        case "Required":
          if (value == null || value.length == 0) {
            validate = false;
            vMessage = message;
          }
          break;
        case "Length":
          if (item.min != null) {
            if (value.length < item.min) {
              validate = false;
              vMessage = message;
            }
          }
          if (item.max != null) {
            if (value.length > item.max) {
              validate = false;
              vMessage = message;
            }
          }
          break;
      }

      if (validate == false) {
        break;
      }
    }

    if (validate == false) {
      $(`#${key}VlaidationSummery`).remove();
      if ($(`[data-key=${key}]`).hasClass()) {
        $(`[data-key=${key}]`).removeClass("is-invalid");
      }
      let validationSummery = ` <span id="${key}VlaidationSummery" class="text-danger error">${vMessage}</span>`;
      $(`[data-key=${key}]`).parent().append(validationSummery);
      $(`[data-key=${key}]`).addClass("is-invalid");
    } else {
      $(`[data-key=${key}]`).removeClass("is-invalid");
      $(`#${key}VlaidationSummery`).remove();
    }
    return validate;
  }

  $("#btnSend").on("click", function () {
    let data = {};
    let validate = true;

    dataBag.forEach(function (item) {
      const { key, type, validations } = item;

      switch (type) {
        case "TextField":
          {
            let value = $(`[data-key=${key}]`).val();
            data[key] = value;
          }
          break;
        case "Checkbox":
          {
            if ($(`[data-key=${key}]`).prop("checked")) data[key] = true;
            else data[key] = false;
          }
          break;
        case "Email":
          {
            let value = $(`[data-key=${key}]`).val();
            data[key] = value;
          }
          break;
        case "Number":
          {
            let value = $(`[data-key=${key}]`).val();
            data[key] = parsint(value);
          }
          break;
        case "Password":
          {
            let value = $(`[data-key=${key}]`).val();
            data[key] = value;
          }
          break;
        case "RadioBox":
          {
            let rdoValue = 0;
            $(`[name=${key}]`).each(function (index, element) {
              if (element.checked) {
                rdoValue = element.value;
              }
            });
            data[key] = rdoValue;
          }
          break;
        case "Select":
          {
            let value = $(`[data-key=${key}]`).val();
            
            if (value == "-1") {
              data[key] = null;
            } else {
              data[key] = value;
            }
          }
          break;
        case "TextArea":
          {
            let value = $(`[data-key=${key}]`).val();
            data[key] = value;
          }
          break;
        case "Date":
          {
            let value = $(`[data-key=${key}]`).val();
            data[key] = value;
          }
          break;
        case "GroupCheckbox":
          {
            let elements = document.querySelectorAll(`[data-key=${key}]`);
            let values = [];
            elements.forEach((item) => {
              console;
              let value = item.getAttribute("data-value");
              if (item.checked) values.push(value);
            });
            data[key] = values;
          }
          break;
        default:
          break;
      }
      validate &= validateData(key, data[key], validations);
    });

    if (validate == true) {
      let result = {};
      result.customerId = parseInt(document.getElementById("custId").value);
      result.employeeId = parseInt(document.getElementById("emplId").value);
      result.campaignId = parseInt(document.getElementById("campaignId").value);
      result.schema = JSON.stringify(schema);
      result.data = JSON.stringify(data);
      result.connectionType = parseInt(connectionType.value);
      result.connectionSubject = parseInt(connectionSubject.value);
      result.connectionTemplate = parseInt(connectionTemplate.value);

      sendData(result);
    } else {
      console.error("Invalid data");
    }
  });
});

function sendData(data) {
  let btn = document.getElementById("btnSend");
  btn.setAttribute("disabled", "disabled");
  
  var addConnectionUrl = "/Api/AddConnection";
  $.post(addConnectionUrl, data)
    .done(function (msg) {
      console.log(msg.message);
      window.location.replace("/Dashboard/customer/" + data.customerId);
    })
    .fail(function (xhr, status, error) {
      console.log(status, error);
    });
}
