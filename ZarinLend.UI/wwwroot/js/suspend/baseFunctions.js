function employeeFullFilterInit() {
  omurFilter.innerHTML = "<option value='-1'>انتخاب کنید</option>";
  GetDropDown(omurFilter, omurUrl);
  omurFilter.options.selectedIndex = 0;

  mantagheFilter.innerHTML = "<option value='-1'>انتخاب کنید</option>";
  hozeFilter.innerHTML = "<option value='-1'>انتخاب کنید</option>";
  branchFilter.innerHTML = "<option value='-1'>انتخاب کنید</option>";
}

function employeeOmurFilterInit() {
  omurFilter.innerHTML = "<option value='-1'>انتخاب کنید</option>";
  GetDropDown(omurFilter, omurUrl, currentEmployeeDepartment);
  $("#omurFilter").addClass("disabled");
  $("#omurFilter").attr("disabled", "disabled");

  let tmpMantagheUrl = mantagheUrl + currentEmployeeDepartment;
  mantagheFilter.innerHTML = "<option value='-1'>انتخاب کنید</option>";
  GetDropDown(mantagheFilter, tmpMantagheUrl);
  mantagheFilter.options.selectedIndex = 0;

  hozeFilter.innerHTML = "<option value='-1'>انتخاب کنید</option>";
  branchFilter.innerHTML = "<option value='-1'>انتخاب کنید</option>";
}

function employeeMantagheFilterInit() {
  omurFilter.innerHTML = "<option value='-1'>انتخاب کنید</option>";
  GetDropDown(omurFilter, omurUrl, currentEmployeeDepartment);
  $("#omurFilter").addClass("disabled");
  $("#omurFilter").attr("disabled", "disabled");

  mantagheFilter.innerHTML = "<option value='-1'>انتخاب کنید</option>";
  let tmpMantagheUrl = mantagheUrl + currentEmployeeDepartment;
  GetDropDown(mantagheFilter, tmpMantagheUrl, currentEmployeeRegion);
  $("#mantagheFilter").addClass("disabled");
  $("#mantagheFilter").attr("disabled", "disabled");

  hozeFilter.innerHTML = "<option value='-1'>انتخاب کنید</option>";
  let tmpHozeUrl = HozeUrl + currentEmployeeRegion;
  GetDropDown(hozeFilter, tmpHozeUrl);
  hozeFilter.options.selectedIndex = 0;

  branchFilter.innerHTML = "<option value='-1'>انتخاب کنید</option>";
}

function employeeHozeFilterInit() {
  omurFilter.innerHTML = "<option value='-1'>انتخاب کنید</option>";
  GetDropDown(omurFilter, omurUrl, currentEmployeeDepartment);
  $("#omurFilter").addClass("disabled");
  $("#omurFilter").attr("disabled", "disabled");

  mantagheFilter.innerHTML = "<option value='-1'>انتخاب کنید</option>";
  let tmpMantagheUrl = mantagheUrl + currentEmployeeDepartment;
  GetDropDown(mantagheFilter, tmpMantagheUrl, currentEmployeeRegion);
  $("#mantagheFilter").addClass("disabled");
  $("#mantagheFilter").attr("disabled", "disabled");

  hozeFilter.innerHTML = "<option value='-1'>انتخاب کنید</option>";
  let tmpHozeUrl = HozeUrl + currentEmployeeRegion;
  GetDropDown(hozeFilter, tmpHozeUrl, currentEmployeeArea);
  $("#hozeFilter").addClass("disabled");
  $("#hozeFilter").attr("disabled", "disabled");

  branchFilter.innerHTML = "<option value='-1'>انتخاب کنید</option>";
  let tmpBranchUrl = branchUrl + currentEmployeeArea;
  GetDropDown(branchFilter, tmpBranchUrl);
  branchFilter.options.selectedIndex = 0;
}

