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
});
