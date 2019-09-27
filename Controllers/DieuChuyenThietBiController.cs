using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Source.Models;

namespace Source.Controllers
{
    public class DieuChuyenThietBiController : Controller
    {
        private QuanLyTaiSanCNTTEntities db = new QuanLyTaiSanCNTTEntities();

        // GET: /DieuChuyenThietBi/
        public ActionResult Index()
        {
            //var dieu_chuyen_thiet_bi = db.DIEU_CHUYEN_THIET_BI.Include(d => d.DON_VI).Include(d => d.DON_VI1).Include(d => d.THIETBI);
            //return View(await dieu_chuyen_thiet_bi.ToListAsync());
            if(!String.IsNullOrEmpty(Session["CHUC_NANG"].ToString()))
            {
                var pHAN_QUYEN = Session["NHOM_ND"].ToString();
                ViewBag.Them = db.NHOM_ND_CHUCNANG.Where(a => a.MA_CHUC_NANG == 3 && 
                                                         a.MA_QUYEN == 1 && 
                                                         a.MA_NHOM == pHAN_QUYEN).FirstOrDefault();

                ViewBag.Sua = db.NHOM_ND_CHUCNANG.Where(a => a.MA_CHUC_NANG == 3 && 
                                                        a.MA_QUYEN == 3 && 
                                                        a.MA_NHOM == pHAN_QUYEN).FirstOrDefault();
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(FormCollection form, string SAVE)
        {
            var pHAN_QUYEN = Session["NHOM_ND"].ToString();
            ViewBag.Them = db.NHOM_ND_CHUCNANG.Where(a => a.MA_CHUC_NANG == 3 &&
                                                     a.MA_QUYEN == 1 &&
                                                     a.MA_NHOM == pHAN_QUYEN).FirstOrDefault();

            ViewBag.Sua = db.NHOM_ND_CHUCNANG.Where(a => a.MA_CHUC_NANG == 3 &&
                                                    a.MA_QUYEN == 3 &&
                                                    a.MA_NHOM == pHAN_QUYEN).FirstOrDefault();

            if (String.IsNullOrEmpty(form["maTB"]))
            {
                ViewBag.ErrorMessage = "Xin chọn thiết bị";
            }
            else if (String.IsNullOrEmpty(form["MADV_NHAN"]))
            {
                ViewBag.ErrorMessage = "Xin chọn đơn vị tiếp nhận";
            }
            else if (!String.IsNullOrEmpty(SAVE))
            {
                //Tạo điều chuyển thiết bị
                var dieu_chuyen_thiet_bi = new DIEU_CHUYEN_THIET_BI();
                dieu_chuyen_thiet_bi.MATB = Int32.Parse(form["maTB"]);

                var temp = form["MADV_QL"].ToString();
                dieu_chuyen_thiet_bi.MADV_QL = (from p in db.DON_VI
                                                       where p.TEN_DON_VI == temp
                                                       select p.MA_DON_VI).FirstOrDefault();
                temp = form["MADV_NHAN"].ToString();
                dieu_chuyen_thiet_bi.MADV_NHAN = (from p in db.DON_VI
                                                         where p.TEN_DON_VI == temp
                                                         select p.MA_DON_VI).FirstOrDefault();
                temp = form["MAND_NHAN"].ToString();
                dieu_chuyen_thiet_bi.MAND_NHAN = (from p in db.NGUOI_DUNG
                                                  where p.TEN_ND == temp
                                                  select p.MA_ND).FirstOrDefault();

                //dieu_chuyen_thiet_bi.MAND_THUC_HIEN = Session["TEN_DANG_NHAP"].ToString();
                dieu_chuyen_thiet_bi.MAND_THUC_HIEN = "temp";
                dieu_chuyen_thiet_bi.NGAY_CHUYEN = DateTime.Now;
                dieu_chuyen_thiet_bi.GHI_CHU = form["GHI_CHU"];

                //Thêm vào nhật ký thiết bị
                NHAT_KY_THIET_BI nHAT_KY_THIET_BI = new NHAT_KY_THIET_BI();
                nHAT_KY_THIET_BI.MATB = Int32.Parse(form["maTB"]);
                nHAT_KY_THIET_BI.TINH_TRANG = "Được điều chuyển";
                nHAT_KY_THIET_BI.NGAY_THUC_HIEN = DateTime.Now;

                if (ModelState.IsValid)
                {
                    db.DIEU_CHUYEN_THIET_BI.Add(dieu_chuyen_thiet_bi);
                    db.NHAT_KY_THIET_BI.Add(nHAT_KY_THIET_BI);
                    await db.SaveChangesAsync();

                    ViewBag.ErrorMessage = "Thêm thành công";
                }
            }

            return View();
        }

        // GET: /DieuChuyenThietBi/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DIEU_CHUYEN_THIET_BI dieu_chuyen_thiet_bi = await db.DIEU_CHUYEN_THIET_BI.FindAsync(id);
            if (dieu_chuyen_thiet_bi == null)
            {
                return HttpNotFound();
            }
            return View(dieu_chuyen_thiet_bi);
        }

        // GET: /DieuChuyenThietBi/Create
        public ActionResult Create()
        {
            ViewBag.MADV_QL = new SelectList(db.DON_VI, "MA_DON_VI", "TEN_DON_VI");
            ViewBag.MADV_NHAN = new SelectList(db.DON_VI, "MA_DON_VI", "TEN_DON_VI");
            ViewBag.MATB = new SelectList(db.THIETBIs, "MATB", "TENTB");
            return View();
        }

        // POST: /DieuChuyenThietBi/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(DIEU_CHUYEN_THIET_BI dieu_chuyen_thiet_bi)
        {
            if (ModelState.IsValid)
            {
                db.DIEU_CHUYEN_THIET_BI.Add(dieu_chuyen_thiet_bi);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.MADV_QL = new SelectList(db.DON_VI, "MA_DON_VI", "TEN_DON_VI", dieu_chuyen_thiet_bi.MADV_QL);
            ViewBag.MADV_NHAN = new SelectList(db.DON_VI, "MA_DON_VI", "TEN_DON_VI", dieu_chuyen_thiet_bi.MADV_NHAN);
            ViewBag.MATB = new SelectList(db.THIETBIs, "MATB", "TENTB", dieu_chuyen_thiet_bi.MATB);
            return View(dieu_chuyen_thiet_bi);
        }

        // GET: /DieuChuyenThietBi/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DIEU_CHUYEN_THIET_BI dieu_chuyen_thiet_bi = await db.DIEU_CHUYEN_THIET_BI.FindAsync(id);
            if (dieu_chuyen_thiet_bi == null)
            {
                return HttpNotFound();
            }
            ViewBag.MADV_QL = new SelectList(db.DON_VI, "MA_DON_VI", "TEN_DON_VI", dieu_chuyen_thiet_bi.MADV_QL);
            ViewBag.MADV_NHAN = new SelectList(db.DON_VI, "MA_DON_VI", "TEN_DON_VI", dieu_chuyen_thiet_bi.MADV_NHAN);
            ViewBag.MATB = new SelectList(db.THIETBIs, "MATB", "TENTB", dieu_chuyen_thiet_bi.MATB);
            return View(dieu_chuyen_thiet_bi);
        }

        // POST: /DieuChuyenThietBi/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "MA_DIEU_CHUYEN,MATB,MANS_THEO_DOI,MADV_QL,MANS_QL,MADV_NHAN,MANS_NHAN,NGAY_CHUYEN,GHI_CHU")] DIEU_CHUYEN_THIET_BI dieu_chuyen_thiet_bi)
        {
            if (ModelState.IsValid)
            {
                db.Entry(dieu_chuyen_thiet_bi).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.MADV_QL = new SelectList(db.DON_VI, "MA_DON_VI", "TEN_DON_VI", dieu_chuyen_thiet_bi.MADV_QL);
            ViewBag.MADV_NHAN = new SelectList(db.DON_VI, "MA_DON_VI", "TEN_DON_VI", dieu_chuyen_thiet_bi.MADV_NHAN);
            ViewBag.MATB = new SelectList(db.THIETBIs, "MATB", "TENTB", dieu_chuyen_thiet_bi.MATB);
            return View(dieu_chuyen_thiet_bi);
        }

        // GET: /DieuChuyenThietBi/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DIEU_CHUYEN_THIET_BI dieu_chuyen_thiet_bi = await db.DIEU_CHUYEN_THIET_BI.FindAsync(id);
            if (dieu_chuyen_thiet_bi == null)
            {
                return HttpNotFound();
            }
            return View(dieu_chuyen_thiet_bi);
        }

        // POST: /DieuChuyenThietBi/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            DIEU_CHUYEN_THIET_BI dieu_chuyen_thiet_bi = await db.DIEU_CHUYEN_THIET_BI.FindAsync(id);
            db.DIEU_CHUYEN_THIET_BI.Remove(dieu_chuyen_thiet_bi);
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
