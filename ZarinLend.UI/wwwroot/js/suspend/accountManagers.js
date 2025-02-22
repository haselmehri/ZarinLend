$(function () {
  accountManagers.addEventListener("change", function () {
    if (parseInt(this.value) == 8) {
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

      employeeFilterPanel.classList.remove("hidden");
      employeeFilterPanel.classList.add("visible");
    } else {
      removeEmployeeFilter();
    }
  });

  function removeEmployeeFilter() {
    selectedE.classList.remove("visible");
    selectedE.classList.add("hidden");

    employeeData.innerHTML = "";
    selectedEmployee.value = 0;
    btnAccFilter.innerHTML = "<i class='fal fa-filter'></i> &nbsp; فیلتر";

    angleEmp.classList.add("hidden");
    angleEmp.classList.remove("visible");

    if (employeeFilterPanel.classList.contains("hidden") === false) {
      employeeFilterPanel.classList.remove("visible");
      employeeFilterPanel.classList.add("hidden");
    }
  }

  function getMngDepartment() {
    let value = $("#mngOmurFilter").val();
    if (value >= 0) return value;
    return null;
  }

  function getMngRegion() {
    let value = $("#mngMantagheFilter").val();
    if (value >= 0) return value;
    return null;
  }

  function getMngArea() {
    let value = $("#mngHozeFilter").val();
    if (value == "-1") return null;
    return value;
  }

  function getMngBranch() {
    let value = $("#mngBranchFilter").val();
    if (value >= 0) return value;
    return null;
  }

  omurFilter.addEventListener("change", function () {
    mantagheFilter.innerHTML = "<option value='-1'>انتخاب کنید</option>";
    hozeFilter.innerHTML = "<option value='-1'>انتخاب کنید</option>";
    branchFilter.innerHTML = "<option value='-1'>انتخاب کنید</option>";

    if (this.value !== -1) {
      let tmpMantagheUrl = mantagheUrl + this.value;

      GetDropDown(mantagheFilter, tmpMantagheUrl);
    }
  });

  mantagheFilter.addEventListener("change", function () {
    hozeFilter.innerHTML = "<option value='-1'>انتخاب کنید</option>";
    branchFilter.innerHTML = "<option value='-1'>انتخاب کنید</option>";

    if (this.value !== -1) {
      let tmpHozeUrl = hozeUrl + this.value;

      GetDropDown(hozeFilter, tmpHozeUrl);
    }
  });

  hozeFilter.addEventListener("change", function () {
    branchFilter.innerHTML = "<option value='-1'>انتخاب کنید</option>";

    if (this.value !== -1) {
      var tmp = this.value.split(",");
      let tmpBranchUrl = branchUrl + tmp[0];

      GetDropDown(branchFilter, tmpBranchUrl);
    }
  });

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

    GetEmployeeData("first");
  });

  $(".p-link").on("click", function () {
    let action = $(this).data("action");
    if (!$(this).parent().hasClass("disabled")) GetEmployeeData(action);
  });

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
/*
 var campaignValue =
        campaigns.options[campaigns.options.selectedIndex].value;

      dataRequestCustomer.roleType = parsInt(currentEmployeeRole) + 1;
      dataRequestCustomer.campaign = parsInt(campaignValue);
      dataRequestCustomer.personnelId = parseInt(currentEmployeeId);

      if (parseInt(this.value) > 3 && parseInt(this.value) < 8) {
        dataRequestCustomer.departmentCode = getMngDepartment();
        dataRequestCustomer.regionCode = getMngRegion();
        let areaId = getMngArea();
        if (areaId != null) {
          let area = areaId.split(",");
          dataRequestCustomer.areaId = area[0];
        } else {
          dataRequestCustomer.areaId = areaId;
        }
        dataRequestCustomer.branchId = getMngBranch();
      }
*/
