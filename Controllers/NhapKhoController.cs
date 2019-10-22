using Source.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Source.Controllers
{
    public class NhapKhoController : Controller
    {
        private QuanLyTaiSanCNTTEntities db = new QuanLyTaiSanCNTTEntities();

        // GET: /NhapKho/
        public ActionResult Index()
        {
            //var nhap_kho = db.NHAP_KHO.Include(n => n.DON_VI).Include(n => n.NGUOI_DUNG).Include(n => n.THIETBI);
            //return View(await nhap_kho.ToListAsync());

            if (Session["CHUC_NANG"] != null)
            {                
                var pHAN_QUYEN = db.NHOM_ND_CHUCNANG.Where(a => a.MA_NHOM == Session["NHOM_ND"].ToString()
                                                             && a.MA_CHUC_NANG == 1);

                ViewBag.Them = pHAN_QUYEN.Where(a => a.MA_QUYEN == 1);
                //ViewBag.Sua = pHAN_QUYEN.Where(a => a.MA_QUYEN == 3);
            }
            else
            {
                return HttpNotFound("You have no accesss permissions at this");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(FormCollection form, string SAVE, HttpPostedFileBase[] hINH_ANH)
        {
            var pHAN_QUYEN = db.NHOM_ND_CHUCNANG.Where(a => a.MA_NHOM == Session["NHOM_ND"].ToString()
                                                             && a.MA_CHUC_NANG == 1);

            ViewBag.Them = pHAN_QUYEN.Where(a => a.MA_QUYEN == 1);
            //ViewBag.Sua = pHAN_QUYEN.Where(a => a.MA_QUYEN == 3);

            //Lưu
            if (!String.IsNullOrEmpty(SAVE))
            {
                var temp = form["SO_SERIAL"].ToString();
                if (form["TENTB"] == null || form["SO_SERIAL"] == null || form["MA_LOAITB"] == null)
                {
                    ViewBag.ErrorMessage = "Xin nhập đầy đủ thông tin";
                }
                else if (db.THIETBIs.FirstOrDefault(x => x.SO_SERIAL == temp) != null)
                {
                    ViewBag.ErrorMessage = "Số Serial đã tồn tại";
                }
                else
                {
                    //Thêm thiết bị
                    var tHIETBI = THEM_THIETBI(form);
                    db.THIETBIs.Add(tHIETBI);
                    await db.SaveChangesAsync();

                    //Thêm nhập kho
                    var cREATE_NHAP_KHO = THEM_NHAP_KHO(tHIETBI.MATB, tHIETBI.MA_DV, tHIETBI.MAND_QL, tHIETBI.SO_LUONG);
                    db.NHAP_KHO.Add(cREATE_NHAP_KHO);
                    await db.SaveChangesAsync();

                    //Thêm vào nhật ký thiết bị
                    NHAT_KY_THIET_BI nHAT_KY_THIET_BI = THEM_NHAT_KY_THIET_BI(tHIETBI.MATB);
                    db.NHAT_KY_THIET_BI.Add(nHAT_KY_THIET_BI);
                    await db.SaveChangesAsync();

                    //Thêm hình ảnh
                    if (hINH_ANH != null)
                    {
                        var hinh_Anh = THEM_HINH_ANH(hINH_ANH, tHIETBI.MATB);
                        db.HINH_ANH.Add(hinh_Anh);
                        await db.SaveChangesAsync();
                    }
                    ViewBag.ErrorMessage = "Thêm thành công";                    
                }
            }
            return View();
        }

        #region Thêm thiết bị
        public THIETBI THEM_THIETBI(FormCollection form)
        {            
            var temp = form["MA_NHOMTB"].ToString();           
            var nHOMTB = db.NHOM_THIETBI.FirstOrDefault(a => a.MA_NHOMTB == temp).MA_NHOMTB;            
            var cOUNT = db.THIETBIs.Where(a => a.LOAI_THIETBI.NHOM_THIETBI.MA_NHOMTB == nHOMTB).Count() + 1;

            THIETBI tHIETBI = new THIETBI();
            tHIETBI.MATB = nHOMTB + "0" + cOUNT;
            tHIETBI.TENTB = form["TENTB"];
            tHIETBI.SO_SERIAL = form["SO_SERIAL"];            
            tHIETBI.MA_LOAITB = form["MA_LOAITB"];

            temp = Session["TEN_DANG_NHAP"].ToString();
            var nGUOI_DUNG = db.NGUOI_DUNG.Where(a => a.TEN_DANG_NHAP == temp).FirstOrDefault();
            tHIETBI.MAND_QL = nGUOI_DUNG.MA_ND;
            tHIETBI.MA_DV = nGUOI_DUNG.MA_DON_VI;

            if (!String.IsNullOrEmpty(form["MA_NCC"]))
            {
                tHIETBI.MA_NCC = Int32.Parse(form["MA_NCC"]);
            }

            tHIETBI.TINH_TRANG = "Mới nhập";
            tHIETBI.NGAY_MUA = DateTime.Parse(form["NGAY_MUA"].ToString());
            tHIETBI.THOI_HAN_BAO_HANH = Int32.Parse(form["THOI_HAN_BAO_HANH"]);
            tHIETBI.THOI_HAN_HET_BAO_HANH = DateTime.Parse(form["THOI_HAN_HET_BAO_HANH"].ToString());

            tHIETBI.SO_LUONG = Int32.Parse(form["SO_LUONG"]);
            if (!String.IsNullOrEmpty(form["GIA_TIEN"]))
            {
                tHIETBI.GIA_TIEN = Decimal.Parse(form["GIA_TIEN"].ToString());
            }
            else
            {
                tHIETBI.GIA_TIEN = 0;
            }

            return tHIETBI;
        }
        #endregion

        #region Tạo cấu hình
        //var cau_Hinh = new CAU_HINH();
        //if (nhomTB == "PC")
        //{
        //    if (!String.IsNullOrEmpty(form["thong_so1"]))
        //    {
        //        temp = form["thong_so1"].ToString();
        //        cau_Hinh.CPU = (from p in db.DM_CPU
        //                        where p.TEN_CPU == temp
        //                        select p.MA_CPU).First();
        //    }

        //    if (!String.IsNullOrEmpty(form["thong_so2"]))
        //    {
        //        temp = form["thong_so2"].ToString();
        //        cau_Hinh.RAM = (from p in db.DM_RAM
        //                        where p.TEN_RAM == temp
        //                        select p.MA_RAM).First();
        //    }

        //    if (!String.IsNullOrEmpty(form["thong_so3"]))
        //    {
        //        temp = form["thong_so3"].ToString();
        //        cau_Hinh.MAN_HINH = (from p in db.DM_MAN_HINH
        //                             where p.TEN_MAN_HINH == temp
        //                             select p.MA_MAN_HINH).First();
        //    }

        //    if (!String.IsNullOrEmpty(form["thong_so4"]))
        //    {
        //        temp = form["thong_so4"].ToString();
        //        cau_Hinh.O_CUNG = (from p in db.DM_O_CUNG
        //                           where p.TEN_O_CUNG == temp
        //                           select p.MA_O_CUNG).First();
        //    }

        //    if (!String.IsNullOrEmpty(form["thong_so5"]))
        //    {
        //        temp = form["thong_so5"].ToString();
        //        cau_Hinh.VGA = (from p in db.DM_VGA
        //                        where p.TEN_VGA == temp
        //                        select p.MA_VGA).First();
        //    }

        //    if (!String.IsNullOrEmpty(form["thong_so6"]))
        //    {
        //        temp = form["thong_so6"].ToString();
        //        cau_Hinh.HE_DIEU_HANH = (from p in db.DM_HDH
        //                                 where p.TEN_HDH == temp
        //                                 select p.MA_HDH).First();
        //    }
        //}
        //else if (nhomTB == "PR")
        //{
        //    if (!String.IsNullOrEmpty(form["input_thong_so1"]))
        //    {
        //        cau_Hinh.KICH_THUOC = form["input_thong_so1"];
        //    }

        //    if (!String.IsNullOrEmpty(form["thong_so2"]))
        //    {
        //        temp = form["thong_so2"].ToString();
        //        cau_Hinh.LOAI_MUC = (from a in db.DM_LOAI_MUC
        //                             where a.TEN_LOAI_MUC == temp
        //                             select a.MA_LOAI_MUC).FirstOrDefault();
        //    }

        //    if (!String.IsNullOrEmpty(form["input_thong_so3"]))
        //    {
        //        cau_Hinh.TOC_DO = form["input_thong_so3"] + "ppm";
        //    }

        //    if (!String.IsNullOrEmpty(form["input_thong_so4"]))
        //    {
        //        cau_Hinh.DO_PHAN_GIAI = form["input_thong_so4"];
        //    }
        //}
        #endregion

        #region Thêm nhập kho
        public NHAP_KHO THEM_NHAP_KHO(string mATB, int? mADV_NHAP, string mAND_QL, int? sO_LUONG)
        {
            NHAP_KHO nHAP_KHO = new NHAP_KHO();
            nHAP_KHO.MATB = mATB;
            nHAP_KHO.MADV_NHAP = mADV_NHAP;
            nHAP_KHO.MAND_NHAP = mAND_QL;
            nHAP_KHO.NGAY_NHAP = DateTime.Now;
            nHAP_KHO.SO_LUONG = sO_LUONG;

            return nHAP_KHO;
        }
        #endregion

        #region Thêm vào nhật ký thiết bị
        public NHAT_KY_THIET_BI THEM_NHAT_KY_THIET_BI(string mATB)
        {
            NHAT_KY_THIET_BI nHAT_KY_THIET_BI = new NHAT_KY_THIET_BI();
            nHAT_KY_THIET_BI.MA_NHAP_KHO = db.NHAP_KHO.FirstOrDefault(a => a.MATB == mATB).MA_NHAP_KHO;
            nHAT_KY_THIET_BI.TINH_TRANG = "Mới nhập";

            return nHAT_KY_THIET_BI;
        }
        #endregion

        #region Thêm hình ảnh
        public HINH_ANH THEM_HINH_ANH(HttpPostedFileBase[] hINH_ANH, string mATB)
        {
            var cREATE_HINH_ANH = new HINH_ANH();
            cREATE_HINH_ANH.MATB = mATB;

            foreach (var file in hINH_ANH)
            {
                if (file.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    var path = Path.Combine(Server.MapPath("~/Images/" + fileName));
                    file.SaveAs(path);

                    if (cREATE_HINH_ANH.HINH1 == null)
                    {
                        cREATE_HINH_ANH.HINH1 = fileName;
                    }
                    else if (cREATE_HINH_ANH.HINH2 == null)
                    {
                        cREATE_HINH_ANH.HINH2 = fileName;
                    }
                    else if (cREATE_HINH_ANH.HINH3 == null)
                    {
                        cREATE_HINH_ANH.HINH3 = fileName;
                    }
                    else if (cREATE_HINH_ANH.HINH4 == null)
                    {
                        cREATE_HINH_ANH.HINH4 = fileName;
                    }
                    else if (cREATE_HINH_ANH.HINH5 == null)
                    {
                        cREATE_HINH_ANH.HINH5 = fileName;
                        break;
                    }
                }
            }

            return cREATE_HINH_ANH;
        }
        #endregion


        // GET: /NhapKho/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NHAP_KHO nhap_kho = await db.NHAP_KHO.FindAsync(id);
            if (nhap_kho == null)
            {
                return HttpNotFound();
            }
            return View(nhap_kho);
        }

        // GET: /NhapKho/Create
        public ActionResult Create(string value)
        {
            ViewBag.MADV_NHAP = new SelectList(db.DON_VI, "MA_DON_VI", "TEN_DON_VI");
            ViewBag.MAND_NHAP = new SelectList(db.NGUOI_DUNG, "MA_ND", "TEN_ND");
            ViewBag.MATB = new SelectList(db.THIETBIs, "MATB", "TENTB");
            return View();
        }

        // POST: /NhapKho/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create()
        {
            var nhap_kho = db.NHAP_KHO.Include(n => n.DON_VI).Include(n => n.NGUOI_DUNG).Include(n => n.THIETBI);
            return View(await nhap_kho.ToListAsync());
        }

        // GET: /NhapKho/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NHAP_KHO nhap_kho = await db.NHAP_KHO.FindAsync(id);
            if (nhap_kho == null)
            {
                return HttpNotFound();
            }
            ViewBag.MADV_NHAP = new SelectList(db.DON_VI, "MA_DON_VI", "TEN_DON_VI", nhap_kho.MADV_NHAP);
            ViewBag.MAND_NHAP = new SelectList(db.NGUOI_DUNG, "MA_ND", "TEN_ND", nhap_kho.MAND_NHAP);
            ViewBag.MATB = new SelectList(db.THIETBIs, "MATB", "TENTB", nhap_kho.MATB);
            return View(nhap_kho);
        }

        // POST: /NhapKho/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "MA_NHAPKHO,MATB,MADV_NHAP,MAND_NHAP,NGAY")] NHAP_KHO nhap_kho)
        {
            if (ModelState.IsValid)
            {
                db.Entry(nhap_kho).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.MADV_NHAP = new SelectList(db.DON_VI, "MA_DON_VI", "TEN_DON_VI", nhap_kho.MADV_NHAP);
            ViewBag.MAND_NHAP = new SelectList(db.NGUOI_DUNG, "MA_ND", "TEN_ND", nhap_kho.MAND_NHAP);
            ViewBag.MATB = new SelectList(db.THIETBIs, "MATB", "TENTB", nhap_kho.MATB);
            return View(nhap_kho);
        }

        // GET: /NhapKho/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NHAP_KHO nhap_kho = await db.NHAP_KHO.FindAsync(id);
            if (nhap_kho == null)
            {
                return HttpNotFound();
            }
            return View(nhap_kho);
        }

        // POST: /NhapKho/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            NHAP_KHO nhap_kho = await db.NHAP_KHO.FindAsync(id);
            db.NHAP_KHO.Remove(nhap_kho);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
