$(function () {
  $("#loginForm").submit(function (event) {
    let form_validate = true;

    var usernameValue = $("#Username").val().toLowerCase();
    if (usernameValue === "") {
      $("#Username").addClass("is-invalid");
      $("#validationForUsername").text("نام کاربری الزامی است.");
      form_validate = false;
    } else {
      $("#Username").removeClass("is-invalid");
      $("#validationForUsername").text("");
    }

    var passwordValue = $("#Password").val();
    if (passwordValue === "") {
      $("#Password").addClass("is-invalid");
      $("#validationForPassword").text("کلمه عبور الزامی است.");
      form_validate = false;
    } else {
      $("#Password").removeClass("is-invalid");
      $("#validationForPassword").text("");
    }

    if (form_validate == false) {
      event.preventDefault();
    }
  });
});
