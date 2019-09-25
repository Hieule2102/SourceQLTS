using Source.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Source.Controllers
{
    public class ScriptController : Controller
    {
        private QuanLyTaiSanCNTTEntities db = new QuanLyTaiSanCNTTEntities();

        // GET: Script
        public ActionResult Index()
        {
            return View();
        }
        #region get_ThongTin
        [HttpGet]
        public ActionResult get_ThongTinTB(string maTB)
        {
            //Tìm tên thiết bị
            if (!String.IsNullOrEmpty(maTB))
            {
                int temp = Int32.Parse(maTB);
                var tenTB = (from d in db.THIETBIs
                             where d.MATB == temp
                             select d.TENTB).FirstOrDefault();

                var tenDonVi = (from d in db.THIETBIs
                             where d.MATB == temp
                             select d.DON_VI.TEN_DON_VI).FirstOrDefault();

                var thet_Bi = db.THIETBIs.Select(x => new
                                                {
                                                    tenTB,
                                                    tenDonVi
                                                }).FirstOrDefault();

                return Json(thet_Bi, JsonRequestBehavior.AllowGet);
            }

            return null;
        }

        [HttpGet]
        public ActionResult get_ThongTinND(string ma_ND)
        {
            if (!String.IsNullOrEmpty(ma_ND))
            {
                var nhom_ND = (from a in db.NHOM_ND
                               where a.MA_ND == ma_ND
                               select (from b in db.NHOM_NGUOI_DUNG
                                       where b.MA_NHOM == a.MA_NHOM
                                       select b.TEN_NHOM)
                                       .FirstOrDefault())
                               .FirstOrDefault();

                var nguoi_Dung = db.NGUOI_DUNG.Where(a => a.MA_ND == ma_ND)
                                              .Select(x => new
                                                {
                                                    x.TEN_ND,
                                                    x.DIEN_THOAI,
                                                    x.DON_VI.TEN_DON_VI,
                                                    x.EMAIL,
                                                    nhom_ND
                                                }).FirstOrDefault();
                return Json(nguoi_Dung, JsonRequestBehavior.AllowGet);
            }

            return null;
        }

        [HttpGet]
        public ActionResult get_ThongTinNhomND(string ma_NHOM)
        {
            if (!String.IsNullOrEmpty(ma_NHOM))
            {
                var nHOM_NGUOI_DUNG = db.NHOM_NGUOI_DUNG.Where(a => a.MA_NHOM == ma_NHOM)
                                                        .Select(x => new
                                                        {
                                                            x.TEN_NHOM,
                                                            x.GHI_CHU
                                                        }).FirstOrDefault();
                return Json(nHOM_NGUOI_DUNG, JsonRequestBehavior.AllowGet);
            }

            return null;
        }

        [HttpGet]
        public ActionResult get_ThongTinNhomTB(string ma_NHOMTB)
        {
            if (!String.IsNullOrEmpty(ma_NHOMTB))
            {
                var nHOM_TB = db.NHOM_THIETBI.Where(a => a.MA_NHOMTB == ma_NHOMTB)
                                                        .Select(x => new
                                                        {
                                                            x.TEN_NHOM,
                                                            x.GHI_CHU
                                                        }).FirstOrDefault();
                return Json(nHOM_TB, JsonRequestBehavior.AllowGet);
            }

            return null;
        }

        [HttpGet]
        public ActionResult get_ThongTinLoaiTB(string ma_LOAITB)
        {
            if (!String.IsNullOrEmpty(ma_LOAITB))
            {
                var lOAI_TB = db.LOAI_THIETBI.Where(a => a.MA_LOAITB == ma_LOAITB)
                                                        .Select(x => new
                                                        {
                                                            x.TEN_LOAI,
                                                            x.NHOM_THIETBI.TEN_NHOM,
                                                            x.GHI_CHU
                                                        }).FirstOrDefault();
                return Json(lOAI_TB, JsonRequestBehavior.AllowGet);
            }

            return null;
        }

        [HttpGet]
        public ActionResult get_ThongTinDonVi(string ma_DON_VI)
        {
            if (!String.IsNullOrEmpty(ma_DON_VI))
            {
                var temp = Int32.Parse(ma_DON_VI);
                var dON_VI = db.DON_VI.Where(a => a.MA_DON_VI == temp)
                                                        .Select(x => new
                                                        {
                                                            x.TEN_DON_VI,
                                                            DON_VI_CAP_TREN = x.DON_VI2.TEN_DON_VI,
                                                            x.DIA_CHI,
                                                            x.DIEN_THOAI,
                                                            x.FAX
                                                        }).FirstOrDefault();
                return Json(dON_VI, JsonRequestBehavior.AllowGet);
            }

            return null;
        }

        [HttpGet]
        public ActionResult get_ThongTinNCC(string ma_NCC)
        {
            if (!String.IsNullOrEmpty(ma_NCC))
            {
                var temp = Int32.Parse(ma_NCC);
                var nCC = db.NHA_CUNG_CAP.Where(a => a.MA_NCC == temp)
                                                        .Select(x => new
                                                        {
                                                            x.MA_NCC,
                                                            x.TEN_NCC,
                                                            x.DIA_CHI,
                                                            x.DIEN_THOAI,
                                                            x.FAX,
                                                            x.GHI_CHU
                                                        }).FirstOrDefault();
                return Json(nCC, JsonRequestBehavior.AllowGet);
            }

            return null;
        }

        [HttpGet]
        public ActionResult get_THONG_SO_KY_THUAT(string nhom_TB)
        {
            if (!String.IsNullOrEmpty(nhom_TB))
            {
                var nhomTB = (from d in db.NHOM_THIETBI
                              where d.TEN_NHOM == nhom_TB
                              select d.MA_NHOMTB).FirstOrDefault();
                if (nhomTB == "PC")
                {
                    var CPU = db.DM_CPU.Select(x => x.TEN_CPU).ToList();
                    var MAN_HINH = db.DM_MAN_HINH.Select(x => x.TEN_MAN_HINH).ToList();
                    var RAM = db.DM_RAM.Select(x => x.TEN_RAM).ToList();
                    var O_CUNG = db.DM_O_CUNG.Select(x => x.TEN_O_CUNG).ToList();
                    var VGA = db.DM_VGA.Select(x => x.TEN_VGA).ToList();
                    var HDH = db.DM_HDH.Select(x => x.TEN_HDH).ToList();

                    var cauHinh = db.CAU_HINH.Select(x => new
                    {
                        NHOMTB = nhomTB,
                        CPU,
                        MAN_HINH,
                        RAM,
                        O_CUNG,
                        VGA,
                        HDH
                    }).FirstOrDefault();
                    return Json(cauHinh, JsonRequestBehavior.AllowGet);
                }
                else if (nhomTB == "PR")
                {
                    var cauHinh = db.CAU_HINH.Select(x => new
                    {
                        NHOMTB = nhomTB,
                        KICH_THUOC = "a",
                        LOAI_MUC = "a",
                        DO_PHAN_GIAI = "a",
                    }).FirstOrDefault();
                    return Json(cauHinh, JsonRequestBehavior.AllowGet);
                }
            }

            return null;
        }

        [HttpPost]
        public ActionResult get_CauHinh(string maTB)
        {
            var temp = Int32.Parse(maTB);
            if (!String.IsNullOrEmpty(maTB))
            {
                var cauHinh = db.CAU_HINH.Where(x => x.MATB == temp)
                                         .Select(x => new
                                         {
                                             x.THIETBI.LOAI_THIETBI.MA_NHOMTB,
                                             x.THIETBI.TENTB,
                                             CPU = x.DM_CPU.TEN_CPU,
                                             MAN_HINH = x.DM_MAN_HINH.TEN_MAN_HINH,
                                             RAM = x.DM_RAM.TEN_RAM,
                                             O_CUNG = x.DM_O_CUNG.TEN_O_CUNG,
                                             VGA = x.DM_VGA.TEN_VGA,
                                             HDH = x.DM_HDH.TEN_HDH,
                                             x.KICH_THUOC,
                                             x.LOAI_MUC,
                                             x.TOC_DO,
                                             x.DO_PHAN_GIAI
                                         })
                                         .FirstOrDefault();
                return Json(cauHinh, JsonRequestBehavior.AllowGet);
            }
            return null;
        }
        [HttpGet]
        public ActionResult get_HinhAnh(string maTB)
        {
            if (!String.IsNullOrEmpty(maTB))
            {
                var temp = Int32.Parse(maTB);
                var hinhAnh = db.HINH_ANH.Where(x => x.MATB == temp)
                                         .Select(x => x.HINH1)
                                         .FirstOrDefault();
                if (String.IsNullOrEmpty(hinhAnh))
                {
                    return null;
                }
                return Json(hinhAnh, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        [HttpGet]
        public ActionResult get_ThongTinPhanQuyen(string ma_NHOM)
        {
            //Tìm kiếm
            if (!String.IsNullOrEmpty(ma_NHOM))
            {
                var temp = (from d in db.NHOM_NGUOI_DUNG
                            where d.TEN_NHOM == ma_NHOM
                            select d.MA_NHOM).FirstOrDefault();
                var thietbis = db.NHOM_ND_CHUCNANG.Where(d => d.MA_NHOM == temp)
                                                    .Select(d => new
                                                    {
                                                        d.MA_QUYEN,
                                                        d.MA_CHUC_NANG,
                                                        MA_DM = d.DM_CHUC_NANG.CHUC_NANG.MA_CHUC_NANG
                                                    })
                                                    .ToList();

                return Json(thietbis, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        [HttpGet]
        public ActionResult get_ThongTinDieuChuyen(string ma_DIEU_CHUYEN)
        {
            //Tìm tên thiết bị
            if (!String.IsNullOrEmpty(ma_DIEU_CHUYEN))
            {
                int temp = Int32.Parse(ma_DIEU_CHUYEN);
                var NGAY_CHUYEN = String.Format("MM/dd/yyyy");
                var dIEUCHUYEN = db.DIEU_CHUYEN_THIET_BI.Where(x => x.MA_DIEU_CHUYEN == temp)
                                                        .Select(x => new
                                                                {
                                                                    x.MA_DIEU_CHUYEN,
                                                                    x.MATB,
                                                                    x.THIETBI.TENTB,
                                                                    NGAY_CHUYEN = x.NGAY_CHUYEN.ToString(),
                                                                    DV_QL = x.DON_VI.TEN_DON_VI,
                                                                    DV_NHAN = x.DON_VI1.TEN_DON_VI,
                                                                    x.MAND_THUC_HIEN,
                                                                    x.GHI_CHU
                                                                }).FirstOrDefault();

                return Json(dIEUCHUYEN, JsonRequestBehavior.AllowGet);
            }

            return null;
        }
        #endregion

        #region get_Ten
        //Tìm loại thiết bị
        [HttpGet]
        public ActionResult get_LOAITB(string nhom_TB)
        {
            var dsLoaiTB = new List<string>();
            if (!String.IsNullOrEmpty(nhom_TB))
            {
                var qLoaiTB = (from d in db.LOAI_THIETBI
                               where d.NHOM_THIETBI.TEN_NHOM == nhom_TB
                               orderby d.TEN_LOAI
                               select d.TEN_LOAI);
                dsLoaiTB.AddRange(qLoaiTB.ToList());
            }

            return Json(dsLoaiTB, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult get_MATB()
        {
            var dsMaTB = new List<int>();
            var qMaTB = (from d in db.THIETBIs
                         orderby d.MATB
                         select d.MATB);
            dsMaTB.AddRange(qMaTB.ToList());
            return Json(dsMaTB, JsonRequestBehavior.AllowGet);
        }

        //Tìm tên thiết bị
        [HttpGet]
        public ActionResult get_TENTB(string maTB)
        {
            int temp = Int32.Parse(maTB);
            //Tìm tên thiết bị
            if (!String.IsNullOrEmpty(maTB))
            {
                var tenTB = (from d in db.THIETBIs
                             where d.MATB == temp
                             select d.TENTB).First();
                return Json(tenTB, JsonRequestBehavior.AllowGet);
            }

            return null;
        }

        //get nhóm thiết bị
        [HttpGet]
        public ActionResult get_NHOMTB()
        {
            var dsNhomTB = new List<string>();
            var qNhomTB = (from d in db.NHOM_THIETBI
                           orderby d.TEN_NHOM
                           select d.TEN_NHOM);
            dsNhomTB.AddRange(qNhomTB.ToList());

            return Json(dsNhomTB, JsonRequestBehavior.AllowGet);
        }

        public ActionResult get_NCC()
        {
            var dsNCC = new List<string>();
            var qNCC = (from d in db.NHA_CUNG_CAP
                        orderby d.TEN_NCC
                        select d.TEN_NCC);
            dsNCC.AddRange(qNCC.ToList());

            return Json(dsNCC, JsonRequestBehavior.AllowGet);
        }

        public ActionResult get_DONVI()
        {
            var dsDonVi = new List<string>();
            var qDonVi = (from d in db.DON_VI
                          orderby d.TEN_DON_VI
                          select d.TEN_DON_VI);
            dsDonVi.AddRange(qDonVi.ToList());

            return Json(dsDonVi, JsonRequestBehavior.AllowGet);
        }

        public ActionResult get_TRANGTHAI()
        {
            var dsTrangThai = new List<string>();
            var qTrangThai = (from d in db.THIETBIs
                              orderby d.TINH_TRANG
                              select d.TINH_TRANG);
            dsTrangThai.AddRange(qTrangThai.Distinct());

            return Json(dsTrangThai, JsonRequestBehavior.AllowGet);
        }

        public ActionResult get_NHOM_ND()
        {
            var dsTenNhom = new List<string>();
            var qTenNhom = (from d in db.NHOM_NGUOI_DUNG
                            orderby d.TEN_NHOM
                            select d.TEN_NHOM);
            dsTenNhom.AddRange(qTenNhom.Distinct());

            return Json(dsTenNhom, JsonRequestBehavior.AllowGet);
        }

        public ActionResult get_CPU()
        {
            var dsCPU = new List<string>();
            var qCPU = (from d in db.DM_CPU
                        orderby d.TEN_CPU
                        select d.TEN_CPU);
            dsCPU.AddRange(qCPU.Distinct());

            return Json(dsCPU, JsonRequestBehavior.AllowGet);
        }

        public ActionResult get_RAM()
        {
            var dsRAM = new List<string>();
            var qRAM = (from d in db.DM_RAM
                        orderby d.TEN_RAM
                        select d.TEN_RAM);
            dsRAM.AddRange(qRAM.Distinct());

            return Json(dsRAM, JsonRequestBehavior.AllowGet);
        }

        public ActionResult get_O_CUNG()
        {
            var dsOCung = new List<string>();
            var qOCung = (from d in db.DM_O_CUNG
                          orderby d.TEN_O_CUNG
                          select d.TEN_O_CUNG);
            dsOCung.AddRange(qOCung.Distinct());
            ViewBag.O_CUNG = new SelectList(dsOCung);

            return Json(dsOCung, JsonRequestBehavior.AllowGet);
        }

        public ActionResult get_MAN_HINH()
        {
            var dsManHInh = new List<string>();
            var qManHInh = (from d in db.DM_MAN_HINH
                            orderby d.TEN_MAN_HINH
                            select d.TEN_MAN_HINH);
            dsManHInh.AddRange(qManHInh.Distinct());

            return Json(dsManHInh, JsonRequestBehavior.AllowGet);
        }

        public ActionResult get_VGA()
        {
            var dsVGA = new List<string>();
            var qVGA = (from d in db.DM_VGA
                        orderby d.TEN_VGA
                        select d.TEN_VGA);
            dsVGA.AddRange(qVGA.Distinct());

            return Json(dsVGA, JsonRequestBehavior.AllowGet);
        }

        public ActionResult get_HDH()
        {
            var dsHDH = new List<string>();
            var qHDH = (from d in db.DM_HDH
                        orderby d.TEN_HDH
                        select d.TEN_HDH);
            dsHDH.AddRange(qHDH.Distinct());

            return Json(dsHDH, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}