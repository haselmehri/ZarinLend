function GetDropDown(base, url, selectedItem) {
  $.getJSON(url, function (data) {
    data.forEach(function (item, index) {
      let option = document.createElement("option");
      option.text = item.name;
      option.value = item.id;

      if (selectedItem !== undefined && item.id == selectedItem)
        option.selected = true;

      base.appendChild(option);
    });
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
