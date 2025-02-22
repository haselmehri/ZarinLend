function getChartData(customerId) {
  let url = `/Api/CustomerChartData/${customerId}`;

  $.getJSON(url, function (data) {
    initChart(data);
  });
}

getChartData(customerNo);

function initChart(chartData) {
  // ==============================================================
  // customerData
  // ==============================================================
  let seriesObj = {
    data: [...chartData.data],
  };

  let data = {
    labels: chartData.title,
    series: [seriesObj],
  };


  const max = Math.max(...chartData.data);


  var responsiveOptions = [
    [
      "screen and (min-width: 641px) and (max-width: 1024px)",
      {
        showPoint: false,
        axisX: {
          labelInterpolationFnc: function (value) {
            return  value;
          },
        },
      },
    ],
    [
      "screen and (max-width: 640px)",
      {
        showLine: false,
        axisX: {
          labelInterpolationFnc: function (value) {
            return  value;
          },
        },
      },
    ],
  ];

  var chart = new Chartist.Line(
    "#chartData",
    data,
    {
      low: 0,
      high: max,
      showArea: true,
      fullWidth: true,
      axisX: {
        labelInterpolationFnc: function (value) {
          return  value;
        },
      },
      axisY: {
        onlyInteger: true,
        labelInterpolationFnc: function (value) {
          return value / 10000 + "ریال";
        },
      },
    },
    responsiveOptions
  );

}
