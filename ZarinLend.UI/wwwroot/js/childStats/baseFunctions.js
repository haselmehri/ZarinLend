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

function OmurFilterInit() {
  omurFilter.innerHTML = "<option value='-1'>انتخاب کنید</option>";
  GetDropDown(omurFilter, omurUrl);
  omurFilter.options.selectedIndex = 0;

  mantagheFilter.innerHTML = "<option value='-1'>انتخاب کنید</option>";
  hozeFilter.innerHTML = "<option value='-1'>انتخاب کنید</option>";
  branchFilter.innerHTML = "<option value='-1'>انتخاب کنید</option>";
}

function getDepartment() {
  let value = $("#omurFilter").val();
  if (value >= 0) return value;
  return null;
}

function getRegion() {
  let value = $("#mantagheFilter").val();
  if (value >= 0) return value;
  return null;
}

function getArea() {
  let value = $("#hozeFilter").val();
  if (value == "-1") return null;
  return value;
}

function getBranch() {
  let value = $("#branchFilter").val();
  if (value >= 0) return value;
  return null;
}

function createTemplate(item, unitType) {
  let rowTemplate = `<tr>
                        <th>${
                          unitType < 4
                            ? item.title
                            : `${item.unitTitle}: ${item.title}(${item.id})`
                        }</th>`;
  if (unitType < 8) {
    rowTemplate += `<th><a href="#" class="directAssigned" data-id="${item.id}" data-value="${item.unitType}" data-title="${item.title}">${item.countDirectAssigned}</a></th>`;
  } else {
    rowTemplate += `<th>${item.countDirectAssigned}</a></th>`;
  }
  rowTemplate += `<th>${item.countAll}</th>
                  <th>${item.countAssigned}</th>
                  <th>${item.countUnAssigned}</th>
                  <th>${item.countSuspended}</th>
                  <th>
                  <span class="ui blue label noselect" data-toggle="tooltip" data-placement="top" title="نامه">${item.countLetter}</span>
                  <span class="ui purple label noselect" data-toggle="tooltip" data-placement="top" title="تلفن">${item.countTel}</span>
                  <span class="ui red label noselect" data-toggle="tooltip" data-placement="top" title="پیامک">${item.countSms}</span>
                  <span class="ui orange label noselect" data-toggle="tooltip" data-placement="top" title="جلسه">${item.countMeeting}</span>
                  <span class="ui yellow label noselect" data-toggle="tooltip" data-placement="top" title="پست الکترونیک">${item.countEmail}</span>
                  </th>`;

  if (unitType < 7) {
    rowTemplate += `<th>
                      <button class="btn btn-link lnk" data-id=${item.id} data-value="${item.unitType}">نمایش زیر مجموعه</button>
                    </th>
                  </tr>`;
  } else {
    rowTemplate += `<th>
          <button class="btn btn-link lnkC" data-id=${item.id} data-value="${item.title}">نمایش مشتریان</button> |
          <button class="btn btn-link lnkA" data-id=${item.id} data-value="${item.title}">نمایش مدیران حساب</button></th>
          </tr>`;
  }
  return rowTemplate;
}

