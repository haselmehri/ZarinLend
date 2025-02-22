(function ($) {
  $.fn.tagManager = function (settings) {
    settings = $.extend(
      {
        className: "pill-tag pill-gray",
        defaultTags: "",
      },
      settings
    );

    var data = new Array();
    var tagListId;
    var elementId;

    function serialize() {
      let returnData = "";

      data.forEach((element) => {
        returnData += element + ";";
      });

      var str = returnData.substring(0, returnData.length - 1);
      return str;
    }

    function addTag(el) {
      var inputValue = el.value;
      //inputValue = inputValue.replace(/\s+/g, "_");
      let tagList = document.getElementById(tagListId);

      if (inputValue !== "") {
        if (!arrayContains(data, inputValue)) {
          var tag = `<li class='${settings.className}' data-value="${inputValue}">${inputValue}&nbsp;<a data-dismiss="tag">&times;</a></li>`;
          tagList.innerHTML += tag;
          data.push(inputValue);
          el.value = "";
          tagClickAction();
          let obj = serialize();
          eventRise(obj);
        }
      }
    }

    function uuid() {
      return "xxxxxxxxxxxx".replace(/[xy]/g, function (c) {
        var r = (Math.random() * 16) | 0,
          v = c === "x" ? r : (r & 0x3) | 0x8;
        return v.toString(16);
      });
    }
    function arrayRemove(arr, value) {
      return arr.filter(function (ele) {
        return ele !== value;
      });
    }

    function arrayContains(arr, value) {
      return arr.indexOf(value) > -1;
    }

    function eventRise(obj) {
      let element = document.getElementById(elementId);

      element.dispatchEvent(
        new CustomEvent("bt.tag.serialized", {
          bubbles: true,
          detail: {
            data: obj,
          },
        })
      );
    }

    function tagClickAction() {
      let elements = document.querySelectorAll('[data-dismiss="tag"]');
      console.log(elements);

      for (const item of elements) {
        item.addEventListener("click", function () {
          var parent = this.parentElement;

          var val = parent.getAttribute("data-value");
          if (arrayContains(data, val)) {
            data = arrayRemove(data, val);
            let obj = serialize();
            eventRise(obj);
            parent.remove();
          }
        });
      }
    }

    function parsTags(tags) {
      let result = tags.split(";");
      let tagList = document.getElementById(tagListId);

      result.forEach(function (e) {
        var tag = `<li class='${settings.className}' data-value="${e}">${e}&nbsp;<a data-dismiss="tag">&times;</a></li>`;
        tagList.innerHTML += tag;
        data.push(e);
      });

      tagClickAction();
    }

    return this.each(function () {
      tagListId = uuid();
      elementId = this.id;

      var tagTemplate = `<ul id="${tagListId}"> </ul>`;

      $(tagTemplate).insertAfter(this).addClass("tagList");

      if (settings.defaultTags !== "") {
        parsTags(settings.defaultTags);
      }

      if (this.addEventListener) {
        this.addEventListener(
          "keypress",
          function (e) {
            if (e.key === ";") {
              e.preventDefault();
              addTag(this);
            } else if (e.key === "Enter") {
              e.preventDefault();
              addTag(this);
            }
          },
          false
        );

        this.addEventListener("blur", function () {
          let obj = serialize();
          eventRise(obj);
        });
      }
    });
  };
})(jQuery);
