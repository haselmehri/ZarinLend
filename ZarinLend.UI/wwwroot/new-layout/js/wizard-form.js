//    Wizard tabs with icons setup
// ------------------------------
$(".wizard-horizontal").steps({
    headerTag: "h6",
    bodyTag: "fieldset",
    transitionEffect: "fade",
    titleTemplate:
        '<span class="step d-flex align-items-center justify-content-center">#index#</span> #title#',
    labels: {
        cancel: "انصراف",
        current: "قدم کنونی:",
        pagination: "صفحه بندی",
        finish: "ثبت",
        next: "بعدی",
        previous: "قبلی",
        loading: "در حال بارگذاری ...",
    },
    onFinished: function (event, currentIndex) {
        alert("فرم ثبت شد.");
    },
});

// Icon change on state

// if click on next button icon change
$(".actions [href='#next']").click(function () {
    $(".done")
        .find(".step-icon")
        .removeClass("bx bx-time-five")
        .addClass("bx bx-check-circle");
    $(".current")
        .find(".step-icon")
        .removeClass("bx bx-check-circle")
        .addClass("bx bx-time-five");
    // live icon color change on next button's on click
    $(".current").find(".fonticon-wrap .livicon-evo").updateLiviconEvo({
        strokeColor: "#17c3b2",
    });
    $(".current")
        .prev("li")
        .find(".fonticon-wrap .livicon-evo")
        .updateLiviconEvo({
            strokeColor: "#39DA8A",
        });
});

$(".actions [href='#previous']").click(function () {
    // live icon color change on next button's on click
    $(".current").find(".fonticon-wrap .livicon-evo").updateLiviconEvo({
        strokeColor: "#17c3b2",
    });
    $(".current")
        .next("li")
        .find(".fonticon-wrap .livicon-evo")
        .updateLiviconEvo({
            strokeColor: "#adb5bd",
        });
});

// if click on  submit   button icon change
$(".actions [href='#finish']").click(function () {
    $(".done")
        .find(".step-icon")
        .removeClass("bx-time-five")
        .addClass("bx bx-check-circle");
    $(".last.current.done").find(".fonticon-wrap .livicon-evo").updateLiviconEvo({
        strokeColor: "#39DA8A",
    });
});

// add primary btn class
$('.actions a[role="menuitem"]').addClass("btn btn-zl-primary");
$('.icon-tab [role="menuitem"]').addClass("glow ");
$('.wizard-vertical [role="menuitem"]')
    .removeClass("btn-zl-primary")
    .addClass("btn-light-primary");