function bindLinks() {
  $(".lnk").on("click", function () {
    $("#personels").block({
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
    let unitType = $(this).attr("data-value");
    let itemId = $(this).attr("data-Id");
    dataGetChildStats.campaign = currentCampaignId;
    dataGetChildStats.personnelId = currentEmployeeId;
    dataGetChildStats.unitType = unitType;

    switch (unitType) {
      case "5":
        dataGetChildStats.departmentCode = itemId;
        break;
      case "6":
        dataGetChildStats.regionCode = itemId;
        break;
      case "7":
        dataGetChildStats.areaId = itemId;
        break;
    }

    $.getJSON(getChildStatsUrl, dataGetChildStats, function (data) {
      personels.innerHTML = "";
      let Template = "";

      data.forEach(function (item) {
        let rowTemplate = createTemplate(item, unitType);
        Template += rowTemplate;
      });

      personels.innerHTML = Template;
      bindLinks();
      bindBtn();
      bindDirectAssigned();

      $("#personels").unblock();
      $('[data-toggle="tooltip"]').tooltip();
    }).fail(function (xhr, status, error) {
      console.error(status, error);
    });
  });
}

function calculateCustomerPage(param) {
  switch (param) {
    case "first":
      dataGetChildStatsC.page = 1;
      break;
    case "perviews":
      if (dataGetChildStatsC.page - 1 >= 1) {
        dataGetChildStatsC.page = dataGetChildStatsC.page - 1;
      }
      break;
    case "next":
      if (dataGetChildStatsC.page + 1 <= dataGetChildStatsC.totalPages) {
        dataGetChildStatsC.page = dataGetChildStatsC.page + 1;
      }
      break;
    case "last":
      dataGetChildStatsC.page = dataGetChildStatsC.totalPages;
      break;
  }
}

function calculatePageEmployee(param) {
  switch (param) {
    case "first":
      dataGetChildStatsA.page = 1;
      break;
    case "perviews":
      if (dataGetChildStatsA.page - 1 >= 1) {
        dataGetChildStatsA.page--;
      }
      break;
    case "next":
      if (dataGetChildStatsA.page + 1 <= dataGetChildStatsA.totalPages) {
        dataGetChildStatsA.page++;
      }
      break;
    case "last":
      dataGetChildStatsA.page = dataGetChildStatsA.totalPages;
      break;
  }
}

function bindCustomerLink() {
  $(".cLink").on("click", function () {
    let action = $(this).data("action");
    if (!$(this).parent().hasClass("disabled")) {
      getBranchInfoByCustomer(action);
    }
  });
}

function bindEmployeeLink() {
  $(".eLink").on("click", function () {
    let action = $(this).data("action");
    if (!$(this).parent().hasClass("disabled")) {
      getBranchInfoByEmployee(action);
    }
  });
}

function getBranchInfoByEmployee(action) {
  $("#emploees").block({
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

  dataGetChildStatsA.campaign = currentCampaignId;
  dataGetChildStatsA.personnelId = currentEmployeeId;
  dataGetChildStatsA.branchId = branchIdG;
  dataGetChildStatsA.minAssignmentCount = null;
  dataGetChildStatsA.maxAssignmentCount = null;

  calculatePageEmployee(action);

  $.getJSON(getBranchInfoByEmployeeUrl, dataGetChildStatsA, function (data) {
    let template = "";

    if (data.totalPages > 1) {
      pagingE.classList.add("visible");
      pagingE.classList.remove("hidden");
    } else {
      pagingE.classList.add("hidden");
      pagingE.classList.remove("visible");
    }

    eCurrentPage.innerText = data.currentPage;
    eTotalPages.innerText = data.totalPages;

    dataGetChildStatsA.totalPages = data.totalPages;

    if (data.currentPage == 1) {
      eFirst.classList.add("disabled");
      ePerviews.classList.add("disabled");
    } else {
      eFirst.classList.remove("disabled");
      ePerviews.classList.remove("disabled");
    }

    if (data.currentPage == data.totalPages) {
      eLast.classList.add("disabled");
      eNext.classList.add("disabled");
    } else {
      eLast.classList.remove("disabled");
      eNext.classList.remove("disabled");
    }

    data.items.forEach(function (item, index) {
      let rowTemplate = ` <tr>
                        <th>
                        <a href="/Dashboard/Profile/Index/${item.personnelId}"  target="_blank">
                            ${item.employeeFirstName} ${item.employeeLastName}(${item.personnelId})
                        </a>
                        </th>
                        <th>${item.assignmentCount}</th>
                        <th>${item.connectedCustomerCount}</th>
                        <th>
                        <span class="ui green label noselect" data-toggle="tooltip" data-placement="top" title="تفاهم نامه">${item.countCounteracts}</span>
                        <span class="ui teal label noselect" data-toggle="tooltip" data-placement="top" title="افزونه">${item.countPlugins}</span>
                        <span class="ui blue label noselect" data-toggle="tooltip" data-placement="top" title="نامه">${item.countLetter}</span>
                        <span class="ui purple label noselect" data-toggle="tooltip" data-placement="top" title="تلفن">${item.countTel}</span>
                        <span class="ui red label noselect" data-toggle="tooltip" data-placement="top" title="پیامک">${item.countSms}</span>
                        <span class="ui orange label noselect" data-toggle="tooltip" data-placement="top" title="جلسه">${item.countMeeting}</span>
                        <span class="ui yellow label noselect" data-toggle="tooltip" data-placement="top" title="پست الکترونیک">${item.countEmail}</span>
                        </th>
                    </tr>`;

      template += rowTemplate;
    });
    emploees.innerHTML = template;
    $('[data-toggle="tooltip"]').tooltip();
    $("#emploees").unblock();
  });
}

function getBranchInfoByCustomer(action) {
  $("#customers").block({
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

  dataGetChildStatsC.campaign = currentCampaignId;
  dataGetChildStatsC.personnelId = currentEmployeeId;
  dataGetChildStatsC.branchId = branchIdG;

  calculateCustomerPage(action);

  $.getJSON(getBranchInfoByCustomerUrl, dataGetChildStatsC, function (data) {
    let template = "";

    if (data.totalPages > 1) {
      pagingC.classList.add("visible");
      pagingC.classList.remove("hidden");
    } else {
      pagingC.classList.add("hidden");
      pagingC.classList.remove("visible");
    }

    cCurrentPage.innerHTML = data.currentPage;
    cTotalPages.innerHTML = data.totalPages;

    dataGetChildStatsC.totalPages = data.totalPages;

    if (data.currentPage == 1) {
      cFirst.classList.add("disabled");
      cPerviews.classList.add("disabled");
    } else {
      cFirst.classList.remove("disabled");
      cPerviews.classList.remove("disabled");
    }

    if (data.currentPage == data.totalPages) {
      cLast.classList.add("disabled");
      cNext.classList.add("disabled");
    } else {
      cLast.classList.remove("disabled");
      cNext.classList.remove("disabled");
    }

    data.items.forEach(function (item, index) {
      let rowTemplate = ` <tr>
                        <th>
                            <a href="/Dashboard/customer/${
                              item.customerNo
                            }" target="_blank">
                            ${item.name} ${item.family}(${item.customerNo})
                            </a>
                        </th>
                        <th>${item.assignTo}</th>
                        <th>
                        ${
                          item.employeeFirstName != null
                            ? `${item.employeeFirstName} ${item.employeeLastName}(${item.assignmentPersonnelId})`
                            : "------"
                        }
                        </th>
                        <th>${item.assignBy}</th>
                        <th>${item.assignmentStatusTitle}</th>
                        <th>
                        <span class="ui green label noselect" data-toggle="tooltip" data-placement="top" title="تفاهم نامه">${
                          item.countCounteracts
                        }</span>
                        <span class="ui teal label noselect" data-toggle="tooltip" data-placement="top" title="افزونه">${
                          item.countPlugins
                        }</span>
                        <span class="ui blue label noselect" data-toggle="tooltip" data-placement="top" title="نامه">${
                          item.countLetter
                        }</span>
                        <span class="ui purple label noselect" data-toggle="tooltip" data-placement="top" title="تلفن">${
                          item.countTel
                        }</span>
                        <span class="ui red label noselect" data-toggle="tooltip" data-placement="top" title="پیامک">${
                          item.countSms
                        }</span>
                        <span class="ui orange label noselect" data-toggle="tooltip" data-placement="top" title="جلسه">${
                          item.countMeeting
                        }</span>
                        <span class="ui yellow label noselect" data-toggle="tooltip" data-placement="top" title="پست الکترونیک">${
                          item.countEmail
                        }</span>
                        </th>
                    </tr>`;

      template += rowTemplate;
    });
    customers.innerHTML = template;

    $('[data-toggle="tooltip"]').tooltip();
    $("#customers").unblock();
  });
}

function bindBtn() {
  $(".lnkC").on("click", function () {
    $("#directAssignedCustomersPanel").removeClass("visible");
    $("#directAssignedCustomersPanel").addClass("hidden");

    branchCustomers.classList.add("visible");
    branchCustomers.classList.remove("hidden");

    branchEmployee.classList.add("hidden");
    branchEmployee.classList.remove("visible");

    $("html, body").animate(
      {
        scrollTop: $("#branchCustomerTitle").offset().top,
      },
      2000
    );

    let branchId = $(this).attr("data-Id");
    let title = $(this).attr("data-value");

    dataGetChildStatsC.campaign = currentCampaignId;
    dataGetChildStatsC.personnelId = currentEmployeeId;
    dataGetChildStatsC.branchId = branchId;
    branchIdG = branchId;

    customers.innerHTML = "";
    branchCustomerTitle.innerText = `وضعیت مشتریان ${title}(${branchId})`;

    getBranchInfoByCustomer("first");
    bindCustomerLink();
  });

  $(".lnkA").on("click", function () {
    $("#directAssignedCustomersPanel").removeClass("visible");
    $("#directAssignedCustomersPanel").addClass("hidden");

    branchEmployee.classList.add("visible");
    branchEmployee.classList.remove("hidden");

    branchCustomers.classList.add("hidden");
    branchCustomers.classList.remove("visible");

    $("html, body").animate(
      {
        scrollTop: $("#branchEmployeeTitle").offset().top,
      },
      2000
    );

    let branchId = $(this).attr("data-Id");
    let title = $(this).attr("data-value");

    dataGetChildStatsA.campaign = currentCampaignId;
    dataGetChildStatsA.personnelId = currentEmployeeId;
    dataGetChildStatsA.branchId = branchId;
    branchIdG = branchId;
    emploees.innerHTML = "";
    branchEmployeeTitle.innerText = `وضعیت مدیران حساب ${title}(${branchId})`;

    getBranchInfoByEmployee("first");
    bindEmployeeLink();
  });
}

$("#btnFilter").on("click", function () {
  $("#personels").block({
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

  dataGetChildStats.campaign = currentCampaignId;
  dataGetChildStats.personnelId = currentEmployeeId;
  dataGetChildStats.unitType = unitTypeId;
  dataGetChildStats.departmentCode = getDepartment();
  dataGetChildStats.regionCode = getRegion();
  let hozeData = getArea();
  if (hozeData != null) {
    let hozeId = hozeData.split(",");
    dataGetChildStats.areaId = hozeId[0];
  } else dataGetChildStats.areaId = null;
  dataGetChildStats.branchId = getBranch();

  $.getJSON(getChildStatsUrl, dataGetChildStats, function (data) {
    personels.innerHTML = "";
    let Template = "";
    data.forEach(function (item, index) {
      let rowTemplate = createTemplate(item, unitTypeId);
      Template += rowTemplate;
    });
    personels.innerHTML = Template;
    bindLinks();
    bindBtn();
    bindDirectAssigned();

    $("#personels").unblock();
    $('[data-toggle="tooltip"]').tooltip();
  }).fail(function (xhr, status, error) {
    console.error(status, error);
  });
});

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

function calculatePageDirect(param) {
  switch (param) {
    case "first":
      getDirectAssignmentsForRoleParam.page = 1;
      break;
    case "perviews":
      if (getDirectAssignmentsForRoleParam.page - 1 >= 1) {
        getDirectAssignmentsForRoleParam.page--;
      }
      break;
    case "next":
      if (
        getDirectAssignmentsForRoleParam.page + 1 <=
        getDirectAssignmentsForRoleParam.totalPages
      ) {
        getDirectAssignmentsForRoleParam.page++;
      }
      break;
    case "last":
      getDirectAssignmentsForRoleParam.page =
        getDirectAssignmentsForRoleParam.totalPages;
      break;
  }
}

function bindDirectAssigned() {
  $(".directAssigned").on("click", function () {
    $("#directAssignedCustomersPanel").removeClass("hidden");
    $("#directAssignedCustomersPanel").addClass("visible");

    $("#branchCustomers").removeClass("visible");
    $("#branchCustomers").addClass("hidden");

    $("#branchEmployee").removeClass("visible");
    $("#branchEmployee").addClass("hidden");

    $("#directAssignedCustomers").block({
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

    let roleTypeId = $(this).attr("data-id");
    let roleType = $(this).attr("data-value");

    let departmentCode = null,
      regionCode = null,
      areaId = null,
      branchId = null;

    switch (roleType) {
      case "5":
        departmentCode = roleTypeId;
        break;
      case "6":
        regionCode = roleTypeId;
        break;
      case "7":
        areaId = roleTypeId;
        break;
      case "8":
        branchId = roleTypeId;
        break;
    }

    getDirectAssignmentsForRoleParam.campaign = currentCampaignId;
    getDirectAssignmentsForRoleParam.departmentCode = departmentCode;
    getDirectAssignmentsForRoleParam.regionCode = regionCode;
    getDirectAssignmentsForRoleParam.areaId = areaId;
    getDirectAssignmentsForRoleParam.branchId = branchId;
    getDirectAssignmentsForRoleParam.roleType = roleType;

    getDirectAssignee("first");

    $("#directAssignedCustomers").unblock();
  });
}

function bindDirectAssigneeLink() {
  $(".dcLink").on("click", function () {
    let action = $(this).data("action");
    if (!$(this).parent().hasClass("disabled")) {
      getDirectAssignee(action);
    }
  });
}

function getDirectAssignee(action) {
  calculatePageDirect(action);

  $.getJSON(
    getDirectAssignmentsForRole,
    getDirectAssignmentsForRoleParam,
    function (data) {
      if (data.totalPages > 1) {
        pagingDC.classList.add("visible");
        pagingDC.classList.remove("hidden");
      } else {
        pagingDC.classList.add("hidden");
        pagingDC.classList.remove("visible");
      }

      dcCurrentPage.innerText = data.currentPage;
      dcTotalPages.innerText = data.totalPages;

      getDirectAssignmentsForRoleParam.totalPages = data.totalPages;

      if (data.currentPage == 1) {
        dcFirst.classList.add("disabled");
        dcPerviews.classList.add("disabled");
      } else {
        dcFirst.classList.remove("disabled");
        dcPerviews.classList.remove("disabled");
      }

      if (data.currentPage == data.totalPages) {
        dcLast.classList.add("disabled");
        dcNext.classList.add("disabled");
      } else {
        dcLast.classList.remove("disabled");
        dcNext.classList.remove("disabled");
      }

      let title = "";
      if (data.items.length > 0)
        title = `${data.items[0].jobTitle || ""}: ${
          data.items[0].employeeFullName || ""
        } (${data.items[0].personnelId || ""})`;

      directAssignedCustomerTitle.innerHTML = title;
      let template = "";

      data.items.forEach(function (item) {
        let rowTemplate = ` <tr>
                          <th>
                              <a href="/Dashboard/customer/${item.customerNo}" target="_blank">
                              ${item.customerFullName}(${item.customerNo})
                              </a>
                          </th>                            
                          <th>
                              <span class="ui blue label noselect" data-toggle="tooltip" data-placement="top" title="نامه">${item.countLetter}</span>
                              <span class="ui purple label noselect" data-toggle="tooltip" data-placement="top" title="تلفن">${item.countTel}</span>
                              <span class="ui red label noselect" data-toggle="tooltip" data-placement="top" title="پیامک">${item.countSms}</span>
                              <span class="ui orange label noselect" data-toggle="tooltip" data-placement="top" title="جلسه">${item.countMeeting}</span>
                              <span class="ui yellow label noselect" data-toggle="tooltip" data-placement="top" title="پست الکترونیک">${item.countEmail}</span>
                          </th>
                      </tr>`;

        template += rowTemplate;
      });
      directAssignedCustomers.innerHTML = template;
      bindDirectAssigneeLink();
    }
  );
}

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

  campaignsInit();
  $(bindLinks());
  bindDirectAssigned();
}

init();
