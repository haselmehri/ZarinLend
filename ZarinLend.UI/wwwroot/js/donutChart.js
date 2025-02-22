function GetStatsChart(campaignId, employeeId) {
  const getChildSumStats = "/Api/GetChildSumStats";
  let requestData = {};
  requestData.personnelId = employeeId;
  requestData.campaignId = campaignId;

  $.getJSON(getChildSumStats, requestData, function (data) {
    
    let countAll = document.getElementById("countAll");
    let countAssigned = document.getElementById("countAssigned");
    let countUnAssigned = document.getElementById("countUnAssigned");
    let countSuspended = document.getElementById("countSuspended");

    countAll.innerHTML = data.sumCountAll;
    countAssigned.innerHTML = data.sumCountAssigned;
    countUnAssigned.innerHTML = data.sumCountUnAssigned;
    countSuspended.innerHTML = data.sumCountSuspended;

    //["CountDirectAssigned", data.sumCountDirectAssigned],

    var optsAssigned = {
      angle: 0, // The span of the gauge arc
      lineWidth: 0.3, // The line thickness
      radiusScale: 0.9, // Relative radius
      pointer: {
        length: 0.56, //Relative to gauge radius
        strokeWidth: 0.022, // The thickness
        color: "#000000", // Fill color
      },
      limitMax: false, // If false, max value increases automatically if value > maxValue
      limitMin: false, // If true, the min value of the gauge will be fixed
      colorStart: "#1CC1DA", // Colors
      colorStop: "#26C6DA", // just experiment with them
      strokeColor: "#E0E0E0", // to see which ones work best for you
      generateGradient: true,
      highDpiSupport: true, // High resolution support
    };

    var assigned = document.getElementById("assigned"); // your canvas element
    var gaugeAssigned = new Gauge(assigned).setOptions(optsAssigned); // create sexy gauge!
    gaugeAssigned.maxValue = data.sumCountAll; // set max gauge value
    gaugeAssigned.setMinValue(0); // Prefer setter over gauge.minValue = 0
    gaugeAssigned.animationSpeed = 32; // set animation speed (32 is default value)
    gaugeAssigned.set(data.sumCountAssigned); // set actual value

    var optsUnAssigned = {
      angle: 0, // The span of the gauge arc
      lineWidth: 0.3, // The line thickness
      radiusScale: 0.9, // Relative radius
      pointer: {
        length: 0.56, //Relative to gauge radius
        strokeWidth: 0.022, // The thickness
        color: "#000000", // Fill color
      },
      limitMax: false, // If false, max value increases automatically if value > maxValue
      limitMin: false, // If true, the min value of the gauge will be fixed
      colorStart: "#f81f48", // Colors
      colorStop: "#fc4b6c", // just experiment with them
      strokeColor: "#E0E0E0", // to see which ones work best for you
      generateGradient: true,
      highDpiSupport: true, // High resolution support
    };

    var unAssigned = document.getElementById("unAssigned"); // your canvas element
    var gaugeUnAssigned = new Gauge(unAssigned).setOptions(optsUnAssigned); // create sexy gauge!
    gaugeUnAssigned.maxValue = data.sumCountAll; // set max gauge value
    gaugeUnAssigned.setMinValue(0); // Prefer setter over gauge.minValue = 0
    gaugeUnAssigned.animationSpeed = 32; // set animation speed (32 is default value)
    gaugeUnAssigned.set(data.sumCountUnAssigned); // set actual value

    var optsSuspended = {
      angle: 0, // The span of the gauge arc
      lineWidth: 0.3, // The line thickness
      radiusScale: 0.9, // Relative radius
      pointer: {
        length: 0.56, //Relative to gauge radius
        strokeWidth: 0.022, // The thickness
        color: "#000000", // Fill color
      },
      limitMax: false, // If false, max value increases automatically if value > maxValue
      limitMin: false, // If true, the min value of the gauge will be fixed
      colorStart: "#f8a91e", // Colors
      colorStop: "#ffb22b", // just experiment with them
      strokeColor: "#E0E0E0", // to see which ones work best for you
      generateGradient: true,
      highDpiSupport: true, // High resolution support
    };

    var suspended = document.getElementById("suspended"); // your canvas element
    var gaugeSuspended = new Gauge(suspended).setOptions(optsSuspended); // create sexy gauge!
    gaugeSuspended.maxValue = data.sumCountAll; // set max gauge value
    gaugeSuspended.setMinValue(0); // Prefer setter over gauge.minValue = 0
    gaugeSuspended.animationSpeed = 32; // set animation speed (32 is default value)
    gaugeSuspended.set(data.sumCountSuspended); // set actual value

    let chartData = [
      ["تلفن", data.sumCountTel],
      ["پیامک", data.sumCountSms],
      ["جلسه", data.sumCountMeeting],
      ["پست الکترونیک", data.sumCountEmail],
      ["نامه", data.sumCountLetter],
    ];

    var chart = c3.generate({
      bindto: "#connections",
      data: {
        columns: chartData,
        type: "donut",
      },
      donut: {
        label: {
          show: false,
        },
        title: "ارتباط ها",
        width: 20,
      },

      legend: {
        hide: true,
      },
      color: {
        pattern: ["#a333c8", "#db2828", "#f2711c", "#fbbd08", "#2185d0"],
      },
    });
  });
}
