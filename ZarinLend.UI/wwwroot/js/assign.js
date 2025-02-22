campaigns.innerHTML = "<option value='-1'>انتخاب کنید</option>";
accountManagers.innerHTML = "<option value='-1'>انتخاب کنید</option>";

$(function () {
  function split(val) {
    return val.split(/,\s*/);
  }

  function extractLast(term) {
    return split(term).pop();
  }

  $.getJSON(campaginsUrl, function (data) {
    data.forEach(function (item, index) {
      let option = document.createElement("option");
      option.text = item.title;
      option.value = item.id;
      campaigns.appendChild(option);
    });
  }).fail(function (xhr, status, error) {
    console.log(status, error);
  });

  $.getJSON(accManagerUrl, function (data) {
    data.forEach(function (item, index) {
      let option = document.createElement("option");
      option.text = item.title;
      option.value = item.id;
      accountManagers.appendChild(option);
    });
  }).fail(function (xhr, status, error) {
    console.log(status, error);
  });
  accountManagers.addEventListener("change", function () {
    if (this.value === "8") {
      switch (currentEmployeeRole) {
        case 4:
          break;
        case 5:
          break;
        case 6:
          break;
        case 7:
          break;
      }
      omurFilter.options.selectedIndex = 0;
      mantagheFilter.innerHTML = "<option value='-1'>انتخاب کنید</option>";
      hozeFilter.innerHTML = "<option value='-1'>انتخاب کنید</option>";
      branchFilter.innerHTML = "<option value='-1'>انتخاب کنید</option>";

      employee.classList.remove("hidden");
      employee.classList.add("visible");
    } else {
      selectedE.classList.remove("visible");
      selectedE.classList.add("hidden");

      employeeData.innerHTML = "";
      selectedEmployee.value = 0;
      btnAccFilter.innerHTML = "<i class='fal fa-filter'></i> &nbsp; فیلتر";

      angleEmp.classList.add("hidden");
      angleEmp.classList.remove("visible");

      if (employee.classList.contains("hidden") === false) {
        employee.classList.remove("visible");
        employee.classList.add("hidden");
      }
    }
  });
  // $("#EmployeeSearch")
  //   .autocomplete({
  //     source: function (request, response) {
  //       $.ajax({
  //         url: searchEmployeeUrl,
  //         data: { query: request.term },
  //         dataType: "json",
  //         type: "GET",
  //         success: response,
  //         error: function () {
  //           response([]);
  //         },
  //       });
  //     },
  //     search: function () {
  //       // custom minLength
  //       var term = extractLast(this.value);
  //       if (term.length < 2) {
  //         return false;
  //       }
  //     },
  //     select: function (event, ui) {
  //       let selectedPerson = ui.item;
  //       let personnelId = selectedPerson.personnelId;
  //       $("#EmployeeSearch").val(personnelId);
  //       employeeInfo.innerText = `${selectedPerson.firstName} ${selectedPerson.family} (${selectedPerson.personnelId})`;
  //       let g = item.firstName;
  //     },
  //   })
  //   .autocomplete("instance")._renderItem = function (ul, item) {
  //   return $("<li>")
  //     .append(
  //       "<div>نام :" +
  //         item.firstName +
  //         " " +
  //         item.family +
  //         "<br> کد پرسنلی :" +
  //         item.personnelId +
  //         "<br> سمت :" +
  //         item.jobTitle +
  //         " </div>"
  //     )
  //     .appendTo(ul);
  // };
});

function toaccountStep() {
  let campaginValue = campaigns.options[campaigns.selectedIndex].value;

  if (campaginValue <= 0) {
    campaignStep.classList.remove("current");
    campaignStep.classList.add("danger");

    alertElement.innerText = cerror;
    alertElement.classList.remove("hidden");
    alertElement.classList.add("visible");
    setError = true;
  } else {
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
  }
}

function toCampaignStep() {
  accountManagers.options.selectedIndex = 0;

  selectedE.classList.remove("visible");
  selectedE.classList.add("hidden");

  employeeData.innerHTML = "";
  selectedEmployee.value = 0;
  btnAccFilter.innerHTML = "<i class='fal fa-filter'></i> &nbsp; فیلتر";

  angleEmp.classList.add("hidden");
  angleEmp.classList.remove("visible");

  employee.classList.remove("visible");
  employee.classList.add("hidden");

  campaignStep.classList.remove("active");
  campaignStep.classList.add("current");
  accountStep.classList.remove("current");

  accountPanel.classList.remove("visible");
  accountPanel.classList.add("hidden");
  campaignPanel.classList.remove("hidden");
  campaignPanel.classList.add("visible");
}

function toCustomerStep() {
  validateAccountStep();
  if (setError == false) {
    accountStep.classList.remove("current");
    accountStep.classList.add("active");
    customerStep.classList.add("current");

    accountPanel.classList.remove("visible");
    accountPanel.classList.add("hidden");

    customerPanel.classList.remove("hidden");
    customerPanel.classList.add("visible");
  }
}

function toAccountStep() {
  customerStep.classList.remove("current");
  accountStep.classList.remove("active");
  accountStep.classList.add("current");

  customerPanel.classList.remove("visible");
  customerPanel.classList.add("hidden");

  accountPanel.classList.remove("hidden");
  accountPanel.classList.add("visible");
}
