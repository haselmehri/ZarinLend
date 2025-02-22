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

function campaignFilterInit() {
  campaignFilter.innerHTML = "<option value='-1'>انتخاب کنید</option>";
  GetDropDown(campaignFilter, campaginsUrl);
}

function OmurFilterInit() {
  omurFilter.innerHTML = "<option value='-1'>انتخاب کنید</option>";
  GetDropDown(omurFilter, omurUrl);
  omurFilter.options.selectedIndex = 0;

  mantagheFilter.innerHTML = "<option value='-1'>انتخاب کنید</option>";
  hozeFilter.innerHTML = "<option value='-1'>انتخاب کنید</option>";
  branchFilter.innerHTML = "<option value='-1'>انتخاب کنید</option>";
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
  GetDropDown(hozeFilter, tmpHozeUrl, currentEmployeeArea);
  $("#hozeFilter").addClass("disabled");
  $("#hozeFilter").attr("disabled", "disabled");

  branchFilter.innerHTML = "<option value='-1'>انتخاب کنید</option>";
  let tmpBranchUrl = branchUrl + currentEmployeeArea;
  GetDropDown(branchFilter, tmpBranchUrl);
  branchFilter.options.selectedIndex = 0;
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

omurFilter.addEventListener("change", function () {
  let omurData = this.value;
  if (omurData == "-1") {
    unitTypeId = 4;
  } else {
    unitTypeId = 5;
  }
  let tmpMantagheUrl = mantagheUrl + omurData;
  mantagheFilter.innerHTML = "<option value='-1'>انتخاب کنید</option>";
  GetDropDown(mantagheFilter, tmpMantagheUrl);
  mantagheFilter.options.selectedIndex = 0;

  hozeFilter.innerHTML = "<option value='-1'>انتخاب کنید</option>";
  branchFilter.innerHTML = "<option value='-1'>انتخاب کنید</option>";
});

mantagheFilter.addEventListener("change", function () {
  let mantagheData = this.value;
  hozeFilter.innerHTML = "<option value='-1'>انتخاب کنید</option>";

  if (mantagheData == "-1") {
    unitTypeId = 5;
  } else {
    unitTypeId = 6;
  }

  let tmpHozeUrl = hozeUrl + mantagheData;
  GetDropDown(hozeFilter, tmpHozeUrl);
  hozeFilter.options.selectedIndex = 0;

  branchFilter.innerHTML = "<option value='-1'>انتخاب کنید</option>";
});

hozeFilter.addEventListener("change", function () {
  let hozeData = this.value;
  let hozeId = hozeData.split(",");
  if (hozeData == "-1") {
    unitTypeId = 6;
  } else {
    unitTypeId = 7;
  }
  let tmpBranchUrl = branchUrl + hozeId[0];
  GetDropDown(branchFilter, tmpBranchUrl);
  branchFilter.options.selectedIndex = 0;
});

branchFilter.addEventListener("change", function () {
  let branchData = this.value;
  if (branchData == "-1") {
    unitTypeId = 7;
  } else {
    unitTypeId = 8;
  }
});

$("#btnFilter").on("click", function () {
  let dpCode, regCode, area, branch, status;

  departmentCode = omurFilter.value;
  regionCode = mantagheFilter.value;
  areaId = hozeFilter.value;
  branchId = branchFilter.value;
  statusId = connectionFilter.value;

  if (departmentCode > 0) {
    dpCode = `departmentCode=${departmentCode}&`;
  } else {
    dpCode = "";
  }

  if (regionCode > 0) {
    regCode = `regionCode=${regionCode}&`;
  } else {
    regCode = "";
  }

  if (areaId > 0) {
    area = `areaId=${areaId}&`;
  } else {
    area = "";
  }

  if (branchId > 0) {
    branch = `branchId=${branchId}&`;
  } else {
    branch = "";
  }

  if (statusId > 0) {
    status = `statusId=${statusId}&`;
  } else {
    status = "";
  }

  let url = `${baseUrl}${dpCode}${regCode}${area}${branch}${status}`;
  url = url.trim().replace(/^&|&$/g, "");
  window.location.href = url;
});

function init() {
  switch (currentEmployeeRole) {
    case "4":
      employeeOmurFilterInit();
      break;
    case "5":
      employeeMantagheFilterInit();
      break;
    case "6":
      employeeHozeFilterInit();
      break;
    case "7":
      employeeBranchFilterInit();
      break;
    default:
      employeeFullFilterInit();
      break;
  }
  campaignFilterInit();
}
init();
