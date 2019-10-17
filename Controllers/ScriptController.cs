using Source.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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
        public ActionResult get_ThongTinTB(string mATB)
        {
            //Tìm tên thiết bị
            if (!String.IsNullOrEmpty(mATB))
            {
                var thet_Bi = db.THIETBIs.Where(a => a.MATB == mATB)
                                         .Select(x => new
                                         {
                                             x.MATB,
                                             x.TENTB,
                                             x.DON_VI.TEN_DON_VI,
                                             x.NGUOI_DUNG.TEN_ND,
                                             x.LOAI_THIETBI.TEN_LOAI,
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
        public ActionResult get_THONG_SO_KY_THUAT(string mA_NHOMTB)
        {
            if (!String.IsNullOrEmpty(mA_NHOMTB))
            {
                var nhomTB = (from d in db.NHOM_THIETBI
                              where d.TEN_NHOM == mA_NHOMTB
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
                    var LOAI_MUC = db.DM_LOAI_MUC.Select(x => x.TEN_LOAI_MUC).ToList();

                    var cauHinh = db.CAU_HINH.Select(x => new
                    {
                        NHOMTB = nhomTB,
                        LOAI_MUC,
                    }).FirstOrDefault();
                    return Json(cauHinh, JsonRequestBehavior.AllowGet);
                }
            }

            return null;
        }

        [HttpPost]
        public ActionResult get_CAU_HINH(string mA_LOAITB)
        {
            if (!String.IsNullOrEmpty(mA_LOAITB))
            {
                //mA_LOAITB = (from a in db.LOAI_THIETBI
                //             where a.TEN_LOAI == mA_LOAITB
                //             select a.MA_LOAITB).FirstOrDefault();
                var cAU_HINH = db.CAU_HINH.Where(x => x.LOAI_THIETBI.TEN_LOAI == mA_LOAITB)
                                         .Select(x => new
                                         {
                                             x.LOAI_THIETBI.TEN_LOAI,
                                             x.LOAI_THIETBI.MA_NHOMTB,
                                             CPU = x.DM_CPU.TEN_CPU,
                                             MAN_HINH = x.DM_MAN_HINH.TEN_MAN_HINH,
                                             RAM = x.DM_RAM.TEN_RAM,
                                             O_CUNG = x.DM_O_CUNG.TEN_O_CUNG,
                                             VGA = x.DM_VGA.TEN_VGA,
                                             HDH = x.DM_HDH.TEN_HDH,
                                             x.KICH_THUOC,
                                             LOAI_MUC = x.DM_LOAI_MUC.TEN_LOAI_MUC,
                                             x.TOC_DO,
                                             x.DO_PHAN_GIAI
                                         })
                                         .FirstOrDefault();
                if(cAU_HINH == null)
                {
                    return null;
                }
                return Json(cAU_HINH, JsonRequestBehavior.AllowGet);
            }
            return null;
        }
        [HttpGet]
        public ActionResult get_HINH_ANH(string mATB)
        {
            if (!String.IsNullOrEmpty(mATB))
            {
                var hinhAnh = db.HINH_ANH.Where(x => x.MATB == mATB)
                                         .Select(x => new
                                         {
                                             x.HINH1,
                                             x.HINH2,
                                             x.HINH3,
                                             x.HINH4,
                                             x.HINH5,
                                         })
                                         .FirstOrDefault();
                if (hinhAnh == null)
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
        public ActionResult get_ThongTinDieuChuyen(string mATB)
        {
            //Tìm tên thiết bị
            if (!String.IsNullOrEmpty(mATB))
            {
                if (db.XUAT_KHO.FirstOrDefault(a => a.MATB == mATB) != null)
                {
                    var xAC_NHAN = db.XUAT_KHO.Where(x => x.MATB == mATB)
                                              .Select(x => new
                                              {
                                                  x.MA_XUAT_KHO,
                                                  x.MATB,
                                                  x.THIETBI.TENTB,
                                                  NGAY_THUC_HIEN = x.NGAY_XUAT.ToString(),
                                                  DV_THUC_HIEN = x.DON_VI.TEN_DON_VI,
                                                  DV_NHAN = x.DON_VI1.TEN_DON_VI,
                                                  MAND_THUC_HIEN = x.MAND_XUAT,
                                                  x.SO_LUONG,
                                                  x.GHI_CHU,
                                                  x.VAN_CHUYEN
                                              }).FirstOrDefault();
                    return Json(xAC_NHAN, JsonRequestBehavior.AllowGet);
                }
                else if (db.DIEU_CHUYEN_THIET_BI.FirstOrDefault(a => a.MATB == mATB) != null)
                {
                    var xAC_NHAN = db.DIEU_CHUYEN_THIET_BI.Where(x => x.MATB == mATB)
                                                        .Select(x => new
                                                        {
                                                            x.MA_DIEU_CHUYEN,
                                                            x.MATB,
                                                            x.THIETBI.TENTB,
                                                            NGAY_THUC_HIEN = x.NGAY_DIEU_CHUYEN.ToString(),
                                                            DV_THUC_HIEN = x.DON_VI.TEN_DON_VI,
                                                            DV_NHAN = x.DON_VI1.TEN_DON_VI,
                                                            MAND_THUC_HIEN = x.MAND_DIEU_CHUYEN,
                                                            x.SO_LUONG,
                                                            x.GHI_CHU,
                                                            x.VAN_CHUYEN
                                                        }).FirstOrDefault();
                    return Json(xAC_NHAN, JsonRequestBehavior.AllowGet);
                }
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
            var dsMaTB = new List<string>();
            var qMaTB = (from d in db.THIETBIs
                         orderby d.MATB
                         select d.MATB).ToList();
            dsMaTB.AddRange(qMaTB);
            return Json(dsMaTB, JsonRequestBehavior.AllowGet);
        }

        //Tìm tên thiết bị
        [HttpGet]
        public ActionResult get_TENTB(string maTB)
        {
            //Tìm tên thiết bị
            if (!String.IsNullOrEmpty(maTB))
            {
                var tenTB = (from d in db.THIETBIs
                             where d.MATB == maTB
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
                          where d.MA_DON_VI != 7
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

        [HttpGet]
        public ActionResult get_ND(string mA_DV)
        {
            var dsND = new List<string>();
            var qND = (from d in db.NGUOI_DUNG
                       where d.DON_VI.TEN_DON_VI == mA_DV
                       orderby d.TEN_ND
                       select d.TEN_ND);
            dsND.AddRange(qND.Distinct());

            return Json(dsND, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult get_DV(string tEN_DANG_NHAP)
        {
            if(!String.IsNullOrEmpty(tEN_DANG_NHAP))
            {
                var dONVI = db.NGUOI_DUNG.Where(a => a.TEN_DANG_NHAP == tEN_DANG_NHAP)
                                         .Select(a => a.DON_VI.TEN_DON_VI)
                                         .FirstOrDefault();

                return Json(dONVI, JsonRequestBehavior.AllowGet);
            }
            return null;
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

        [HttpGet]
        public ActionResult get_MATB_XUAT_KHO()
        {
            var maTB_XUAT_KHO = db.XUAT_KHO.Select(a => a.MATB);
            var maTB_DIEU_CHUYEN = db.DIEU_CHUYEN_THIET_BI.Select(a => a.MATB);

            var xUATKHO = db.NHAP_KHO.Where(x => !maTB_XUAT_KHO.Contains(x.MATB) && !maTB_DIEU_CHUYEN.Contains(x.MATB))
                                     .Select(x => new
                                     {
                                         x.MATB,
                                         x.THIETBI.TENTB
                                     }).ToList();
            return Json(xUATKHO, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult get_MATB_DIEU_CHUYEN()
        {
            var maTB_DIEU_CHUYEN = db.DIEU_CHUYEN_THIET_BI.Select(a => a.MATB);
            var maTB_XUAT_KHO = db.XUAT_KHO.Select(a => a.MATB);

            var dIEUCHUYEN = db.NHAP_KHO.Where(x => !maTB_XUAT_KHO.Contains(x.MATB) && !maTB_DIEU_CHUYEN.Contains(x.MATB))
                                        .Select(x => new
                                        {
                                            x.MATB,
                                            x.THIETBI.TENTB
                                        }).ToList();
            return Json(dIEUCHUYEN, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}