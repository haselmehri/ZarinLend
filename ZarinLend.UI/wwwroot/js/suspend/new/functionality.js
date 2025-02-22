function campaignsInit() {
  campaigns.innerHTML = "<option value='-1'>انتخاب کنید</option>";
  GetDropDown(campaigns, campaginsUrl);
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
  } else if (accountManagerData == 8 && selectedEmployeeData == "0") {
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

function accountManagersInit() {
  accountManagers.innerHTML = "<option value='-1'>انتخاب کنید</option>";
  GetDropDown(accountManagers, accountManagersUrl);
}
