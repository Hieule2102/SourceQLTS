$(document).ready(function () {
    $('.nav-icon').click(function () {
        $('.nav-icon').toggleClass('nav-close');
    });
    $('.nav-icon').click(function () {
        $('header').toggleClass('header-show');
    });
    $('.nav-icon').click(function () {
        $('.navbar').toggleClass('navbar-show');
    });
    $('.nav-icon').click(function () {
        $('.nav').toggleClass('d-none');
    });


    $('.login-close').click(function () {
        $('#login').removeClass('login-show');
    });

    $('#chct').click(function () {
        $('.cau-hinh-chi-tiet').addClass('cau-hinh-chi-tiet-show');
    });
    $('.close-cauhinh').click(function () {
        $('.cau-hinh-chi-tiet').removeClass('cau-hinh-chi-tiet-show');
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
});

    // Accordion
    function close_accordion_section() {
        $('.accordion .accordion-section-title').removeClass('active');
        $('.accordion .accordion-section-content').slideUp(300).removeClass('open');
    }

    $('.accordion-section-title').click(function (e) {
        // Grab current anchor value
        var currentAttrValue = $(this).attr('href');

        if ($(e.target).is('.active')) {
            close_accordion_section();
        } else {
            close_accordion_section();

            // Add active class to section title
            $(this).addClass('active');
            // Open up the hidden content panel
            $('.accordion ' + currentAttrValue).slideDown(300).addClass('open');
        }

        e.preventDefault();
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

