$(function () {
  let showTooltip = (target, content) => {
    target.tooltip("dispose");
    target
      .tooltip({
        container: "body",
        html: true,
        trigger: "manual",
        title: content,
      })
      .tooltip("show");
  };

  let currentLength = 0;
  $(".price-css")
    .on("keyup", function (event) {
      var digit = $(this).val();
      while (digit.indexOf(",", 0) != -1) {
        digit = digit.replace(",", "");
      }
      if ($(this).val() != "") {
        $(this).val(digit.replace(/(\d)(?=(\d\d\d)+(?!\d))/g, "$1,"));
        if ($(this).hasClass("price-css")) {
          if (digit == undefined) return;

          showTooltip($("#" + this.id), wordifyRials(digit));
        }
      } else {
        if ($(this).hasClass("price-css")) $("#" + this.id).tooltip("hide");
      }
    })
    .on("blur", function () {
      if ($(this).hasClass("price-css")) $("#" + this.id).tooltip("hide");
    })
    .on("keydown", function (event) {
      if (
        event.keyCode == 46 ||
        event.keyCode == 8 ||
        event.which == 8 ||
        event.which == 46
      ) {
        prevent = false;
        return;
      }
      var maxLength = $(this).attr("maxlength");
      if (maxLength != undefined) {
        var digit = $(this).val();
        while (digit.indexOf(",", 0) != -1) {
          digit = digit.replace(",", "");
        }
        currentLength = digit.length; // + 1;
        var splitCount = parseInt(currentLength / 3, 0);
        if (currentLength + splitCount > maxLength) {
          event.preventDefault();
        }
      }
    })
    .on("focusout", function () {
      var digit = $(this).val();
      while (digit.indexOf(",", 0) != -1) {
        digit = digit.replace(",", "");
      }
      if ($(this).val() != "") {
        $(this).val(digit.replace(/(\d)(?=(\d\d\d)+(?!\d))/g, "$1,"));
        if ($(this).hasClass("price-css")) {
          showTooltip($("#" + this.id), wordifyRials(digit));
        }
      } else {
        if ($(this).hasClass("price-css")) $("#" + this.id).tooltip("hide");
      }
    })
    .focus(function () {
      $(this).select();
      var digit = $(this).val();
      while (digit.indexOf(",", 0) != -1) {
        digit = digit.replace(",", "");
      }
      showTooltip($("#" + this.id), wordifyRials(digit));
    });
});
