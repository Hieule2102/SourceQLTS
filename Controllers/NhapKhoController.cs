@model IEnumerable<Source.Models.NHAP_KHO>

@{
    ViewBag.Title = "Index";
}

<main>
    <div class="container link">
        <p>Trang chủ > Chức năng > Nhập kho</p>
        <hr />
    </div>
    @using (Html.BeginForm("Index", "NhapKho", FormMethod.Post, new { enctype = "multipart/form-data" }))
    {
        @Html.AntiForgeryToken()
        @Html.ValidationSummary(true)
        <section class="container" id="nhap-kho">
            <h1 class="title">NHẬP KHO</h1>
            @if (ViewBag.ErrorMessage != null)
            {
                <p id="tb-sm">@ViewBag.ErrorMessage</p>
            }
            <div class="form-nhap-kho">

                <div class="thong-tin-chung">

                    <h4 class="sub-title">
                        Thông tin chung
                    </h4>
                    <div class="form-group">
                        <label for="">Nhóm thiết bị: </label>
                        @*@Html.DropDownList("MA_NHOMTB", null, String.Empty, new { @class = "form-control", @placeholder = "", @onchange = "Fill_LoaiTB()" })*@
                        <select class="form-control" id="MA_NHOMTB" name="MA_NHOMTB" onchange="Fill_LoaiTB()"></select>
                    </div>
                    <div class="form-group">
                        <label for="">Loại thiết bị: </label>
                        <select class="form-control" id="MA_LOAITB" name="MA_LOAITB"></select>
                    </div>

                    <div class="form-group">
                        <label for="">Tên thiết bị:</label>
                        <input type="text" name="TENTB" />
                    </div>
                    <div class="form-group">
                        <label for="">Số serial:</label>
                        <input type="text" name="SO_SERIAL" />
                    </div>
                    <div class="form-group">
                        <label for="">Giá tiền:</label>
                        <input type="number" name="GIA_TIEN" />
                    </div>
                    <div class="form-group">
                        <label for="">THBH:</label>
                        <input type="text" name="THOI_HAN_BAO_HANH" />
                    </div>
                    <div class="form-group">
                        <label for="">Ngày mua:</label>
                        <input type="date" name="NGAY_MUA" />
                    </div>
                    <div class="form-group">
                        <label for="">Đơn vị mua:</label>
                        <select class="form-control" id="MA_DON_VI" name="MA_DON_VI"></select>
                    </div>
                    <div class="form-group">
                        <label for="">Nhà cung cấp:</label>
                        <select class="form-control" id="MA_NCC" name="MA_NCC"></select>
                    </div>
                </div>
                <div class="thongso-hinhanh">
                    <div class="them-hinh-anh">
                        <h4 class="sub-title">
                            Thêm hình ảnh
                        </h4>
                        <div class="img-container">
                            <img id="img" src="#" alt="your image" />
                            <span><i class="far fa-times-circle"></i></span>
                        </div>
                        <div class="row" id="image_preview"></div>
                        <div class="inputfile-div">
                            <input type="file" class="inputfile inputfile-1"
                                   data-multiple-caption="{count} files selected" id="file-1" name="HINH_ANH" onchange="preview_images();" multiple />
                            <label for="file-1">
                                <svg xmlns="http://www.w3.org/2000/svg"
                                     width="20"
                                     height="17"
                                     viewBox="0 0 20 17">
                                    <path d="M10 0l-5.2 4.9h3.3v5.1h3.8v-5.1h3.3l-5.2-4.9zm9.3 11.5l-3.2-2.1h-2l3.4 2.6h-3.5c-.1 0-.2.1-.2.1l-.8 2.3h-6l-.8-2.2c-.1-.1-.1-.2-.2-.2h-3.6l3.4-2.6h-2l-3.2 2.1c-.4.3-.7 1-.6 1.5l.6 3.1c.1.5.7.9 1.2.9h16.3c.6 0 1.1-.4 1.3-.9l.6-3.1c.1-.5-.2-1.2-.7-1.5z" />
                                </svg>
                                <span>Choose a file&hellip;</span>
                            </label>
                        </div>
                        @*<div class="inputfile-div">
                                <input type="file"
                                       name="file-1[]"
                                       id="file-1"
                                       class="inputfile inputfile-1"
                                       data-multiple-caption="{count} files selected"
                                       multiple />
                                <label for="file-1">
                                    <svg xmlns="http://www.w3.org/2000/svg"
                                         width="20"
                                         height="17"
                                         viewBox="0 0 20 17">
                                        <path d="M10 0l-5.2 4.9h3.3v5.1h3.8v-5.1h3.3l-5.2-4.9zm9.3 11.5l-3.2-2.1h-2l3.4 2.6h-3.5c-.1 0-.2.1-.2.1l-.8 2.3h-6l-.8-2.2c-.1-.1-.1-.2-.2-.2h-3.6l3.4-2.6h-2l-3.2 2.1c-.4.3-.7 1-.6 1.5l.6 3.1c.1.5.7.9 1.2.9h16.3c.6 0 1.1-.4 1.3-.9l.6-3.1c.1-.5-.2-1.2-.7-1.5z" />
                                    </svg>
                                    <span>Choose a file&hellip;</span>
                                </label>
                            </div>*@
                    </div>
                    <div class="thong-so-ky-thuat" id="thong-so-ky-thuat" hidden="hidden">
                        <h4 class="sub-title">
                            Thông số kỹ thuật
                        </h4>
                        <div id="first" class="form-group">
                            <label id="label1"></label>
                            <select class="form-control"
                                    id="thong_so1"
                                    name="thong_so1"></select>
                            <input type="text" id="input_thong_so1" name="input_thong_so1" />
                        </div>
                        <div id="second" class="form-group">
                            <label id="label2"></label>
                            <select class="form-control"
                                    id="thong_so2"
                                    name="thong_so2"></select>
                        </div>
                        <div id="third" class="form-group">
                            <label id="label3"></label>
                            <select class="form-control"
                                    id="thong_so3"
                                    name="thong_so3"></select>
                            <input type="number" id="input_thong_so3" name="input_thong_so3" />
                        </div>
                        <div id="fourth" class="form-group">
                            <label id="label4"></label>
                            <select class="form-control"
                                    id="thong_so4"
                                    name="thong_so4"></select>
                            <input type="text" id="input_thong_so4" name="input_thong_so4" />
                        </div>
                        <div id="fifth">
                            <div class="form-group">
                                <label id="label5"></label>
                                <select class="form-control"
                                        id="thong_so5"
                                        name="thong_so5"></select>
                            </div>
                        </div>

                        <div id="sixth">
                            <div class="form-group">
                                <label id="label6"></label>
                                <select class="form-control"
                                        id="thong_so6"
                                        name="thong_so6"></select>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="submit">
                @if (ViewBag.Them != null)
                {
                    <button type="submit" id="SAVE" name="SAVE" value="save" class="luu">THÊM</button>
                    <button class="reset" name="REFESH" value="refesh">Làm mới</button>
                }
            </div>
        </section>
    }
