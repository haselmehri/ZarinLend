$(function () {
  accountManagers.addEventListener("change", function () {
    if (this.value === "8") {
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

      if (setadEmployeeFilterPanel.classList.contains("visible")) {
        setadEmployeeFilterPanel.classList.remove("visible");
        setadEmployeeFilterPanel.classList.add("hidden");
      }
    } else if (this.value === "30") {
      selectedE.classList.remove("visible");
      selectedE.classList.add("hidden");

      setadEmployeeFilterPanel.classList.remove("hidden");
      setadEmployeeFilterPanel.classList.add("visible");

      if (employeeFilterPanel.classList.contains("hidden") === false) {
        employeeFilterPanel.classList.remove("visible");
        employeeFilterPanel.classList.add("hidden");
      }
    } else {
      if (setadEmployeeFilterPanel.classList.contains("visible")) {
        setadEmployeeFilterPanel.classList.remove("visible");
        setadEmployeeFilterPanel.classList.add("hidden");
      }

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
  });

  btnFindSetadEmployee.addEventListener("click", function (e) {
    let employee = txtSetadEmployeeId.value;
    let url = `${getSetadEmployee}?personnelId=${employee}`;

    btnFindSetadEmployee.setAttribute("disabled", "disabled");

    $.getJSON(url, function (data) {
      if (data && data.access === true) {
        setadNotAccess.classList.remove("visible");
        setadNotAccess.classList.add("hidden");

        setadEmployeeFilterResultPanel.classList.remove("hidden");
        setadEmployeeFilterResultPanel.classList.add("visible");

        setadEmployeeName.innerHTML = `${data.fullName} - ${data.jobTitle} (${employee})`;
        setadEmployeeDepartment.innerHTML = data.departmentName;
        setadEmployeeRegion.innerHTML = data.regionName;
        setadEmployeeBranch.innerHTML = data.branchName;

        btnSelectSetadEmployee.setAttribute("data-name", data.fullName);
        btnSelectSetadEmployee.setAttribute("data-jobTitle", data.jobTitle);
        btnSelectSetadEmployee.setAttribute("data-id", employee);
      } else {
        setadEmployeeFilterResultPanel.classList.add("hidden");
        setadEmployeeFilterResultPanel.classList.remove("visible");

        setadNotAccess.classList.remove("hidden");
        setadNotAccess.classList.add("visible");
      }
      btnFindSetadEmployee.removeAttribute("disabled");
    });
  });

  btnSelectSetadEmployee.addEventListener("click", function (e) {
    console.log(e.target);
    let name = e.target.getAttribute("data-name");
    let jobTitle = e.target.getAttribute("data-jobTitle");
    let id = e.target.getAttribute("data-id");

    let txt = `${name} - ${jobTitle} (${id})`;

    employeeData.innerHTML = txt;
    selectedEmployee.value = id;

    setadEmployeeFilterPanel.classList.add("hidden");
    setadEmployeeFilterPanel.classList.remove("visible");

    selectedE.classList.remove("hidden");
    selectedE.classList.add("visible");
  });

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

  btnAccFilter.addEventListener("click", function () {
    let omurِData = omurFilter.options[omurFilter.options.selectedIndex].value;
    let mantagheData =
      mantagheFilter.options[mantagheFilter.options.selectedIndex].value;

    let hozeData = -1;
    if (hozeFilter.options[hozeFilter.options.selectedIndex].value != -1) {
      let tmpHozeData =
        hozeFilter.options[hozeFilter.options.selectedIndex].value.split(",");
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