function employeeBranchFilterInit() {
  omurFilter.innerHTML = "<option value='-1'>انتخاب کنید</option>";
  GetDropDown(omurFilter, omurUrl, currentEmployeeDepartment);
  $("#omurFilter").addClass("disabled");
  $("#omurFilter").attr("disabled", "disabled");

  mantagheFilter.innerHTML = "<option value='-1'>انتخاب کنید</option>";
  let tmpMantagheUrl = mantagheUrl + currentEmployeeDepartment;
  GetDropDown(mantagheFilter, tmpMantagheUrl, currentEmployeeRegion);
  $("#mantagheFilter").addClass("disabled");
  $("#mantagheFilter").attr("disabled", "disabled");

  hozeFilter.innerHTML = "<option value='-1'>انتخاب کنید</option>";
  let tmpHozeUrl = HozeUrl + currentEmployeeRegion;
  GetDropDown(hozeFilter, tmpHozeUrl, currentEmployeeArea);
  $("#hozeFilter").addClass("disabled");
  $("#hozeFilter").attr("disabled", "disabled");

  branchFilter.innerHTML = "<option value='-1'>انتخاب کنید</option>";
  let tmpBranchUrl = branchUrl + currentEmployeeArea;
  GetDropDown(branchFilter, tmpBranchUrl, currentEmployeeBranch);
  $("#branchFilter").addClass("disabled");
  $("#branchFilter").attr("disabled", "disabled");
}

function CalculatePage(action) {
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
}

function EmployeePagerInit(data) {
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
}

function bindSelectEmployee() {
  $(".selectE").on("click", function () {
    let name = $(this).data("name");
    let jobTitle = $(this).data("jobtitle");
    let id = $(this).data("id");

    let txt = `${name} - ${jobTitle} (${id})`;

    employeeData.innerHTML = txt;
    selectedEmployee.value = id;

    $("#selectedEmployee").attr("data-department", $(this).data("department"));
    $("#selectedEmployee").attr("data-region", $(this).data("region"));
    $("#selectedEmployee").attr("data-area", $(this).data("area"));
    $("#selectedEmployee").attr("data-branch", $(this).data("branch"));

    $("html, body").animate(
      {
        scrollTop: $("#employeeData").offset().top,
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

function GetEmployeeData(action) {
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

  CalculatePage(action);
  dataRequestEmployee.minAssignmentCount = 1;

  $.getJSON(getChildEmployeesUrl, dataRequestEmployee, function (data, status) {
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

      EmployeePagerInit(data);

      personels.innerHTML = "";
      $.each(data.items, function (i, item) {
        let areaId = item.areaId.split(",");
        let template = `<tr>
                            <td>${item.firstName} ${item.family} - ${item.jobTitle} (${item.personnelId})</td>
                            <td>${item.departmentName}</td>
                            <td>${item.regionName}</td>
                            <td> حوزه (${areaId[1]})</td>
                            <td>${item.branchName} - ${item.branchId}</td>
                            <td>${item.assignmentCount}</td>
                            <td><button class="btn btn-info selectE" data-branch="${item.branchId}" data-area="${item.areaId}" data-region="${item.regionCode}" data-department="${item.departmentCode}" data-name="${item.firstName} ${item.family}" data-jobTitle="${item.jobTitle}" data-id="${item.personnelId}">انتخاب</button></td>
                        </tr>`;
        personels.innerHTML += template;
      });
      btnAccFilter.innerHTML = `<i class="fal fa-filter"></i> &nbsp; فیلتر (${data.totalCount})`;
      $("#EmployeeList").unblock();
      bindSelectEmployee();
    } else {
      console.log(status);
    }
  });
}

mngOmurFilter.addEventListener("change", function () {
  mngMantagheFilter.innerHTML = "<option value='-1'>انتخاب کنید</option>";
  mngHozeFilter.innerHTML = "<option value='-1'>انتخاب کنید</option>";
  mngBranchFilter.innerHTML = "<option value='-1'>انتخاب کنید</option>";

  if (this.value !== -1) {
    let tmpMantagheUrl = mantagheUrl + this.value;

    GetDropDown(mngMantagheFilter, tmpMantagheUrl);
  }
});

mngMantagheFilter.addEventListener("change", function () {
  mngHozeFilter.innerHTML = "<option value='-1'>انتخاب کنید</option>";
  mngBranchFilter.innerHTML = "<option value='-1'>انتخاب کنید</option>";

  if (this.value !== -1) {
    let tmpHozeUrl = hozeUrl + this.value;

    GetDropDown(mngHozeFilter, tmpHozeUrl);
  }
});

mngHozeFilter.addEventListener("change", function () {
  mngBranchFilter.innerHTML = "<option value='-1'>انتخاب کنید</option>";

  if (this.value !== -1) {
    var tmp = this.value.split(",");
    let tmpBranchUrl = branchUrl + tmp[0];

    GetDropDown(mngBranchFilter, tmpBranchUrl);
  }
});
