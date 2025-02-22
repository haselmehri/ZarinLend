$(function () {
  campaignsInit();

  $("#btnCampaignNext").on("click", function () {
    let campaignIsValid = validateCampaign();

    if (campaignIsValid) {
      accountManagersInit();
      if (setError) {
        campaignStep.classList.remove("danger");
        alertElement.innerText = "";
        alertElement.classList.remove("visible");
        alertElement.classList.add("hidden");

        setError = false;
      } else {
        campaignStep.classList.remove("current");
      }
      campaignStep.classList.add("active");
      accountStep.classList.add("current");

      campaignPanel.classList.add("hidden");
      accountPanel.classList.remove("hidden");
      accountPanel.classList.add("visible");

      let campaign = campaigns.options[campaigns.selectedIndex].innerText;

      let cmpg = document.createElement("li");
      cmpg.classList.add("breadcrumb-item");
      cmpg.innerHTML = `کمپین: ${campaign}`;
      cmpg.id = "cmpg-info";
      nvs.appendChild(cmpg);
    } else {
      campaignStep.classList.remove("current");
      campaignStep.classList.add("danger");

      alertElement.innerText = cerror;
      alertElement.classList.remove("hidden");
      alertElement.classList.add("visible");
      setError = true;
    }
  });

  $("#btnBackToCampaign").on("click", function () {
    accountManagers.options.selectedIndex = 0;

    selectedE.classList.remove("visible");
    selectedE.classList.add("hidden");

    employeeData.innerHTML = "";
    selectedEmployee.value = 0;
    btnAccFilter.innerHTML = "<i class='fal fa-filter'></i> &nbsp; فیلتر";

    angleEmp.classList.add("hidden");
    angleEmp.classList.remove("visible");

    employeeFilterPanel.classList.remove("visible");
    employeeFilterPanel.classList.add("hidden");

    campaignStep.classList.remove("active");
    campaignStep.classList.add("current");
    accountStep.classList.remove("current");

    accountPanel.classList.remove("visible");
    accountPanel.classList.add("hidden");
    campaignPanel.classList.remove("hidden");
    campaignPanel.classList.add("visible");

    nvs.removeChild(document.getElementById("cmpg-info"));
  });

  $("#btnNextToCustomer").on("click", function () {
    validateAccountStep();
    if (setError == false) {
      let accountManagerData =
        accountManagers.options[accountManagers.options.selectedIndex].value;

      if (accountManagerData == 8) {
        let employeeData = document.getElementById("employeeData");
        let emp = document.createElement("li");
        emp.classList.add("breadcrumb-item");
        emp.innerHTML = `کاربر: ${employeeData.innerText}`;
        emp.id = "emp-info";
        nvs.appendChild(emp);

        GetCustomerDataByEmployee();
      } else {
        let emp = document.createElement("li");
        emp.classList.add("breadcrumb-item");
        switch (accountManagerData) {
          case "2":
            emp.innerHTML = `مدیر عامل`;
            break;
          case "3":
            emp.innerHTML = `معاونت`;
            break;
          case "4":
            emp.innerHTML = `مدیر امور`;
            break;
          case "5":
            emp.innerHTML = `مدیر منطقه`;
            break;
          case "6":
            emp.innerHTML = `رئیس حوزه`;
            break;
          case "7":
            emp.innerHTML = `رئیس شبه`;
            break;
        }
        emp.id = "emp-info";
        nvs.appendChild(emp);
        GetCustomerDataByRole();
      }

      accountStep.classList.remove("current");
      accountStep.classList.add("active");
      customerStep.classList.add("current");

      accountPanel.classList.remove("visible");
      accountPanel.classList.add("hidden");

      customerPanel.classList.remove("hidden");
      customerPanel.classList.add("visible");
    }
  });

  $("#btnBackToAccountStep").on("click", function () {
    customerDataInfo = [];
    customerData = [];

    customerStep.classList.remove("current");
    accountStep.classList.remove("active");
    accountStep.classList.add("current");

    customerPanel.classList.remove("visible");
    customerPanel.classList.add("hidden");

    accountPanel.classList.remove("hidden");
    accountPanel.classList.add("visible");
    nvs.removeChild(document.getElementById("emp-info"));
  });

  $("#btnSave").on("click", function () {
    let accountManagerData =
      accountManagers.options[accountManagers.options.selectedIndex].value;
    dataRequestAssign.ids = customerData.join(",");
    dataRequestAssign.status = 2;
    dataRequestAssign.byPersonnelId = currentEmployeeId;
    dataRequestAssign.toRole = accountManagerData;

    if (accountManagerData == 8) {
      var selectedEmployee = $("#selectedEmployee").val();
      dataRequestAssign.toPersonnelId = selectedEmployee;
    } else dataRequestAssign.toPersonnelId = null;

    $.post(postAssignment, dataRequestAssign, function (data, status) {
      if ("success") {
        $.each(customerDataInfo, function (i, item) {
          let li = document.createElement("li");
          li.innerHTML = item;
          selectedCustomers.appendChild(li);
        });
        employeeSelected.innerHTML = employeeData.innerHTML;
        selectedCampaign.innerHTML =
          campaigns.options[campaigns.options.selectedIndex].innerText;
        customerStep.classList.remove("current");
        customerStep.classList.add("active");

        customerPanel.classList.remove("visible");
        customerPanel.classList.add("hidden");

        finalPanel.classList.remove("hidden");
        finalPanel.classList.add("visible");
      }
    });
  });
});
