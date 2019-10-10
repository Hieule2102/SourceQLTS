$(document).ready(function () {
        
    $('.login-close').click(function () {
        $('#login').removeClass('login-show');
    });

    $(".tablinks").click(function () {
        $(".xk-dc").removeClass("d-none");
        $(".div-cauhinh").removeClass("d-none");
    });
    $("#chct").click(function () {
        $(".cau-hinh-chi-tiet").removeClass("d-none");
    });

    $(".close-cauhinh").click(function () {
        $(".cau-hinh-chi-tiet").addClass("d-none");
    });

    $('.xn').click(function () {
        $('.div-ttxn').addClass('show-ttxn');
        $.ajax({
            type: "GET",
            url: "/Script/get_ThongTinDieuChuyen",
            data: { ma_DIEU_CHUYEN: ma_DIEU_CHUYEN },
            success: function (data) {
                $("#MATB").html(data.MATB);
                $("#TENTB").html(data.TENTB);
                $("#NGAY_CHUYEN").html(data.NGAY_CHUYEN);
                $("#DV_QL").html(data.DV_QL);
                $("#DV_NHAN").html(data.DV_NHAN);
                $("#MAND_THUC_HIEN").html(data.MAND_THUC_HIEN);
                $("#GHI_CHU").html(data.GHI_CHU);
            }
        });
    });

    $("#huy-xn").click(function () {
        $(".div-ttxn").removeClass("show-ttxn");
    });

    $(".btn_edit").click(function () {
        $(".them").addClass("d-none");
        $(".sua").removeClass("d-none");
        $(".reset").removeClass("d-none");
        $("#them-sua").html("Sửa");
        $("#tb-sm").addClass("d-none");
    });

    if ($("body").width() <= 768) {
        $(".tablinks").click(function () {
            $(".cau-hinh-chi-tiet").removeClass("d-none");
        });
    }
});

    

function btn_Edit(evt, row) {
    var i, btn_edit, tablinks;
    btn_edit = document.getElementsByClassName("btn_edit");
    for (i = 0; i < btn_edit.length; i++) {
      btn_edit[i].style.display = "none";
    }
    tablinks = document.getElementsByClassName("tablinks");
    for (i = 0; i < tablinks.length; i++) {
      tablinks[i].className = tablinks[i].className.replace(" active", "");
    }
    document.getElementById(row).style.display = "block";
    evt.currentTarget.className += " active";
  };

  function Active(evt) {
    var i, tablinks;
    
    tablinks = document.getElementsByClassName("tablinks");
    for (i = 0; i < tablinks.length; i++) {
      tablinks[i].className = tablinks[i].className.replace(" active", "");
    }
    
    evt.currentTarget.className += " active";
  };

//input file
(function (e, t, n) {
    var r = e.querySelectorAll("html")[0];
    r.className = r.className.replace(/(^|\s)no-js(\s|$)/, "$1js$2");
})(document, window, 0);

(function (document, window, index) {
    var inputs = document.querySelectorAll(".inputfile");
    Array.prototype.forEach.call(inputs, function (input) {
        var label = input.nextElementSibling,
            labelVal = label.innerHTML;

        input.addEventListener("change", function (e) {
            var fileName = "";
            if (this.files && this.files.length > 1)
                fileName = (
                    this.getAttribute("data-multiple-caption") || ""
                ).replace("{count}", this.files.length);
            else fileName = e.target.value.split("\\").pop();

            if (fileName) label.querySelector("span").innerHTML = fileName;
            else label.innerHTML = labelVal;
        });

        // Firefox bug fix
        input.addEventListener("focus", function () {
            input.classList.add("has-focus");
        });
        input.addEventListener("blur", function () {
            input.classList.remove("has-focus");
        });
    });
})(document, window, 0);
