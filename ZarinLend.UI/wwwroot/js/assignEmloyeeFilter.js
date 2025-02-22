$(function () {
  omurFilter.addEventListener("change", function () {
    mantagheFilter.innerHTML = "<option value='-1'>انتخاب کنید</option>";
    hozeFilter.innerHTML = "<option value='-1'>انتخاب کنید</option>";
    branchFilter.innerHTML = "<option value='-1'>انتخاب کنید</option>";

    if (this.value !== -1) {
      let tmpMantagheUrl = mantagheUrl + this.value;

      $.getJSON(tmpMantagheUrl, function (data) {
        data.forEach(function (item, index) {
          let mantagheOpt = document.createElement("option");
          mantagheOpt.text = item.name;
          mantagheOpt.value = item.id;
          mantagheFilter.appendChild(mantagheOpt);
        });
      });
    }
  });

  mantagheFilter.addEventListener("change", function () {
    hozeFilter.innerHTML = "<option value='-1'>انتخاب کنید</option>";
    branchFilter.innerHTML = "<option value='-1'>انتخاب کنید</option>";

    if (this.value !== -1) {
      let tmpHozeUrl = hozeUrl + this.value;

      $.getJSON(tmpHozeUrl, function (data) {
        data.forEach(function (item, index) {
          let hozeOpt = document.createElement("option");
          hozeOpt.text = item.name;
          hozeOpt.value = item.id;
          hozeFilter.appendChild(hozeOpt);
        });
      });
    }
  });

  hozeFilter.addEventListener("change", function () {
    branchFilter.innerHTML = "<option value='-1'>انتخاب کنید</option>";

    if (this.value !== -1) {
      var tmp = this.value.split(",");
      let tmpBranchUrl = branchUrl + tmp[0];

      $.getJSON(tmpBranchUrl, function (data) {
        data.forEach(function (item, index) {
          let branchOpt = document.createElement("option");
          branchOpt.text = item.name;
          branchOpt.value = item.id;
          branchFilter.appendChild(branchOpt);
        });
      });
    }
  });

  btnAccFilter.addEventListener("click", function () {
    let omurِData = omurFilter.options[omurFilter.options.selectedIndex].value;
    let mantagheData =
      mantagheFilter.options[mantagheFilter.options.selectedIndex].value;

    let hozeData = -1;
    if (hozeFilter.options[hozeFilter.options.selectedIndex].value != -1) {
      let tmpHozeData = hozeFilter.options[
        hozeFilter.options.selectedIndex
      ].value.split(",");
      hozeData = tmpHozeData[1];
    }

    let branchData =
      branchFilter.options[branchFilter.options.selectedIndex].value;
    let campaignData = campaigns.options[campaigns.options.selectedIndex].value;

    dataRequestEmployee.campaign = parseInt(campaignData);
    dataRequestEmployee.personnelId = parseInt(currentEmployeeId);
    dataRequestEmployee.departmentCode = parseInt(omurِData);
    dataRequestEmployee.regionCode = parseInt(mantagheData);
    dataRequestEmployee.areaId = parseInt(hozeData);
    dataRequestEmployee.branchId = parseInt(branchData);
    GetData("first");
  });

  $(".p-link").on("click", function () {
    let action = $(this).data("action");
    if (!$(this).parent().hasClass("disabled")) GetData(action);
  });

  function GetData(action) {
    $("#EmployeeList").block({
      message: "لطفا صبر کنید",
      css: {
        border: "none",
        padding: "15px",
        backgroundColor: "#000",
        "-webkit-border-radius": "10px",
        "-moz-border-radius": "10px",
        opacity: 0.5,
        color: "#fff",
      },
    });
    switch (action) {
      case "":
        dataRequestEmployee.page = 1;
        employeeCurrent.value = 1;
        break;
      case "first":
        dataRequestEmployee.page = 1;
        employeeCurrent.value = 1;
        break;
      case "perviews":
        if (parseInt(employeeCurrent.value) == 1) {
          dataRequestEmployee.page = 1;
        } else if (parseInt(employeeCurrent.value) > 1) {
          dataRequestEmployee.page = parseInt(employeeCurrent.value) - 1;
        }
        break;
      case "next":
        if (parseInt(employeeTotal.value) == parseInt(employeeCurrent.value)) {
          dataRequestEmployee.page = parseInt(employeeCurrent.value);
        } else if (
          parseInt(employeeTotal.value) > parseInt(employeeCurrent.value)
        ) {
          dataRequestEmployee.page = parseInt(employeeCurrent.value) + 1;
        }
        break;
      case "last":
        dataRequestEmployee.page = parseInt(employeeTotal.value);
        break;
      default:
        dataRequestEmployee.page = 1;
        break;
    }
    $.getJSON(
      getChildEmployeesUrl,
      dataRequestEmployee,
      function (data, status) {
        if (status == "success") {
          if (slide === true) {
            $("#EmployeeList").animate(
              {
                height: "100%",
              },
              1000
            );

            angleEmp.classList.remove("visible");
            angleEmp.classList.add("hidden");

            slide = false;

            selectedE.classList.remove("visible");
            selectedE.classList.add("hidden");
          }
          employeeCurrent.value = data.currentPage;
          employeeTotal.value = data.totalPages;

          if (data.currentPage == 1) {
            firstT.classList.add("disabled");
            perviewsT.classList.add("disabled");

            firstB.classList.add("disabled");
            perviewsB.classList.add("disabled");
          } else if (data.currentPage > 1) {
            firstT.classList.remove("disabled");
            perviewsT.classList.remove("disabled");

            firstB.classList.remove("disabled");
            perviewsB.classList.remove("disabled");
          }

          if (data.currentPage == data.totalPages) {
            nextT.classList.add("disabled");
            lastT.classList.add("disabled");

            nextB.classList.add("disabled");
            lastB.classList.add("disabled");
          } else {
            nextT.classList.remove("disabled");
            lastT.classList.remove("disabled");

            nextB.classList.remove("disabled");
            lastB.classList.remove("disabled");
          }

          if (data.totalPages > 1) {
            totalPagesT.innerText = data.totalPages;
            currentPageT.innerText = data.currentPage;

            totalPagesB.innerText = data.totalPages;
            currentPageB.innerText = data.currentPage;

            employeePagingTop.classList.remove("hidden");
            employeePagingTop.classList.add("visible");

            employeePagingBottom.classList.remove("hidden");
            employeePagingBottom.classList.add("visible");
          } else {
            totalPagesT.innerText = 0;
            currentPageT.innerText = 0;

            totalPagesB.innerText = 0;
            currentPageB.innerText = 0;

            employeePagingTop.classList.remove("visible");
            employeePagingTop.classList.add("hidden");

            employeePagingBottom.classList.remove("visible");
            employeePagingBottom.classList.add("hidden");
          }

          personels.innerHTML = "";
          $.each(data.items, function (i, item) {
            let template = `<tr>
                              <td>${item.firstName} ${item.family} - ${item.jobTitle} (${item.personnelId})</td>
                              <td>${item.departmentName}</td>
                              <td>${item.regionName}</td>
                              <td> حوزه (${item.areaId})</td>
                              <td>${item.branchName} - ${item.branchId}</td>
                              <td>${item.assignmentCount}</td>
                              <td><button class="btn btn-info selectE" data-name="${item.firstName} ${item.family}" data-jobTitle="${item.jobTitle}" data-id="${item.personnelId}">انتخاب</button></td>
                          </tr>`;
            personels.innerHTML += template;
          });
          btnAccFilter.innerHTML = `<i class="fal fa-filter"></i> &nbsp; فیلتر (${data.totalCount})`;
          $("#EmployeeList").unblock();
          bindSelectEmployee();
        } else {
          console.log(status);
        }
      }
    );
  }

  function bindSelectEmployee() {
    $(".selectE").on("click", function () {
      let name = $(this).data("name");
      let jobTitle = $(this).data("jobtitle");
      let id = $(this).data("id");

      let txt = `${name} - ${jobTitle} (${id})`;

      employeeData.innerHTML = txt;
      selectedEmployee.value = id;

      $("html, body").animate(
        {
          scrollTop: $("#selectedEmployee").offset().top,
        },
        2000
      );

      $("#EmployeeList").animate(
        {
          height: "0",
        },
        1000
      );

      slide = true;

      angleEmp.classList.remove("hidden");
      angleEmp.classList.add("visible");

      selectedE.classList.remove("hidden");
      selectedE.classList.add("visible");

      if (employeeTotal.value > 1) {
        employeePagingTop.classList.remove("visible");
        employeePagingTop.classList.add("hidden");

        employeePagingBottom.classList.remove("visible");
        employeePagingBottom.classList.add("hidden");
      }
    });
  }

  $("#angleEmployeeButton").on("click", function () {
    if (slide === true) {
      angleEmployeeButton.innerHTML = " <i class='fal fa-angle-up'></i>";
      slide = false;
      $("#EmployeeList").animate(
        {
          height: "100%",
        },
        "slow"
      );
      if (employeeTotal.value > 1) {
        employeePagingTop.classList.remove("hidden");
        employeePagingTop.classList.add("visible");

        employeePagingBottom.classList.remove("hidden");
        employeePagingBottom.classList.add("visible");
      }
    } else {
      angleEmployeeButton.innerHTML = " <i class='fal fa-angle-down'></i>";
      slide = true;
      $("#EmployeeList").animate(
        {
          height: "0",
        },
        1000
      );
      if (employeeTotal.value > 1) {
        employeePagingTop.classList.remove("visible");
        employeePagingTop.classList.add("hidden");

        employeePagingBottom.classList.remove("visible");
        employeePagingBottom.classList.add("hidden");
      }
    }
  });
});
