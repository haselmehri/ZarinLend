var config = {
  type: "gauge",
  data: {
    //labels: ['Success', 'Warning', 'Warning', 'Error'],
    datasets: [
      {
        data: [360, 50, 680, 900],
        value: 326,
        backgroundColor: ["red", "orange", "yellow", "green"],
        borderWidth: 2,
      },
    ],
  },
  options: {
    width: 10,
    responsive: false,
    // title: {
    //   display: true,
    //   text: "Gauge chart",
    // },
    layout: {
      padding: {
        bottom: 30,
      },
    },
    needle: {
      // Needle circle radius as the percentage of the chart area width
      radiusPercentage: 3,
      // Needle width as the percentage of the chart area width
      widthPercentage: 3,
      // Needle length as the percentage of the interval between inner radius (0%) and outer radius (100%) of the arc
      lengthPercentage: 15,
      // The color of the needle
      color: "rgba(0, 0, 0, 1)",
    },
    valueLabel: {
      formatter: Math.round,
    },
  },
};

window.onload = function () {
  var ctx = document.getElementById("chart").getContext("2d");
  window.myGauge = new Chart(ctx, config);
};

function userValidationNextStep(step) {
  const elem = $("#user-validation-section").children();
  const tabContainer = $("#user-validation-tabs").children();

  if (step <= 1) {
    elem.eq(step).addClass("d-none");
    elem.eq(step + 1).removeClass("d-none");

    const iconElement = tabContainer.eq(step).find(".bx");
    const tabElement = tabContainer.find(".zl-uv-current");

    $(iconElement).addClass("bxs-check-circle");
    $(iconElement).removeClass("bxs-time-five");
    $(tabElement).addClass("zl-uv-passed");
    $(tabElement).removeClass("zl-uv-current");

    const nextTab = $(tabElement).parent().next();
    nextTab.find(".zl-uv-next").addClass("zl-uv-current");
    nextTab.find(".bx").removeClass("bx-circle");
    nextTab.find(".bx").addClass("bxs-time-five");
  }
}

function userValidationPrevStep(step) {
  const elem = $("#user-validation-section").children();

  if (step > 0) {
    elem.eq(step).addClass("d-none");
    elem.eq(step - 1).removeClass("d-none");
  }
}
