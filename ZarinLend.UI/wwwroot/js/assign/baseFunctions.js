function GetDropDown(base, url, selectedItem) {
  $.getJSON(url, function (data) {
    data.forEach(function (item, index) {
      let option = document.createElement("option");
      option.text = item.name;
      option.value = item.id;

      base.appendChild(option);
    });

    if (selectedItem !== undefined) base.value = selectedItem;
  }).fail(function (xhr, status, error) {
    console.error(status, error);
  });
}

function removeItemOnce(arr, value) {
  var index = arr.indexOf(value);
  if (index > -1) {
    arr.splice(index, 1);
  }
  return arr;
}

function find(arr, value) {
  var index = arr.indexOf(value);
  if (index > -1) {
    return true;
  }
  return false;
}

function campaignsInit() {
  campaigns.innerHTML = "<option value='-1'>انتخاب کنید</option>";
  GetDropDown(campaigns, campaginsUrl);
}

function accountManagersInit() {
  accountManagers.innerHTML = "<option value='-1'>انتخاب کنید</option>";
  GetDropDown(accountManagers, accountManagersUrl);
}

function validateCampaign() {
  let campaginValue = parseInt(
    campaigns.options[campaigns.selectedIndex].value
  );

  if (campaginValue === -1) return false;
  else return true;
}

function validateAccountStep() {
  let accountManagerData =
    accountManagers.options[accountManagers.selectedIndex].value;
  let selectedEmployeeData = parseInt(selectedEmployee.value);

  if (accountManagerData == "-1") {
    accountStep.classList.remove("current");
    accountStep.classList.add("danger");

    alertElement.innerText = aError;
    alertElement.classList.remove("hidden");
    alertElement.classList.add("visible");
    setError = true;
  } else if (
    (accountManagerData == 8 || accountManagerData == 30) &&
    selectedEmployeeData == "0"
  ) {
    accountStep.classList.remove("current");
    accountStep.classList.add("danger");

    alertElement.innerText = aError;
    alertElement.classList.remove("hidden");
    alertElement.classList.add("visible");
    setError = true;
  } else {
    if (setError) {
      accountStep.classList.remove("danger");
      accountStep.classList.add("current");

      alertElement.innerText = "";
      alertElement.classList.remove("visible");
      alertElement.classList.add("hidden");

      setError = false;
    }
  }
}

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
  let tmpHozeUrl = hozeUrl + currentEmployeeRegion;
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
  let tmpHozeUrl = hozeUrl + currentEmployeeRegion;

  // GetDropDown(hozeFilter, tmpHozeUrl, currentEmployeeArea);
  setHozeAndBranchFilter(hozeFilter, tmpHozeUrl, currentEmployeeArea);
}

function setHozeAndBranchFilter(element, url, id) {
  fetch(url)
    .then((response) => response.json())
    .then((data) => {
      let selectedArea = "";
      let selectedItem = "";

      data.forEach(function (item, index) {
        let option = document.createElement("option");
        option.text = item.name;
        option.value = item.id;
        let seprated = item.id.split(",");

        if (seprated[1] && seprated[1] == id) {
          selectedItem = item.id;
          selectedArea = seprated[0];
        }

        element.appendChild(option);
      });

      if (selectedItem !== undefined) {
        element.value = selectedItem;
      }

      $("#hozeFilter").addClass("disabled");
      $("#hozeFilter").attr("disabled", "disabled");

      branchFilter.innerHTML = "<option value='-1'>انتخاب کنید</option>";
      let tmpBranchUrl = branchUrl + selectedArea;

      GetDropDown(branchFilter, tmpBranchUrl);
      branchFilter.options.selectedIndex = 0;
    })
    .catch((response) => {
      console.error(response);
    });
}

