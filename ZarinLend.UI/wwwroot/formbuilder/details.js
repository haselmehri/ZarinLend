import { FormBuilder } from "./formBuilder";

$(function () {
  let connectionType = document.getElementById("ConnectionType");
  let connectionSubject = document.getElementById("connectionSubject");
  let connectionTemplate = document.getElementById("connectionTemplate");

  let title = document.getElementById("templateTitle");

  let schema = JSON.parse(document.getElementById("schema").value);
  let data = JSON.parse(document.getElementById("data").value);

  let connectionTypeId = parseInt(
    document.getElementById("ConnectionTypeId").value
  );

  let connectionSubjectId = parseInt(
    document.getElementById("connectionSubjectId").value
  );

  let connectionTemplateId = parseInt(
    document.getElementById("connectionTemplateId").value
  );

  let formBase = document.getElementById("form-base");
  let connectionTypesUrl = "/Api/GetConnectionTypes";
  let connectionSubjectsUrl =
    "/Api/GetConnectionSubjects?typeId=" + connectionTypeId;
  let connectionTemplatesUrl =
    "/Api/GetConnectionTemplates?subjectId=" + connectionSubjectId;

  $.getJSON(connectionTypesUrl, function (data) {
    data.forEach(function (item, index) {
      let option = document.createElement("option");
      option.text = item.name;
      option.value = item.id;
      if (item.id == connectionTypeId) option.selected = true;
      connectionType.appendChild(option);
    });
  });

  $.getJSON(connectionSubjectsUrl, function (data) {
    data.forEach(function (item, index) {
      let option = document.createElement("option");
      option.text = item.name;
      option.value = item.id;
      if (item.id == connectionSubjectId) option.selected = true;
      connectionSubject.appendChild(option);
    });
  });

  $.getJSON(connectionTemplatesUrl, function (data) {
    data.forEach(function (item, index) {
      let option = document.createElement("option");
      option.text = item.name;
      option.value = item.id;
      if (item.id == connectionTemplateId) option.selected = true;
      connectionTemplate.appendChild(option);
      title.innerText = item.name;
    });
  });

  let formbuilder = new FormBuilder(schema, formBase);
  let print = formbuilder.print(data);
  formBase.innerHTML = "";
  formBase.appendChild(print.form);
});
