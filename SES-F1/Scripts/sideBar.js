﻿    $("input[type='image']").click(function () {
        $("input[id='my_file']").click();
    });
        $("#menu-toggle").click(function (e) {
        e.preventDefault();
    $("#wrapper").toggleClass("toggled");
        });