</main>

<script type="text/javascript" src="~/Scripts/main.js"></script>

<script type="text/javascript">
    $(document).ready(function () {
        $.ajax({
            type: "GET",
            url: "/Script/get_NHOMTB",
            data: "{}",
            success: function (data) {
                var s = '<option disabled selected value>Xin chọn nhóm thiết bị</option>';
                for (var i = 0; i < data.length; i++) {
                    s += '<option value="' + data[i] + '">' + data[i] + '</option>';
                }
                $("#MA_NHOMTB").html(s);
            }
        });

        $.ajax({
            type: "GET",
            url: "/Script/get_DONVI",
            data: "{}",
            success: function (data) {
                var s = '<option disabled selected value>Xin chọn đơn vị</option>';
                for (var i = 0; i < data.length; i++) {
                    s += '<option value="' + data[i] + '">' + data[i] + '</option>';
                }
                $("#MA_DON_VI").html(s);
            }
        });

        $.ajax({
            type: "GET",
            url: "/Script/get_NCC",
            data: "{}",
            success: function (data) {
                var s = '<option disabled selected value>Xin chọn nhà cung cấp</option>';
                for (var i = 0; i < data.length; i++) {
                    s += '<option value="' + data[i] + '">' + data[i] + '</option>';
                }
                $("#MA_NCC").html(s);
            }
        });
    });

    function preview_images() {
        var total_file = document.getElementById("file-1").files.length;
        if (total_file > 5) {
            alert('Hello, just 5!');
        }
        else {
            for (var i = 0; i < total_file; i++) {
                $('#image_preview').append("<div style='display: inline-block; margin: 10px;'><img class='img-responsive' width='50' height='50' src='" + URL.createObjectURL(event.target.files[i]) + "' onclick='cHANGE_IMG(this.src)'></div>");
            }
            var reader = new FileReader();

            reader.onload = function (e) {
                $('#img')
                    .attr('src', e.target.result)
                    .width(313)
                    .height(178);
            };
            reader.readAsDataURL(event.target.files[total_file - 1]);
        }
    }

    function Fill_LoaiTB() {
        var nhom_TB = $('#MA_NHOMTB').val();
        $.ajax({
            type: "GET",
            url: "/Script/get_LOAITB",
            data: { nhom_TB: nhom_TB },
            success: function (data) {
                document.getElementById("thong-so-ky-thuat").hidden = false;
                $("#MA_LOAITB").html("");
                var options = '<option disabled selected value>Xin chọn loại thiết bị</option>';
                for (var i = 0; i < data.length; i++) {
                    options += '<option value="' + data[i] + '">' + data[i] + '</option>';
                }
                $("#MA_LOAITB").html(options);
            }
        });

        $.ajax({
            type: "GET",
            url: "/Script/get_THONG_SO_KY_THUAT",
            data: { nhom_TB: nhom_TB },
            success: function (data) {
                if (data.NHOMTB == "PC") {
                    document.getElementById("fifth").hidden = false;
                    document.getElementById("sixth").hidden = false;
                    //document.getElementById("fifth").setAttribute("hidden", "false");
                    //document.getElementById("sixth").setAttribute("hidden", "false");

                    document.getElementById("thong_so1").hidden = false;
                    document.getElementById("input_thong_so1").hidden = true;
                    document.getElementById("thong_so3").hidden = false;
                    document.getElementById("input_thong_so3").hidden = true;
                    document.getElementById("thong_so4").hidden = false;
                    document.getElementById("input_thong_so4").hidden = true;

                    document.getElementById("label1").innerHTML = "CPU:";
                    document.getElementById("label2").innerHTML = "RAM";
                    document.getElementById("label3").innerHTML = "Màn hình:";
                    document.getElementById("label4").innerHTML = "Ổ cứng:";
                    document.getElementById("label5").innerHTML = "VGA:";
                    document.getElementById("label6").innerHTML = "Hệ điều hành:";



                    $("#thong_so1").html("");
                    $("#thong_so2").html("");
                    $("#thong_so3").html("");
                    $("#thong_so4").html("");
                    $("#thong_so5").html("");
                    $("#thong_so6").html("");

                    var options1 = '<option disabled selected value>Xin chọn các thông số kỹ thuật</option>';
                    var options2 = '<option disabled selected value></option>';
                    var options3 = '<option disabled selected value></option>';
                    var options4 = '<option disabled selected value></option>';
                    var options5 = '<option disabled selected value></option>';
                    var options6 = '<option disabled selected value></option>';

                    for (var i = 0; i < data.CPU.length; i++) {
                        options1 += '<option value="' + data.CPU[i] + '">' + data.CPU[i] + '</option>';
                    }

                    for (var i = 0; i < data.RAM.length; i++) {
                        options2 += '<option value="' + data.RAM[i] + '">' + data.RAM[i] + '</option>';
                    }

                    for (var i = 0; i < data.MAN_HINH.length; i++) {
                        options3 += '<option value="' + data.MAN_HINH[i] + '">' + data.MAN_HINH[i] + '</option>';
                    }

                    for (var i = 0; i < data.O_CUNG.length; i++) {
                        options4 += '<option value="' + data.O_CUNG[i] + '">' + data.O_CUNG[i] + '</option>';
                    }

                    for (var i = 0; i < data.VGA.length; i++) {
                        options5 += '<option value="' + data.VGA[i] + '">' + data.VGA[i] + '</option>';
                    }

                    for (var i = 0; i < data.HDH.length; i++) {
                        options6 += '<option value="' + data.HDH[i] + '">' + data.HDH[i] + '</option>';
                    }

                    $("#thong_so1").html(options1);
                    $("#thong_so2").html(options2);
                    $("#thong_so3").html(options3);
                    $("#thong_so4").html(options4);
                    $("#thong_so5").html(options5);
                    $("#thong_so6").html(options6);


                }
                else if (data.NHOMTB == "PR") {
                    document.getElementById("fifth").hidden = true;
                    document.getElementById("sixth").hidden = true;
                    //document.getElementById("fifth").setAttribute("hidden", "true");
                    //document.getElementById("sixth").setAttribute("hidden", "true");
                    document.getElementById("thong_so1").hidden = true;
                    document.getElementById("input_thong_so1").hidden = false;
                    document.getElementById("thong_so3").hidden = true;
                    document.getElementById("input_thong_so3").hidden = false;
                    document.getElementById("thong_so4").hidden = true;
                    document.getElementById("input_thong_so4").hidden = false;

                    document.getElementById("label1").innerHTML = "Kích thước:";
                    document.getElementById("label2").innerHTML = "Loại mực:";
                    document.getElementById("label3").innerHTML = "Tốc độ:";
                    document.getElementById("label4").innerHTML = "Độ phân giải:";

                    $("#thong_so2").html("");

                    var options2 = '<option disabled selected value>Xin chọn các thông số kỹ thuật</option>';
                    //var options3 = '<option value="null"></option>';

                    for (var i = 0; i < data.LOAI_MUC.length; i++) {
                        options2 += '<option value="' + data.LOAI_MUC[i] + '">' + data.LOAI_MUC[i] + '</option>';
                    }

                    $("#thong_so2").html(options2);
                    //$("#thong_so3").html(options3);
                }
            }
        });
    };

    function cHANGE_IMG(src) {
        document.getElementById("img").src = src;
    }
</script>
