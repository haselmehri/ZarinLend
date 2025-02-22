$(function () {
  $("#createForm").submit(function (event) {
    var formValidate = true;

    var usernameValue = $("#Title").val().toLowerCase();
    if (usernameValue === "") {
      $("#Title").addClass("is-invalid");
      $("#validationForTitle").text("عنوان الزامی است.");
      formValidate = false;
    } else {
      $("#Title").removeClass("is-invalid");
      $("#validationForTitle").text("");
    }

    var passwordValue = $("#Descriptions").val();
    if (passwordValue === "") {
      $("#Descriptions").addClass("is-invalid");
      $("#validationForDescriptions").text("توضیحات الزامی است.");
      formValidate = false;
    } else {
      $("#Descriptions").removeClass("is-invalid");
      $("#validationForDescriptions").text("");
    }

    if (formValidate == false) {
      event.preventDefault();
    }
  });
});
