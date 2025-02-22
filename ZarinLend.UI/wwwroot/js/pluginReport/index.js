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

function parseParams(querystring) {
  // parse query string
  const params = new URLSearchParams(querystring);

  const obj = {};

  // iterate over all keys
  for (const key of params.keys()) {
    if (params.getAll(key).length > 1) {
      obj[key] = params.getAll(key);
    } else {
      obj[key] = params.get(key);
    }
  }

  return obj;
}

function campaignsInit(campaign) {
  campaigns.innerHTML = "<option value='-1'>انتخاب کنید</option>";

  GetDropDown(campaigns, campaginsUrl, campaign);
}

function pluginsInit(pluginId) {
  plugin.innerHTML = "<option value='-1'>انتخاب کنید</option>";

  GetDropDown(plugin, getPluginsUrl, pluginId);
}

function omurInit(omurId) {
  omur.innerHTML = "<option value='-1'>انتخاب کنید</option>";
  GetDropDown(omur, omurUrl, omurId);
  mantaghe.innerHTML = "<option value='-1'>انتخاب کنید</option>";
  hoze.innerHTML = "<option value='-1'>انتخاب کنید</option>";
  branch.innerHTML = "<option value='-1'>انتخاب کنید</option>";
}

function mantagheInit(omurId, mantagheId) {
  mantaghe.innerHTML = "<option value='-1'>انتخاب کنید</option>";
  let tmpMantagheUrl = mantagheUrl + omurId;
  GetDropDown(mantaghe, tmpMantagheUrl, mantagheId);
  hoze.innerHTML = "<option value='-1'>انتخاب کنید</option>";
  branch.innerHTML = "<option value='-1'>انتخاب کنید</option>";
}

function hozeInit(mantagheId, hozeId) {
  hoze.innerHTML = "<option value='-1'>انتخاب کنید</option>";
  let tmpHozeUrl = hozeUrl + mantagheId;
  let areaId = null;

  $.getJSON(tmpHozeUrl, function (data) {
    data.forEach(function (item, index) {
      let option = document.createElement("option");
      option.text = item.name;
      option.value = item.id;
      if (item.id.includes(hozeId)) areaId = item.id;
      hoze.appendChild(option);
    });
  });

  if (areaId !== undefined) hoze.value = areaId;
  branch.innerHTML = "<option value='-1'>انتخاب کنید</option>";
}

function branchInit(hozeId, branchId) {
  branch.innerHTML = "<option value='-1'>انتخاب کنید</option>";

  let tmpBranchUrl = branchUrl + hozeId;
  GetDropDown(branch, tmpBranchUrl, branchId);
}

const bindCombos = () => {
  omur.addEventListener("change", function () {
    let omurData = this.value;

    let tmpMantagheUrl = mantagheUrl + omurData;
    mantaghe.innerHTML = "<option value='-1'>انتخاب کنید</option>";
    GetDropDown(mantaghe, tmpMantagheUrl);
    mantaghe.options.selectedIndex = 0;

    hoze.innerHTML = "<option value='-1'>انتخاب کنید</option>";
    branch.innerHTML = "<option value='-1'>انتخاب کنید</option>";
  });

  mantaghe.addEventListener("change", function () {
    let mantagheData = this.value;
    hoze.innerHTML = "<option value='-1'>انتخاب کنید</option>";

    if (mantagheData == "-1") {
      unitTypeId = 5;
    } else {
      unitTypeId = 6;
    }

    let tmpHozeUrl = hozeUrl + mantagheData;
    GetDropDown(hoze, tmpHozeUrl);
    hoze.options.selectedIndex = 0;
    branch.innerHTML = "<option value='-1'>انتخاب کنید</option>";
  });

  hoze.addEventListener("change", function () {
    let hozeData = this.value;
    let hozeId = hozeData.split(",");
    if (hozeData == "-1") {
      unitTypeId = 6;
    } else {
      unitTypeId = 7;
    }
    let tmpBranchUrl = branchUrl + hozeId[0];
    GetDropDown(branch, tmpBranchUrl);
    branch.options.selectedIndex = 0;
  });

  branch.addEventListener("change", function () {
    let branchData = this.value;
    if (branchData == "-1") {
      unitTypeId = 7;
    } else {
      unitTypeId = 8;
    }
  });
};

function preInit() {
  campaignsInit();
  omurInit();
  pluginsInit();
}
function pageInit() {
  $.ajaxSetup({
    async: false,
  });

  preInit();

  let searchParam = parseParams(search);
  Object.keys(searchParam).map((key) => {
    console.log(key, searchParam[key]);
    switch (key) {
      case "campaign":
        campaignsInit(searchParam[key]);
        break;
      case "departmentCode":
        omurInit(searchParam[key]);
        break;
      case "regionCode":
        mantagheInit(searchParam["departmentCode"], searchParam[key]);
        break;
      case "areaId":
        hozeInit(searchParam["regionCode"], searchParam[key]);
        break;
      case "branchId":
        branchInit(searchParam["areaId"], searchParam[key]);
        break;
      case "pluginId":
        pluginsInit(searchParam[key]);
        break;
      case "assignedBy":
        assignedBy.value = searchParam[key];
        break;
      case "customerNo":
        customerNumber.value = searchParam[key];
        break;
      // case "assignedDate":
      //   pluginsInit(searchParam[key]);
      //   break;
    }
  });

  $.ajaxSetup({
    async: true,
  });

  bindCombos();
}

pageInit();
