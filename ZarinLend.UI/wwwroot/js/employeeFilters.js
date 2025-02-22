function filterOmur(current = 0) {
  omurFilter.innerHTML = "<option value='-1'>انتخاب کنید</option>";

  $.getJSON(omurUrl, function (data) {
    data.forEach(function (item, index) {
      let option = document.createElement("option");
      option.text = item.name;
      option.value = item.id;

      if (current > 0 && item.id == current) {
        option.selected = true;
      }

      omurFilter.appendChild(option);
    });
  });
}

function filterMantaghe(omurId, current = -1) {
  mantagheFilter.innerHTML = "<option value='-1'>انتخاب کنید</option>";
  hozeFilter.innerHTML = "<option value='-1'>انتخاب کنید</option>";
  branchFilter.innerHTML = "<option value='-1'>انتخاب کنید</option>";

  if (omurId !== -1) {
    let tmpMantagheUrl = mantagheUrl + omurId;

    $.getJSON(tmpMantagheUrl, function (data) {
      data.forEach(function (item, index) {
        let mantagheOpt = document.createElement("option");
        mantagheOpt.text = item.name;
        mantagheOpt.value = item.id;

        if (current > 0 && item.id == current) {
          mantagheOpt.selected = true;
        }

        mantagheFilter.appendChild(mantagheOpt);
      });
    });
  }
}
