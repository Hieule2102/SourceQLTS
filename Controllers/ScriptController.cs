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
        public ActionResult get_THONG_TIN_TB(string mATB)
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
                                             x.MA_LOAITB,
                                             x.LOAI_THIETBI.TEN_LOAI,
                                             x.LOAI_THIETBI.MA_NHOMTB,
                                             x.SO_LUONG
                                         }).FirstOrDefault();

                return Json(thet_Bi, JsonRequestBehavior.AllowGet);
            }

            return null;
        }

        [HttpGet]
        public ActionResult get_THONG_TIN_ND(string mA_ND)
        {
            if (!String.IsNullOrEmpty(mA_ND))
            {
                var temp = db.NHOM_ND.FirstOrDefault(a => a.MA_ND == mA_ND);
                var MA_NHOM = temp.MA_NHOM;
                var MA_NHOM_ND = temp.MA_NHOM_ND;

                var nGUOI_DUNG = db.NGUOI_DUNG.Where(a => a.MA_ND == mA_ND)
                                              .Select(x => new
                                              {
                                                  x.MA_ND,
                                                  x.TEN_ND,
                                                  x.DIEN_THOAI,
                                                  x.MA_DON_VI,
                                                  x.EMAIL,
                                                  MA_NHOM,
                                                  MA_NHOM_ND
                                              }).FirstOrDefault();
                return Json(nGUOI_DUNG, JsonRequestBehavior.AllowGet);
            }

            return null;
        }

        [HttpGet]
        public ActionResult get_THONG_TIN_NHOM_ND(string mA_NHOM)
        {
            if (!String.IsNullOrEmpty(mA_NHOM))
            {
                var nHOM_NGUOI_DUNG = db.NHOM_NGUOI_DUNG.Where(a => a.MA_NHOM == mA_NHOM)
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
        public ActionResult get_THONG_TIN_NHOMTB(string mA_NHOMTB)
        {
            if (!String.IsNullOrEmpty(mA_NHOMTB))
            {
                var nHOM_TB = db.NHOM_THIETBI.Where(a => a.MA_NHOMTB == mA_NHOMTB)
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
        public ActionResult get_THONG_TIN_LOAITB(string mA_LOAITB)
        {
            if (!String.IsNullOrEmpty(mA_LOAITB))
            {
                var lOAI_TB = db.LOAI_THIETBI.Where(a => a.MA_LOAITB == mA_LOAITB)
                                                        .Select(x => new
                                                        {
                                                            x.MA_LOAITB,
                                                            x.TEN_LOAI,
                                                            x.MA_NHOMTB,
                                                            x.GHI_CHU
                                                        }).FirstOrDefault();
                return Json(lOAI_TB, JsonRequestBehavior.AllowGet);
            }

            return null;
        }

        [HttpGet]
        public ActionResult get_THONG_TIN_DON_VI(string mA_DON_VI)
        {
            if (!String.IsNullOrEmpty(mA_DON_VI))
            {
                var temp = Int32.Parse(mA_DON_VI);
                var dON_VI = db.DON_VI.Where(a => a.MA_DON_VI == temp)
                                                        .Select(x => new
                                                        {
                                                            x.MA_DON_VI,
                                                            x.TEN_DON_VI,
                                                            x.DON_VI_CAP_TREN,
                                                            x.DIA_CHI,
                                                            x.DIEN_THOAI,
                                                            x.FAX
                                                        }).FirstOrDefault();
                return Json(dON_VI, JsonRequestBehavior.AllowGet);
            }

            return null;
        }

        [HttpGet]
        public ActionResult get_THONG_TIN_NCC(string mA_NCC)
        {
            if (!String.IsNullOrEmpty(mA_NCC))
            {
                var temp = Int32.Parse(mA_NCC);
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
                var cAU_HINH = db.CAU_HINH.Where(x => x.MA_LOAITB == mA_LOAITB)
                                         .Select(x => new
                                         {
                                             CPU = x.DM_CPU.TEN_CPU,
                                             MAN_HINH = x.DM_MAN_HINH.TEN_MAN_HINH,
                                             RAM = x.DM_RAM.TEN_RAM,
                                             O_CUNG = x.DM_O_CUNG.TEN_O_CUNG,
                                             VGA = x.DM_VGA.TEN_VGA,
                                             HDH = x.DM_HDH.TEN_HDH,
                                             x.KICH_THUOC,
                                             LOAI_MUC = x.DM_LOAI_MUC.TEN_LOAI_MUC,
                                             x.TOC_DO,
                                             x.DO_PHAN_GIAI,                                            
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
        public ActionResult get_THONG_TIN_PHAN_QUYEN(string mA_NHOM)
        {
            //Tìm kiếm
            if (!String.IsNullOrEmpty(mA_NHOM))
            {
                var cHUC_NANG = db.NHOM_ND_CHUCNANG
                    .Where(d => d.MA_NHOM == mA_NHOM)
                    .Select(d => new
                    {
                        d.MA_QUYEN,
                        d.MA_CHUC_NANG,
                        MA_DM = d.DM_CHUC_NANG.CHUC_NANG.MA_CHUC_NANG
                    })
                    .ToList();

                return Json(cHUC_NANG, JsonRequestBehavior.AllowGet);
            }
            return null;
        }        

        public ActionResult get_THONG_TIN_XAC_NHAN(string mA_XAC_NHAN)
        {
            //Tìm tên thiết bị
            if (!String.IsNullOrEmpty(mA_XAC_NHAN))
            {
                var temp = Int32.Parse(mA_XAC_NHAN);
                var xAC_NHAN = db.XAC_NHAN_DIEU_CHUYEN.Where(a => a.MA_XAC_NHAN == temp);
                if (xAC_NHAN.Select(a => a.MA_XUAT_KHO).FirstOrDefault() != null)
                {
                    var rESULT = xAC_NHAN.Select(b => new
                                        {
                                            b.MA_XAC_NHAN,
                                            b.XUAT_KHO.MATB,
                                            b.XUAT_KHO.THIETBI.TENTB,
                                            NGAY_THUC_HIEN = b.XUAT_KHO.NGAY_XUAT.ToString(),
                                            DV_THUC_HIEN = b.XUAT_KHO.DON_VI.TEN_DON_VI,
                                            DV_NHAN = b.XUAT_KHO.DON_VI1.TEN_DON_VI,
                                            MAND_THUC_HIEN = b.XUAT_KHO.MAND_XUAT,
                                            b.XUAT_KHO.SO_LUONG,
                                            b.XUAT_KHO.GHI_CHU,
                                            b.XUAT_KHO.VAN_CHUYEN
                                        }).FirstOrDefault();
                    return Json(rESULT, JsonRequestBehavior.AllowGet);
                }
                else if (xAC_NHAN.Select(a => a.MA_DIEU_CHUYEN).FirstOrDefault() != null)
                {
                    var rESULT = xAC_NHAN.Select(b => new
                                        {
                                            b.MA_XAC_NHAN,
                                            b.DIEU_CHUYEN_THIET_BI.MATB,
                                            b.DIEU_CHUYEN_THIET_BI.THIETBI.TENTB,
                                            NGAY_THUC_HIEN = b.DIEU_CHUYEN_THIET_BI.NGAY_DIEU_CHUYEN.ToString(),
                                            DV_THUC_HIEN = b.DIEU_CHUYEN_THIET_BI.DON_VI.TEN_DON_VI,
                                            DV_NHAN = b.DIEU_CHUYEN_THIET_BI.DON_VI1.TEN_DON_VI,
                                            MAND_THUC_HIEN = b.DIEU_CHUYEN_THIET_BI.MAND_DIEU_CHUYEN,
                                            b.DIEU_CHUYEN_THIET_BI.SO_LUONG,
                                            b.DIEU_CHUYEN_THIET_BI.GHI_CHU,
                                            b.DIEU_CHUYEN_THIET_BI.VAN_CHUYEN
                                        }).FirstOrDefault();
                    return Json(rESULT, JsonRequestBehavior.AllowGet);
                }
            }
            return null;
        }
        #endregion

        #region get_Ten
        //Tìm loại thiết bị
        [HttpGet]
        public ActionResult get_LOAITB(string nHOM_TB)
        {
            if (!String.IsNullOrEmpty(nHOM_TB))
            {
                var lOAI_TB = db.LOAI_THIETBI.Where(a => a.MA_NHOMTB == nHOM_TB)
                                             .Select(a => new
                                             {
                                                 a.MA_LOAITB,
                                                 a.TEN_LOAI
                                             })
                                             .OrderBy(a => a.TEN_LOAI)
                                             .ToList();
                return Json(lOAI_TB, JsonRequestBehavior.AllowGet);
            }
            return null;
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
            var nHOM_TB = db.NHOM_THIETBI.Select(a => new
                                        {
                                            a.MA_NHOMTB,
                                            a.TEN_NHOM
                                        })
                                         .OrderBy(a => a.TEN_NHOM)
                                         .ToList();
            return Json(nHOM_TB, JsonRequestBehavior.AllowGet);
        }

        public ActionResult get_NCC()
        {
            var nCC = db.NHA_CUNG_CAP
                .Select(a => new
                {
                    a.MA_NCC,
                    a.TEN_NCC
                })
                .OrderBy(a => a.TEN_NCC)
                .ToList();
            return Json(nCC, JsonRequestBehavior.AllowGet);
        }

        public ActionResult get_DON_VI()
        {
            var dON_VI = db.DON_VI
                .Select(a => new
                {
                    a.MA_DON_VI,
                    a.TEN_DON_VI
                })
                .OrderBy(a => a.TEN_DON_VI)
                .ToList();

            return Json(dON_VI, JsonRequestBehavior.AllowGet);
        }

        public ActionResult get_DON_VI_NHAP_KHO(string tEN_DANG_NHAP)
        {
            if (!String.IsNullOrEmpty(tEN_DANG_NHAP))
            {
                var dON_VI = db.NGUOI_DUNG.Where(a => a.TEN_DANG_NHAP == tEN_DANG_NHAP)
                                          .Select(a => new
                                          {
                                              a.MA_DON_VI,
                                              a.DON_VI.TEN_DON_VI
                                          })
                                          .FirstOrDefault();

                return Json(dON_VI, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        public ActionResult get_TRANG_THAI()
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
            var nHOM_ND = db.NHOM_NGUOI_DUNG
                .Select(a => new
                {
                    a.MA_NHOM,
                    a.TEN_NHOM
                })
                .OrderBy(a => a.TEN_NHOM)
                .ToList();
            return Json(nHOM_ND, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult get_ND(int mA_DV)
        {
            //var temp = int.Parse(mA_DV);
            var nGUOI_DUNG = db.NGUOI_DUNG
                .Where(a => a.MA_DON_VI == mA_DV)
                .Select(a => new
                {
                    a.MA_ND,
                    a.TEN_ND
                })
                .OrderBy(a => a.TEN_ND)
                .ToList();
            return Json(nGUOI_DUNG, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult get_DV_DIEU_CHUYEN(string mAND_DIEU_CHUYEN)
        {
            if(!String.IsNullOrEmpty(mAND_DIEU_CHUYEN))
            {
                var dONVI = db.NGUOI_DUNG.Where(a => a.TEN_DANG_NHAP == mAND_DIEU_CHUYEN)
                                         .Select(x => new
                                         {
                                             x.MA_DON_VI,
                                             x.DON_VI.TEN_DON_VI,
                                         }).FirstOrDefault();
                return Json(dONVI, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        public ActionResult get_CPU()
        {
            var cPU = db.DM_CPU
                .Select(a => new
                {
                    a.MA_CPU,
                    a.TEN_CPU
                })
                .OrderBy(a => a.TEN_CPU)
                .ToList();
            return Json(cPU, JsonRequestBehavior.AllowGet);
        }

        public ActionResult get_RAM()
        {
            var rAM = db.DM_RAM
                .Select(a => new
                {
                    a.MA_RAM,
                    a.TEN_RAM
                })
                .OrderBy(a => a.TEN_RAM)
                .ToList();
            return Json(rAM, JsonRequestBehavior.AllowGet);
        }

        public ActionResult get_O_CUNG()
        {
            var o_CUNG = db.DM_O_CUNG
                .Select(a => new
                {
                    a.MA_O_CUNG,
                    a.TEN_O_CUNG
                })
                .OrderBy(a => a.TEN_O_CUNG)
                .ToList();
            return Json(o_CUNG, JsonRequestBehavior.AllowGet);
        }

        public ActionResult get_MAN_HINH()
        {
            var mAN_HINH = db.DM_MAN_HINH
                .Select(a => new
                {
                    a.MA_MAN_HINH,
                    a.TEN_MAN_HINH
                })
                .OrderBy(a => a.TEN_MAN_HINH)
                .ToList();
            return Json(mAN_HINH, JsonRequestBehavior.AllowGet);
        }

        public ActionResult get_VGA()
        {
            var vGA = db.DM_VGA
                .Select(a => new
                {
                    a.MA_VGA,
                    a.TEN_VGA
                })
                .OrderBy(a => a.TEN_VGA)
                .ToList();
            return Json(vGA, JsonRequestBehavior.AllowGet);
        }

        public ActionResult get_HDH()
        {
            var hDH = db.DM_HDH
                .Select(a => new
                {
                    a.MA_HDH,
                    a.TEN_HDH
                })
                .OrderBy(a => a.TEN_HDH)
                .ToList();
            return Json(hDH, JsonRequestBehavior.AllowGet);
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
        public ActionResult get_MATB_DIEU_CHUYEN(string mADV)
        {
            if (!String.IsNullOrEmpty(mADV))
            {
                var temp = Int32.Parse(mADV.ToString());
                var tHIETBI = db.THIETBIs.Where(a => a.MA_DV == temp)
                                         .Select(x => new
                                         {
                                             x.MATB,
                                             x.TENTB
                                         }).ToList();

                return Json(tHIETBI, JsonRequestBehavior.AllowGet);
            }
            return null;

            //var maTB_DIEU_CHUYEN = db.DIEU_CHUYEN_THIET_BI.Select(a => a.MATB);
            //var maTB_XUAT_KHO = db.XUAT_KHO.Select(a => a.MATB);

            //var dIEUCHUYEN = db.NHAP_KHO.Where(x => !maTB_XUAT_KHO.Contains(x.MATB) && !maTB_DIEU_CHUYEN.Contains(x.MATB))
            //                            .Select(x => new
            //                            {
            //                                x.MATB,
            //                                x.THIETBI.TENTB
            //                            }).ToList();
            //return Json(dIEUCHUYEN, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}