function getHozeByBranchId(branchId) {
  let url = getHozeByBranchUrl + branchId;
  let hozeId = 0;

  $.ajaxSetup({
    async: false,
  });

  $.getJSON(url, function (data) {
    console.log(data);
    hozeId = data;
  }).fail(function (xhr, status, error) {
    console.error(status, error);
  });

  $.ajaxSetup({
    async: true,
  });
  return hozeId;
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
  let tmpHozeUrl = hozeUrl + currentEmployeeRegion;
  let areaId = getHozeByBranchId(currentEmployeeBranch);
  let employeeArea = `${areaId},${currentEmployeeArea}`;

  GetDropDown(hozeFilter, tmpHozeUrl, employeeArea);
  $("#hozeFilter").addClass("disabled");
  $("#hozeFilter").attr("disabled", "disabled");

  branchFilter.innerHTML = "<option value='-1'>انتخاب کنید</option>";
  console.log(getHozeByBranchId(currentEmployeeBranch));
  let tmpBranchUrl = branchUrl + getHozeByBranchId(currentEmployeeBranch);
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
  dataRequestEmployee.minAssignmentCount = 0;

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

function customerCalculatePage(action) {
  switch (action) {
    case "":
      dataRequestCustomer.page = 1;
      customerCurrent.value = 1;
      break;
    case "first":
      dataRequestCustomer.page = 1;
      customerCurrent.value = 1;
      break;
    case "perviews":
      if (parseInt(customerCurrent.value) == 1) {
        dataRequestCustomer.page = 1;
      } else if (parseInt(customerCurrent.value) > 1) {
        dataRequestCustomer.page = parseInt(customerCurrent.value) - 1;
      }
      break;
    case "next":
      if (parseInt(customerTotal.value) == parseInt(customerCurrent.value)) {
        dataRequestCustomer.page = parseInt(customerCurrent.value);
      } else if (
        parseInt(customerTotal.value) > parseInt(customerCurrent.value)
      ) {
        dataRequestCustomer.page = parseInt(customerCurrent.value) + 1;
      }
      break;
    case "last":
      dataRequestCustomer.page = parseInt(customerTotal.value);
      break;
    default:
      dataRequestCustomer.page = 1;
      break;
  }
}

function customerPagingInit(data) {
  if (data.currentPage == 1) {
    cFirstT.classList.add("disabled");
    cPerviewsT.classList.add("disabled");

    cFirstB.classList.add("disabled");
    cPerviewsB.classList.add("disabled");
  } else if (data.currentPage > 1) {
    cFirstT.classList.remove("disabled");
    cPerviewsT.classList.remove("disabled");

    cFirstB.classList.remove("disabled");
    cPerviewsB.classList.remove("disabled");
  }

  if (data.currentPage == data.totalPages) {
    cNextT.classList.add("disabled");
    cLastT.classList.add("disabled");

    cNextB.classList.add("disabled");
    cLastB.classList.add("disabled");
  } else {
    cNextT.classList.remove("disabled");
    cLastT.classList.remove("disabled");

    cNextB.classList.remove("disabled");
    cLastB.classList.remove("disabled");
  }

  if (data.totalPages > 1) {
    cTotalPagesT.innerText = data.totalPages;
    cCurrentPageT.innerText = data.currentPage;

    cTotalPagesB.innerText = data.totalPages;
    cCurrentPageB.innerText = data.currentPage;

    customerPagingTop.classList.remove("hidden");
    customerPagingTop.classList.add("visible");

    rowCount.classList.remove("hidden");
    rowCount.classList.add("visible");

    customerPagingBottom.classList.remove("hidden");
    customerPagingBottom.classList.add("visible");
  } else {
    cTotalPagesT.innerText = 0;
    cCurrentPageT.innerText = 0;

    cTotalPagesB.innerText = 0;
    cCurrentPageB.innerText = 0;

    rowCount.classList.remove("visible");
    rowCount.classList.add("hidden");

    customerPagingTop.classList.remove("visible");
    customerPagingTop.classList.add("hidden");

    customerPagingBottom.classList.remove("visible");
    customerPagingBottom.classList.add("hidden");
  }
}

$(".pgn").on("click", function () {
  var last = $(".pgn.active")[0];
  last.classList.remove("active");
  var self = $(this);
  $(this).addClass("active");

  dataRequestCustomer.pageSize = $(this).attr("data-value");

  GetCustomerFirst();
});

function GetCustomerData(action) {
  $("#CustomerList").block({
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

  customerCalculatePage(action);

  $.getJSON(getChildAssigneeUrl, dataRequestCustomer, function (data, status) {
    if (status == "success") {
      customers.innerHTML = "";

      customerTotal.value = data.totalPages;
      customerCurrent.value = data.currentPage;
      customerPagingInit(data);

      btnCustomerFilter.innerHTML = `<i class="fal fa-filter"></i> &nbsp; فیلتر (${data.totalCount})`;

      $.each(data.items, function (i, item) {
        let state = "";
        if (item.state == item.campaignState) {
          let color = "#28a745";
          let txt = "";
          if (item.state == "I") txt = "fa-arrow-alt-up";
          else if (item.state == "D") txt = "fa-arrow-alt-down";

          state = `<i class="fad ${txt}" style="color: ${color}" title="مطلوب"></i>`;
        } else {
          let txt = "";
          let color = "#fd7e14";
          let title = "نا مطلوب";
          switch (item.state) {
            case "I":
              txt = "fa-arrow-alt-up";
              break;
            case "D":
              txt = "fa-arrow-alt-down";
              break;
            case "E":
              txt = "fa-minus";
              color = "#6c757d";
              title = "بی تغییر";
              break;
          }
          state = `<i class="fad ${txt}" style="color: ${color}" title="${title}"></i>`;
        }
        let checkBox = "";
        let status = "";
        let title = "";

        if (item.statuseId == 2) {
          status = "table-info";
        } else if (item.statuseId == 3) {
          status = "table-warning";
        } else if (item.statuseId == 1) {
          status = "";
        }

        if (item.assignPermission == true) {
          checkBox = `<i class="fal fa-square selecting" data-customer="${item.fullName} (${item.customerNo})" data-id="${item.id}" data-value="false"></i>`;
        } else {
          checkBox = `<i class="fal fa-ban" data-customer="${item.fullName} (${item.customerNo})" data-id="${item.id}"></i>`;
          status = "table-danger";
        }

        if (item.employeeFullName) {
          title = `مدیر حساب: ${item.employeeFullName} (${item.assignmentPersonnelId})`;
        } else {
          title = "";
        }

        let template = `<tr class="${status}"   data-toggle="tooltip" title="${title}">
                            <td>${checkBox}</td>
                            <td>${item.fullName} (${item.customerNo})</td>
                            <td>${item.sumManabe.toLocaleString("fa")}</td>
                            <td>${item.sumTasilat.toLocaleString("fa")}</td>
                            <td>${item.assignByRole}</td>
                            <td>${item.assignToRole}</td>
                            <td>${item.statusesTitle}</td>
                            <td style="text-align: center">${state}</td>
                        </tr>`;
        customers.innerHTML += template;
      });

      $("#CustomerList").unblock();

      bindSelectCustomer();
      let isSelected = $("#selectAll")[0].dataset.value;

      if (isSelected == "true") {
        $(this).attr("data-value", "false");

        $(this).removeClass("fa-check-square");
        $(this).addClass("fa-square");
      }
    }
  });
}

$(".selectAll").on("click", function (e) {
  let isSelected = $(this)[0].dataset.value;

  if (isSelected == "true") {
    $(this).attr("data-value", "false");

    $(this).removeClass("fa-check-square");
    $(this).addClass("fa-square");

    let allCustomerSelect = $(".selecting");
    $.each(allCustomerSelect, function (i, item) {
      let id = item.dataset.id;
      let customer = item.dataset.customer;

      item.dataset.value = false;

      item.classList.remove("fa-check-square");
      item.classList.add("fa-square");

      customerDataInfo = removeItemOnce(customerDataInfo, customer);
      customerData = removeItemOnce(customerData, id);
    });
  } else {
    $(this).attr("data-value", "true");

    $(this).removeClass("fa-square");
    $(this).addClass("fa-check-square");

    let allCustomerSelect = $(".selecting");
    $.each(allCustomerSelect, function (i, item) {
      let id = item.dataset.id;
      let customer = item.dataset.customer;

      item.dataset.value = true;

      item.classList.remove("fa-square");
      item.classList.add("fa-check-square");

      if (find(customerDataInfo, customer) == false)
        customerDataInfo.push(customer);

      if (find(customerData, id) == false) customerData.push(id);
    });
  }
});

function bindSelectCustomer() {
  $(".selecting").on("click", function () {
    let isSelected = $(this)[0].dataset.value;
    let id = $(this)[0].dataset.id;
    let customer = $(this)[0].dataset.customer;

    if (isSelected == "true") {
      $(this).attr("data-value", "false");

      $(this).removeClass("fa-check-square");
      $(this).addClass("fa-square");

      customerDataInfo = removeItemOnce(customerDataInfo, customer);
      customerData = removeItemOnce(customerData, id);
    } else if ("false") {
      $(this).attr("data-value", "true");

      $(this).addClass("fa-check-square");
      $(this).removeClass("fa-square");

      if (find(customerDataInfo, customer) == false)
        customerDataInfo.push(customer);

      if (find(customerData, id) == false) customerData.push(id);
    }
  });
}

function GetCustomerDataByEmployee() {
  let department = $("#selectedEmployee")[0].dataset.department;
  let region = $("#selectedEmployee")[0].dataset.region;
  let area = $("#selectedEmployee")[0].dataset.area;
  let branch = $("#selectedEmployee")[0].dataset.branch;

  let departmentUrl = omurUrl;
  let regionUrl = mantagheUrl + department;
  let areaUrl = hozeUrl + region;
  let areaId = area.split(",");
  let tmpBranchUrl = branchUrl + areaId[0];

  omurCustomerFilter.innerHTML = "";
  GetDropDown(omurCustomerFilter, departmentUrl, department);
  mantagheCustomerFilter.innerHTML = "";
  GetDropDown(mantagheCustomerFilter, regionUrl, region);
  hozeCustomerFilter.innerHTML = "";
  GetDropDown(hozeCustomerFilter, areaUrl, area);
  branchCustomerFilter.innerHTML = "";
  GetDropDown(branchCustomerFilter, tmpBranchUrl, branch);

  $("#omurCustomerFilter").addClass("disabled").attr("disabled", true);
  $("#mantagheCustomerFilter").addClass("disabled").attr("disabled", true);
  $("#hozeCustomerFilter").addClass("disabled").attr("disabled", true);
  $("#branchCustomerFilter").addClass("disabled").attr("disabled", true);

  dataRequestCustomer.departmentCode = department;
  dataRequestCustomer.regionCode = region;
  dataRequestCustomer.areaId = areaId[1];
  dataRequestCustomer.branchId = branch;

  GetCustomerData("first");
}

function customerFullFilterInit() {
  omurCustomerFilter.innerHTML = "<option value='-1'>انتخاب کنید</option>";
  GetDropDown(omurCustomerFilter, omurUrl);
  omurCustomerFilter.options.selectedIndex = 0;

  mantagheCustomerFilter.innerHTML = "<option value='-1'>انتخاب کنید</option>";
  hozeCustomerFilter.innerHTML = "<option value='-1'>انتخاب کنید</option>";
  branchCustomerFilter.innerHTML = "<option value='-1'>انتخاب کنید</option>";
}

function customerOmurFilterInit() {
  omurCustomerFilter.innerHTML = "<option value='-1'>انتخاب کنید</option>";
  GetDropDown(omurCustomerFilter, omurUrl, currentEmployeeDepartment);
  $("#omurCustomerFilter").addClass("disabled");
  $("#omurCustomerFilter").attr("disabled", "disabled");

  let tmpMantagheUrl = mantagheUrl + currentEmployeeDepartment;
  mantagheCustomerFilter.innerHTML = "<option value='-1'>انتخاب کنید</option>";
  GetDropDown(mantagheCustomerFilter, tmpMantagheUrl);
  mantagheCustomerFilter.options.selectedIndex = 0;

  hozeCustomerFilter.innerHTML = "<option value='-1'>انتخاب کنید</option>";
  branchCustomerFilter.innerHTML = "<option value='-1'>انتخاب کنید</option>";
}

function customerMantagheFilterInit() {
  omurCustomerFilter.innerHTML = "<option value='-1'>انتخاب کنید</option>";
  GetDropDown(omurCustomerFilter, omurUrl, currentEmployeeDepartment);
  $("#omurCustomerFilter").addClass("disabled");
  $("#omurCustomerFilter").attr("disabled", "disabled");

  mantagheCustomerFilter.innerHTML = "<option value='-1'>انتخاب کنید</option>";
  let tmpMantagheUrl = mantagheUrl + currentEmployeeDepartment;
  GetDropDown(mantagheCustomerFilter, tmpMantagheUrl, currentEmployeeRegion);
  $("#mantagheCustomerFilter").addClass("disabled");
  $("#mantagheCustomerFilter").attr("disabled", "disabled");

  hozeCustomerFilter.innerHTML = "<option value='-1'>انتخاب کنید</option>";
  let tmpHozeUrl = hozeUrl + currentEmployeeRegion;
  GetDropDown(hozeCustomerFilter, tmpHozeUrl);
  hozeCustomerFilter.options.selectedIndex = 0;

  branchCustomerFilter.innerHTML = "<option value='-1'>انتخاب کنید</option>";
}

function customerHozeFilterInit() {
  omurCustomerFilter.innerHTML = "<option value='-1'>انتخاب کنید</option>";
  GetDropDown(omurCustomerFilter, omurUrl, currentEmployeeDepartment);
  $("#omurCustomerFilter").addClass("disabled");
  $("#omurCustomerFilter").attr("disabled", "disabled");

  mantagheCustomerFilter.innerHTML = "<option value='-1'>انتخاب کنید</option>";
  let tmpMantagheUrl = mantagheUrl + currentEmployeeDepartment;
  GetDropDown(mantagheCustomerFilter, tmpMantagheUrl, currentEmployeeRegion);
  $("#mantagheCustomerFilter").addClass("disabled");
  $("#mantagheCustomerFilter").attr("disabled", "disabled");

  hozeCustomerFilter.innerHTML = "<option value='-1'>انتخاب کنید</option>";
  let tmpHozeUrl = hozeUrl + currentEmployeeRegion;
  setHozeCustomerFilter(hozeCustomerFilter, tmpHozeUrl, currentEmployeeArea);
}

function setHozeCustomerFilter(element, url, id) {
  fetch(url)
    .then((response) => response.json())
    .then((data) => {
      let selectedArea = "";
      let selectedItem = "";

      data.forEach(function (item, index) {
        let option = document.createElement("option");
        option.text = item.name;
        option.value = item.id;
        let seprated = item.id.split(",");

        element.appendChild(option);

        if (seprated[1] && seprated[1] == id) {
          console.log(item.id);
          selectedItem = item.id;
          selectedArea = seprated[0];
        }
      });

      if (selectedItem !== undefined) {
        element.value = selectedItem;
      }

      $("#hozeCustomerFilter").addClass("disabled");
      $("#hozeCustomerFilter").attr("disabled", "disabled");

      branchCustomerFilter.innerHTML =
        "<option value='-1'>انتخاب کنید</option>";
      let tmpBranchUrl = branchUrl + selectedArea;
      GetDropDown(branchCustomerFilter, tmpBranchUrl);
      branchCustomerFilter.options.selectedIndex = 0;
    })
    .catch((response) => {
      console.error(response);
    });
}

function customerBranchFilterInit() {
  omurCustomerFilter.innerHTML = "<option value='-1'>انتخاب کنید</option>";
  GetDropDown(omurCustomerFilter, omurUrl, currentEmployeeDepartment);
  $("#omurCustomerFilter").addClass("disabled");
  $("#omurCustomerFilter").attr("disabled", "disabled");

  mantagheCustomerFilter.innerHTML = "<option value='-1'>انتخاب کنید</option>";
  let tmpMantagheUrl = mantagheUrl + currentEmployeeDepartment;
  GetDropDown(mantagheCustomerFilter, tmpMantagheUrl, currentEmployeeRegion);
  $("#mantagheCustomerFilter").addClass("disabled");
  $("#mantagheCustomerFilter").attr("disabled", "disabled");

  hozeCustomerFilter.innerHTML = "<option value='-1'>انتخاب کنید</option>";
  let tmpHozeUrl = hozeUrl + currentEmployeeRegion;
  let areaId = getHozeByBranchId(currentEmployeeBranch);
  let employeeArea = `${areaId},${currentEmployeeArea}`;
  GetDropDown(hozeCustomerFilter, tmpHozeUrl, employeeArea);
  $("#hozeCustomerFilter").addClass("disabled");
  $("#hozeCustomerFilter").attr("disabled", "disabled");

  branchCustomerFilter.innerHTML = "<option value='-1'>انتخاب کنید</option>";
  let tmpBranchUrl = branchUrl + getHozeByBranchId(currentEmployeeBranch);
  GetDropDown(branchCustomerFilter, tmpBranchUrl, currentEmployeeBranch);
  $("#branchCustomerFilter").addClass("disabled");
  $("#branchCustomerFilter").attr("disabled", "disabled");
}

function GetCustomerDataByRole() {
  switch (currentEmployeeRole) {
    case "4":
      {
        customerOmurFilterInit();
        GetCustomerData("first");
      }
      break;
    case "5":
      {
        customerMantagheFilterInit();
        GetCustomerData("first");
      }
      break;
    case "6":
      {
        customerHozeFilterInit();
        GetCustomerData("first");
      }
      break;
    case "7":
      {
        customerBranchFilterInit();
        GetCustomerData("first");
      }
      break;
    default:
      {
        customerFullFilterInit();
        GetCustomerFirst();
      }
      break;
  }
}

omurCustomerFilter.addEventListener("change", function () {
  mantagheCustomerFilter.innerHTML = "<option value='-1'>انتخاب کنید</option>";
  hozeCustomerFilter.innerHTML = "<option value='-1'>انتخاب کنید</option>";
  branchCustomerFilter.innerHTML = "<option value='-1'>انتخاب کنید</option>";

  if (this.value !== -1) {
    let tmpMantagheUrl = mantagheUrl + this.value;

    GetDropDown(mantagheCustomerFilter, tmpMantagheUrl);
  }
});

mantagheCustomerFilter.addEventListener("change", function () {
  hozeCustomerFilter.innerHTML = "<option value='-1'>انتخاب کنید</option>";
  branchCustomerFilter.innerHTML = "<option value='-1'>انتخاب کنید</option>";

  if (this.value !== -1) {
    let tmpHozeUrl = hozeUrl + this.value;

    GetDropDown(hozeCustomerFilter, tmpHozeUrl);
  }
});

hozeCustomerFilter.addEventListener("change", function () {
  branchCustomerFilter.innerHTML = "<option value='-1'>انتخاب کنید</option>";

  if (this.value !== -1) {
    var tmp = this.value.split(",");
    let tmpBranchUrl = branchUrl + tmp[0];
    GetDropDown(branchCustomerFilter, tmpBranchUrl);
  }
});

$(".pLink").on("click", function () {
  let action = $(this).data("action");
  if (!$(this).parent().hasClass("disabled")) GetCustomerData(action);
});

$("#btnCustomerFilter").on("click", function () {
  GetCustomerFirst();
});

function GetCustomerFirst() {
  dataRequestCustomer.departmentCode = getDepartment();
  dataRequestCustomer.regionCode = getRegion();

  let areaId = getArea();
  if (areaId != null) {
    let area = areaId.split(",");
    dataRequestCustomer.areaId = area[0];
  } else {
    dataRequestCustomer.areaId = areaId;
  }

  dataRequestCustomer.branchId = getBranch();
  dataRequestCustomer.statusId = getStatus();
  dataRequestCustomer.customerStateInCampaign = getCustomerStatus();

  let resource = getResourceFilter();

  if (resource != null) {
    switch (resource) {
      case "1":
        dataRequestCustomer.manabe = getPriceFilter();
        dataRequestCustomer.manabeOperator = getFilterOperator();
        dataRequestCustomer.tasilat = null;
        dataRequestCustomer.tasilatOperator = null;
        break;
      case "2":
        dataRequestCustomer.tasilat = getPriceFilter();
        dataRequestCustomer.tasilatOperator = getFilterOperator();
        dataRequestCustomer.manabe = null;
        dataRequestCustomer.manabeOperator = null;
        break;
    }
  }

  var campaignValue = campaigns.options[campaigns.options.selectedIndex].value;

  dataRequestCustomer.campaign = campaignValue;
  dataRequestCustomer.personnelId = parseInt(currentEmployeeId);

  let customerNo = $("#customerNumber").val();
  dataRequestCustomer.customerNumber = customerNo;

  GetCustomerData("first");
}

function getStatus() {
  let value = $("#connectionStatus").val();
  if (value >= 0) return value;
  return null;
}

function getDepartment() {
  let value = $("#omurCustomerFilter").val();
  if (value >= 0) return value;
  return null;
}

function getRegion() {
  let value = $("#mantagheCustomerFilter").val();
  if (value >= 0) return value;
  return null;
}

function getArea() {
  let value = $("#hozeCustomerFilter").val();
  if (value == "-1") return null;
  return value;
}

function getBranch() {
  let value = $("#branchCustomerFilter").val();
  if (value >= 0) return value;
  return null;
}

function getCustomerStatus() {
  let value = $("#customerStatus").val();
  if (value > 0) return value;
  return null;
}

function getResourceFilter() {
  let value = $("#resourceFilter").val();
  if (value > 0) return value;
  return null;
}

function getFilterOperator() {
  let value = $("#filterOperator").val();
  if (value > 0) return value;
  return null;
}

function getPriceFilter() {
  let value = $("#priceFilter").val();

  value = value.replaceAll(",", "");

  let result = parseInt(value);

  if (result >= 0) return result;
  return 0;
}
